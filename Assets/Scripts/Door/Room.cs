using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    [Header("Generación")]
    public Vector2Int gridPosition;
    public bool isGeneratedRoom = false;
    public RoomEvent ActiveEvent => _selectedEvent;

    [Header("Configuración")]
    public string roomName = "Sala";
    public Collider2D cameraBounds;

    [Header("Tag del jugador")]
    public string playerTag = "Player";

    [Header("Puertas")]
    public Door[] doors;

    [Header("Eventos de sala (se elige uno al azar)")]
    public RoomEvent[] possibleEvents;

    private RoomEvent _selectedEvent;
    private bool _playerInside  = false;
    private bool _eventStarted  = false;
    private bool _roomCleared   = false;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
        else
            Debug.LogWarning($"[Room] '{roomName}' no tiene Collider2D en el root.");
    }

    // ── Detección de jugador ─────────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        OnEnter();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        OnExit();
    }

    // ── Lógica de sala ───────────────────────────────────────────────────────

    public void OnEnter()
    {
        if (_playerInside) return;
        _playerInside = true;

        Debug.Log($"[Room] Jugador entró en '{roomName}'");

        // Si la sala ya fue completada, no hace nada
        if (_roomCleared) return;

        // Primera vez que entra: elige evento al azar
        if (_selectedEvent == null)
            SelectRandomEvent();

        // Si hay evento pendiente, arranca
        if (_selectedEvent != null && !_selectedEvent.IsComplete && !_eventStarted)
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

    // ── Evento ───────────────────────────────────────────────────────────────

    private void SelectRandomEvent()
    {
        List<RoomEvent> valid = new List<RoomEvent>();
        foreach (RoomEvent e in possibleEvents)
            if (e != null) valid.Add(e);

        Debug.Log($"[Room] possibleEvents tiene {possibleEvents?.Length ?? 0} entradas, {valid.Count} válidas.");

        if (valid.Count == 0) return;

        _selectedEvent = valid[Random.Range(0, valid.Count)];
        _selectedEvent.OnEventCompleted += OnRoomCleared;

        Debug.Log($"[Room] Evento seleccionado: '{_selectedEvent.eventName}'");
    }

    private IEnumerator WaitForSpaceToStart()
    {
        Debug.Log($"[Room] '{roomName}' — esperando SPACE...");

        while (!Input.GetKeyDown(KeyCode.Space))
            yield return null;

        Debug.Log($"[Room] SPACE detectado, arrancando evento.");
        _eventStarted = true;
        _selectedEvent.StartEvent();
    }

    private void OnRoomCleared()
    {
        _roomCleared = true;
        UnlockAllDoors();
    
        // Notifica al GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.NotifyRoomCleared();

        Debug.Log($"[Room] '{roomName}' — completada, puertas desbloqueadas.");
    }

    // ── Puertas ──────────────────────────────────────────────────────────────

    public Door GetDoor(DoorDirection direction)
    {
        if (doors == null) return null;
        foreach (Door door in doors)
            if (door != null && door.direction == direction)
                return door;
        return null;
    }

    public void AssignOwnerToDoors()
    {
        if (doors == null) return;
        foreach (Door door in doors)
            if (door != null)
                door.ownerRoom = this;
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