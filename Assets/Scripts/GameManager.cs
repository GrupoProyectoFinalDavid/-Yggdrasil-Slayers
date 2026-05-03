using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Salas del nivel")]
    public Room[] rooms;
    
    public bool  GameStarted       { get; private set; }
    public float TotalGameTime     { get; private set; }
    public bool  AllRoomsCleared   { get; private set; }

    private int _roomsCleared = 0;
    
    private void Start()
    {
        GameStarted = true;  // ← arranca al cargar la escena

        foreach (Room room in rooms)
        {
            if (room != null && room.roomEvent != null)
                room.roomEvent.OnEventCompleted += OnRoomCleared;
        }
    }

    private void Update()
    {
        if (GameStarted && !AllRoomsCleared)
            TotalGameTime += Time.deltaTime;
    }


    private void OnRoomCleared()
    {
        if (!GameStarted) GameStarted = true;

        _roomsCleared++;
        Debug.Log($"[GameManager] Salas limpias: {_roomsCleared} / {rooms.Length}");

        if (_roomsCleared >= CountRoomsWithEvents())
        {
            AllRoomsCleared = true;
            Debug.Log("[GameManager] ¡Todas las salas completadas!");
        }
    }
    
    private int CountRoomsWithEvents()
    {
        int count = 0;
        foreach (Room room in rooms)
            if (room != null && room.roomEvent != null)
                count++;
        return count;
    }
    
    public WaveEvent GetActiveWaveEvent()
    {
        foreach (Room room in rooms)
        {
            if (room == null) continue;
            WaveEvent we = room.roomEvent as WaveEvent;
            if (we != null && we.IsRunning) return we;
        }
        return null;
    }
    
    public SurvivalEvent GetActiveSurvivalEvent()
    {
        foreach (Room room in rooms)
        {
            if (room == null) continue;
            SurvivalEvent se = room.roomEvent as SurvivalEvent;
            if (se != null && se.IsRunning) return se;
        }
        return null;
    }
}