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

        transform.position = Vector3.SmoothDamp(currentPos, targetPos, ref velocity, smoothTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(deadZoneWidth * 2, deadZoneHeight * 2, 0));
    }
}