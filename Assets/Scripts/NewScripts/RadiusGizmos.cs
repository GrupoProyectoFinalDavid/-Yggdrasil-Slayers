using UnityEngine;

public class RadiusGizmo : MonoBehaviour
{
    public float radius = 5f;
    public Color gizmoColor = Color.cyan;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}