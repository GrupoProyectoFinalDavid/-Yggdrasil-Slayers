using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour
{
    [Header("Generación")]
    public Vector2Int gridPosition;
    public bool isGeneratedRoom = false;

    [Header("Configuración")]
    public string roomName = "Sala";

    public Collider2D cameraBounds;

    [Header("Puertas")]
    public Door[] doors;

    [Header("Evento de sala")]
    public RoomEvent roomEvent;


    private bool _playerInside = false;
    private bool _eventStarted = false;


    private void Awake()
    {
        if (roomEvent != null)
            roomEvent.OnEventCompleted += OnRoomEventCompleted;
    }

    private void OnDestroy()
    {
        if (roomEvent != null)
            roomEvent.OnEventCompleted -= OnRoomEventCompleted;
    }


    public void OnEnter()
    {
        if (_playerInside) return;
        _playerInside = true;

        Debug.Log($"[Room] Jugador entró en '{roomName}'");

        if (roomEvent != null && !roomEvent.IsComplete && !_eventStarted)
        {
            LockAllDoors();
            StartCoroutine(WaitForSpaceToStart());
        }
    }

    public void OnExit()
    {
        _playerInside = false;
        Debug.Log($"[Room] Jugador salió de '{roomName}'");
    }
    
    public Door GetDoor(DoorDirection direction)
    {
        if (doors == null) return null;

        foreach (Door door in doors)
        {
            if (door != null && door.direction == direction)
                return door;
        }

        return null;
    }
    
    public void AssignOwnerToDoors()
    {
        if (doors == null) return;

        foreach (Door door in doors)
        {
            if (door != null)
                door.ownerRoom = this;
        }
    }


    private IEnumerator WaitForSpaceToStart()
    {
        Debug.Log($"[Room] '{roomName}' — pulsa SPACE para iniciar el evento.");

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        _eventStarted = true;
        roomEvent.StartEvent();
    }


    private void OnRoomEventCompleted()
    {
        UnlockAllDoors();
    }


    private void LockAllDoors()
    {
        if (doors == null) return;
        foreach (Door door in doors)
            if (door != null) door.Lock();

        Debug.Log($"[Room] '{roomName}' — puertas BLOQUEADAS.");
    }

    private void UnlockAllDoors()
    {
        if (doors == null) return;
        foreach (Door door in doors)
            if (door != null) door.Unlock();

        Debug.Log($"[Room] '{roomName}' — puertas DESBLOQUEADAS.");
    }
}