using UnityEngine;
using Unity.Cinemachine;

public class CameraRoomManager : MonoBehaviour
{
    [Header("Cinemachine")]
    public CinemachineCamera virtualCamera;
    public float snapThreshold = 0.1f; 

    private CinemachineConfiner2D _confiner;
    private Camera _cam;

    void Awake()
    {
        _confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
        _cam = Camera.main;
    }

    void Start()
    {
        RoomManager.Instance.OnRoomChanged += OnRoomChanged;
        if (RoomManager.Instance.CurrentRoom != null)
            OnRoomChanged(RoomManager.Instance.CurrentRoom);
    }

    void OnDestroy()
    {
        RoomManager.Instance.OnRoomChanged -= OnRoomChanged;
    }

    private void OnRoomChanged(Room newRoom)
    {
        if (newRoom.cameraBounds == null) return;
        _confiner.BoundingShape2D = newRoom.cameraBounds;
        _confiner.InvalidateBoundingShapeCache();
    }

    public bool IsCameraOnTarget(Transform target)
    {
        Vector2 camPos = _cam.transform.position;
        Vector2 targetPos = target.position;
        return Vector2.Distance(camPos, targetPos) < snapThreshold;
    }
}