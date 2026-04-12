using UnityEngine;

public class ImpactEffect : MonoBehaviour
{
    public float duration = 1f;

    void Start()
    {
        Destroy(gameObject, duration);
    }
}