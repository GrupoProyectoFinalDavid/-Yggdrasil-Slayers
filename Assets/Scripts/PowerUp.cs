using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerUpType type;

    [Header("Weapon (opcional)")]
    public GameObject projectilePrefab;
    public float cooldown;
    public float detectionRadius;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.AddPowerUp(type);

                // 👉 añadir comportamiento de arma
                if (projectilePrefab != null)
                {
                    AutoShooter shooter = player.gameObject.AddComponent<AutoShooter>();

                    shooter.projectilePrefab = projectilePrefab;
                    shooter.cooldown = cooldown;
                    shooter.detectionRadius = detectionRadius;
                }
            }

            Destroy(gameObject);
        }
    }
}