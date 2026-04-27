using UnityEngine;

public class CanvasNoRotate : MonoBehaviour
{
    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void LateUpdate()
    {
        // Compensar la escala del padre para que el canvas no se voltee
        float parentScaleX = transform.parent.lossyScale.x;
        transform.localScale = new Vector3(
            initialScale.x * (parentScaleX < 0 ? -1 : 1),
            initialScale.y,
            initialScale.z
        );
    }
}