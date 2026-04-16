using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;
    public Collider2D targetCollider;

    [Header("Zona muerta (Dead Zone)")]
    public float deadZoneWidth = 7f;
    public float deadZoneHeight = 3.5f;

    [Header("Suavizado")]
    public float smoothTime = 0.2f;

    [Header("Limites del mapa")]
    public Vector2 minBounds;
    public Vector2 maxBounds;
    

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (target != null && targetCollider == null)
        {
            targetCollider = target.GetComponent<Collider2D>();
        }
    }

    void LateUpdate()
    {
        if (target == null || targetCollider == null) return;

        Vector3 currentPos = transform.position;
        Vector3 targetPos = currentPos;

        Bounds bounds = targetCollider.bounds;

        float deltaX = bounds.center.x - currentPos.x;
        float deltaY = bounds.center.y - currentPos.y;

        float halfWidth = bounds.extents.x;
        float halfHeight = bounds.extents.y;

        // X: comprobar bordes del collider
        if (deltaX + halfWidth > deadZoneWidth)
        {
            float excess = (deltaX + halfWidth) - deadZoneWidth;
            targetPos.x += excess;
        }
        else if (deltaX - halfWidth < -deadZoneWidth)
        {
            float excess = (deltaX - halfWidth) + deadZoneWidth;
            targetPos.x += excess;
        }

        // Y: comprobar bordes del collider
        if (deltaY + halfHeight > deadZoneHeight)
        {
            float excess = (deltaY + halfHeight) - deadZoneHeight;
            targetPos.y += excess;
        }
        else if (deltaY - halfHeight < -deadZoneHeight)
        {
            float excess = (deltaY - halfHeight) + deadZoneHeight;
            targetPos.y += excess;
        }

        targetPos.z = currentPos.z;

        Vector3 smoothPos = Vector3.SmoothDamp(currentPos, targetPos, ref velocity, smoothTime);

        // Tamaño de la cámara
        float camHalfHeight = Camera.main.orthographicSize;
        float camHalfWidth = camHalfHeight * Camera.main.aspect;

        // Clamp
        float clampedX = Mathf.Clamp(
            smoothPos.x,
            minBounds.x + camHalfWidth,
            maxBounds.x - camHalfWidth
        );

        float clampedY = Mathf.Clamp(
            smoothPos.y,
            minBounds.y + camHalfHeight,
            maxBounds.y - camHalfHeight
        );

        transform.position = new Vector3(clampedX, clampedY, smoothPos.z);
            }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(deadZoneWidth * 2, deadZoneHeight * 2, 0));

        // Gizmos del area delimitada
        Gizmos.color = Color.green;

        Vector3 center = (minBounds + maxBounds) / 2;
        Vector3 size = maxBounds - minBounds;

        Gizmos.DrawWireCube(center, size);
    }
}