using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Tamaño de la mazmorra")]
    public int width = 3;
    public int height = 4;
    
    [Header("Separación entre salas")]
    public Vector2 roomSpacing = new Vector2(45.5f, 0f);

    [Header("Sala inicial ya colocada en la escena")]
    public Room startRoomInScene;

    [Header("Tipo de sala por fila (índice = fila Y de la matriz)")]
    public RowType[] rowTypes; // Asigna en Inspector: rowTypes[0]=Row0, rowTypes[1]=Row1...

    [Header("Prefabs de salas")]
    public RoomPrefabInfo bossRoomPrefab;
    public RoomPrefabInfo[] normalRoomPrefabs;

    [Header("Referencias")]
    public RoomManager roomManager;

    private Dictionary<Vector2Int, Room> spawnedRooms = new Dictionary<Vector2Int, Room>();
    private Dictionary<Vector2Int, RoomLayoutData> layout = new Dictionary<Vector2Int, RoomLayoutData>();

    private Vector2Int startPosition = new Vector2Int(1, 0);
    private Vector2Int bossBasePosition;
    private Vector2Int bossPosition;

    private void Start()
    {
        GenerateLayout();
        RegisterStartRoom();
    }

    private void GenerateLayout()
    {
        layout.Clear();

        bossBasePosition = new Vector2Int(Random.Range(0, width), height - 1);
        bossPosition = bossBasePosition + Vector2Int.up;

        for (int y = 0; y < height; y++)
        {
            // Determina el RowType de esta fila, con fallback a Row0 si el array es corto
            RowType rowType = (rowTypes != null && y < rowTypes.Length) ? rowTypes[y] : RowType.Row0;

            for (int x = 0; x < width; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                RoomLayoutData data = new RoomLayoutData();

                data.roomType = RoomType.Normal;
                data.rowType = rowType; // <-- asignamos el tipo de fila

                data.hasUp    = y < height - 1;
                data.hasDown  = y > 0;
                data.hasLeft  = x > 0;
                data.hasRight = x < width - 1;

                if (pos == startPosition)
                {
                    data.hasUp    = true;
                    data.hasDown  = false;
                    data.hasLeft  = true;
                    data.hasRight = true;
                }

                if (pos == bossBasePosition)
                {
                    data.hasUp = true;
                }

                layout[pos] = data;
            }
        }

        layout[bossPosition] = new RoomLayoutData
        {
            roomType = RoomType.Boss,
            rowType  = RowType.Row0, // la sala boss no usa rowType, pero necesita un valor
            hasUp    = false,
            hasDown  = true,
            hasLeft  = false,
            hasRight = false
        };
    }

    private void RegisterStartRoom()
    {
        if (startRoomInScene == null)
        {
            Debug.LogError("[DungeonGenerator] Falta asignar Start Room In Scene.");
            return;
        }

        startRoomInScene.gridPosition = startPosition;
        startRoomInScene.isGeneratedRoom = false;
        startRoomInScene.AssignOwnerToDoors();

        spawnedRooms[startPosition] = startRoomInScene;

        if (roomManager != null)
            roomManager.startingRoom = startRoomInScene;

        Debug.Log("[DungeonGenerator] Sala inicial registrada en " + startPosition);
    }

    public Room SpawnRoom(Vector2Int position)
    {
        if (spawnedRooms.ContainsKey(position))
            return spawnedRooms[position];

        if (!layout.ContainsKey(position))
        {
            Debug.LogWarning("[DungeonGenerator] No existe sala en la posición: " + position);
            return null;
        }

        RoomLayoutData data = layout[position];
        RoomPrefabInfo selectedPrefab = GetPrefabFor(data);

        if (selectedPrefab == null)
        {
            Debug.LogError(
                "[DungeonGenerator] No hay prefab compatible para " + position +
                " | RowType=" + data.rowType +
                " Up=" + data.hasUp +
                " Down=" + data.hasDown +
                " Left=" + data.hasLeft +
                " Right=" + data.hasRight
            );
            return null;
        }

        Vector3 worldPosition = GridToWorld(position);
        RoomPrefabInfo instance = Instantiate(selectedPrefab, worldPosition, Quaternion.identity);
        Room room = instance.GetComponent<Room>();

        if (room == null)
        {
            Debug.LogError("[DungeonGenerator] El prefab no tiene componente Room: " + selectedPrefab.name);
            Destroy(instance.gameObject);
            return null;
        }

        room.gridPosition = position;
        room.isGeneratedRoom = true;
        room.AssignOwnerToDoors();

        DisableUnusedDoors(room, data);
        spawnedRooms[position] = room;
        ConnectWithNeighbours(position, room);

        Debug.Log("[DungeonGenerator] Sala generada en " + position + " [" + data.rowType + "]: " + room.name);

        return room;
    }

    private RoomPrefabInfo GetPrefabFor(RoomLayoutData data)
    {
        if (data.roomType == RoomType.Boss)
            return bossRoomPrefab;

        List<RoomPrefabInfo> validPrefabs = new List<RoomPrefabInfo>();

        foreach (RoomPrefabInfo prefab in normalRoomPrefabs)
        {
            if (prefab == null) continue;

            // Filtra por puertas Y por la fila a la que pertenece el prefab
            bool matchesDoors = prefab.Matches(data.hasUp, data.hasDown, data.hasLeft, data.hasRight);
            bool matchesRow   = prefab.rowType == data.rowType;

            if (matchesDoors && matchesRow)
                validPrefabs.Add(prefab);
        }

        if (validPrefabs.Count == 0)
            return null;

        return validPrefabs[Random.Range(0, validPrefabs.Count)];
    }

    private void DisableUnusedDoors(Room room, RoomLayoutData data)
    {
        foreach (Door door in room.doors)
        {
            if (door == null) continue;

            bool shouldBeActive = false;

            if (door.direction == DoorDirection.Up)    shouldBeActive = data.hasUp;
            if (door.direction == DoorDirection.Down)  shouldBeActive = data.hasDown;
            if (door.direction == DoorDirection.Left)  shouldBeActive = data.hasLeft;
            if (door.direction == DoorDirection.Right) shouldBeActive = data.hasRight;

            door.gameObject.SetActive(shouldBeActive);
        }
    }

    private void ConnectWithNeighbours(Vector2Int position, Room room)
    {
        TryConnect(position, room, Vector2Int.up,    DoorDirection.Up,    DoorDirection.Down);
        TryConnect(position, room, Vector2Int.down,  DoorDirection.Down,  DoorDirection.Up);
        TryConnect(position, room, Vector2Int.left,  DoorDirection.Left,  DoorDirection.Right);
        TryConnect(position, room, Vector2Int.right, DoorDirection.Right, DoorDirection.Left);
    }

    private void TryConnect(
        Vector2Int position,
        Room room,
        Vector2Int offset,
        DoorDirection myDirection,
        DoorDirection neighbourDirection)
    {
        Vector2Int neighbourPosition = position + offset;

        if (!spawnedRooms.ContainsKey(neighbourPosition))
            return;

        Room neighbourRoom = spawnedRooms[neighbourPosition];

        Door myDoor        = room.GetDoor(myDirection);
        Door neighbourDoor = neighbourRoom.GetDoor(neighbourDirection);

        if (myDoor == null || neighbourDoor == null)
            return;

        myDoor.connectedDoor        = neighbourDoor;
        neighbourDoor.connectedDoor = myDoor;
    }

    private Vector3 GridToWorld(Vector2Int position)
    {
        Vector3 startWorldPosition = startRoomInScene.transform.position;
        Vector2Int offset = position - startPosition;

        return startWorldPosition + new Vector3(
            offset.x * roomSpacing.x,
            offset.y * roomSpacing.y,
            0f
        );
    }

    private class RoomLayoutData
    {
        public RoomType roomType = RoomType.Normal;
        public RowType  rowType  = RowType.Row0;   // <-- nuevo
        public bool hasUp;
        public bool hasDown;
        public bool hasLeft;
        public bool hasRight;
    }
}