using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float time = 0.5f;

    void Start()
    {
        Destroy(gameObject, time);
    }
}