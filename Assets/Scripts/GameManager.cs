using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Transform Player          { get; private set; }
    public bool  GameStarted         { get; private set; }
    public float TotalGameTime       { get; private set; }
    public bool  AllRoomsCleared     { get; private set; }

    private int _roomsCleared = 0;
    private readonly List<Room> _registeredRooms = new List<Room>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        if (GameStarted && !AllRoomsCleared)
            TotalGameTime += Time.deltaTime;
    }

    // ── API pública ──────────────────────────────────────────────────────────

    public void RegisterPlayer(Transform playerTransform)
    {
        Player = playerTransform;
        GameStarted = true;
        Debug.Log("[GameManager] Jugador registrado: " + playerTransform.name);
    }

    public void RegisterRoom(Room room)
    {
        if (room == null || _registeredRooms.Contains(room)) return;
        if (room.possibleEvents == null || room.possibleEvents.Length == 0) return;

        _registeredRooms.Add(room);
        Debug.Log($"[GameManager] Sala registrada: '{room.roomName}' (total: {_registeredRooms.Count})");
    }

    public WaveEvent GetActiveWaveEvent()
    {
        foreach (Room room in _registeredRooms)
        {
            if (room == null) continue;
            WaveEvent we = room.ActiveEvent as WaveEvent;
            if (we != null && we.IsRunning) return we;
        }
        return null;
    }

    public SurvivalEvent GetActiveSurvivalEvent()
    {
        foreach (Room room in _registeredRooms)
        {
            if (room == null) continue;
            SurvivalEvent se = room.ActiveEvent as SurvivalEvent;
            if (se != null && se.IsRunning) return se;
        }
        return null;
    }
    // Llamado desde Room.OnRoomCleared → necesita ser público
    public void NotifyRoomCleared()
    {
        _roomsCleared++;
        Debug.Log($"[GameManager] Salas limpias: {_roomsCleared} / {_registeredRooms.Count}");

        if (_roomsCleared >= _registeredRooms.Count)
        {
            AllRoomsCleared = true;
            Debug.Log("[GameManager] ¡Todas las salas completadas!");
        }
    }

}