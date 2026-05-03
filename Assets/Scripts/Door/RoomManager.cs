using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }
    private Transform _player;
    [Header("Configuración")]
    public Room startingRoom;
    public ScreenFader fader;
    public CameraRoomManager cameraManager; 
    public float fadeDuration = 0.3f;
    public float doorCooldown = 0.8f;
    public float cameraWaitTimeout = 3f; 

    private Room _currentRoom;
    private bool _isTransitioning = false;

    public Room CurrentRoom => _currentRoom;
    public event System.Action<Room> OnRoomChanged;


    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (startingRoom != null)
            ActivateRoom(startingRoom);
    }


    public void TransitionThroughDoor(Player player, Door from, Door to)
    {
        if (_isTransitioning) return;
        StartCoroutine(DoTransition(player, from, to));
    }

    private IEnumerator DoTransition(Player player, Door from, Door to)
    {
        _isTransitioning = true;

        if (fader != null)
            yield return fader.FadeOut(fadeDuration);

        yield return player.Teleport(to.spawnPoint.position);

        ActivateRoom(to.ownerRoom);
        to.SetCooldown(doorCooldown);
        
        if (cameraManager != null)
        {
            float elapsed = 0f;
            while (!cameraManager.IsCameraOnTarget(player.transform) && elapsed < cameraWaitTimeout)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        
        if (fader != null)
            yield return fader.FadeIn(fadeDuration);

        _isTransitioning = false;
    }

    public void SetPlayer(Transform player)
    {
        _player = player;
    }
    

    private void ActivateRoom(Room room)
    {
        if (_currentRoom != null && _currentRoom != room)
            _currentRoom.OnExit(); // desactiva la anterior

        _currentRoom = room;
        _currentRoom.OnEnter(); // activa la nueva
        OnRoomChanged?.Invoke(_currentRoom);
    }
}