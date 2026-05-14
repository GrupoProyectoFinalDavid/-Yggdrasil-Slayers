using UnityEngine;

public class EnemyHealthDrop : MonoBehaviour
{
    [Header("Drop Settings")]
    [Range(0f, 1f)]
    public float dropChance = 0.25f;

    public GameObject healthPickupPrefab;

    public Vector3 spawnOffset = Vector3.zero;

    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        if (enemy != null)
            enemy.OnDeath += TryDrop;
    }

    private void OnDisable()
    {
        if (enemy != null)
            enemy.OnDeath -= TryDrop;
    }

    void TryDrop()
    {
        float roll = Random.value;

        if (roll <= dropChance && healthPickupPrefab != null)
        {
            Instantiate(
                healthPickupPrefab,
                transform.position + spawnOffset,
                Quaternion.identity
            );
        }
    }
}