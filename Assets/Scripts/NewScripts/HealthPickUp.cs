using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Curación")]
    public int healAmount = 25;

    [Header("Opcional")]
    public bool usePercentage = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        PlayerStats stats = collision.GetComponent<PlayerStats>();

        if (stats != null)
        {
            if (usePercentage)
            {
                stats.currentHealth += stats.maxHealth * (healAmount / 100f);
            }
            else
            {
                stats.currentHealth += healAmount;
            }

            stats.currentHealth = Mathf.Clamp(stats.currentHealth, 0, stats.maxHealth);
        }

        Destroy(gameObject);
    }
}