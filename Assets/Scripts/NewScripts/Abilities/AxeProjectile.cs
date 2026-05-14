using UnityEngine;

public class AxeProjectile : MonoBehaviour
{
    private float damage;
    private GameObject owner;
    private RuntimePowerUp powerUp;
    private float lifetime;

    private float rotationSpeed = 1080f;

    public GameObject impactEffectPrefab;

    private Rigidbody2D rb;

    public void Init(
        GameObject owner,
        float damage,
        float speed,
        Vector2 direction,
        RuntimePowerUp powerUp,
        float lifetime,
        float arcForce
    )
    {
        this.owner = owner;
        this.damage = damage;
        this.powerUp = powerUp;
        this.lifetime = lifetime;

        rb = GetComponent<Rigidbody2D>();

        // velocidad horizontal
        Vector2 horizontalVelocity =
            direction.normalized * speed;

        // aplicar movimiento
        rb.linearVelocity = horizontalVelocity;

        // impulso hacia arriba
        rb.AddForce(
            Vector2.up * arcForce,
            ForceMode2D.Impulse
        );

        Destroy(gameObject, lifetime);

        // ignorar colisión con player
        Collider2D myCollider =
            GetComponent<Collider2D>();

        Collider2D ownerCollider =
            owner.GetComponent<Collider2D>();

        Physics2D.IgnoreCollision(
            myCollider,
            ownerCollider
        );
    }

    void Update()
    {
        // girar continuamente
        transform.Rotate(
            0f,
            0f,
            -rotationSpeed * Time.deltaTime
        );
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            PlayerStats stats =
                owner.GetComponent<PlayerStats>();

            float finalDamage =
                DamageCalculator.Calculate(
                    damage,
                    stats
                );

            other.GetComponent<Enemy>()
                .TakeDamage((int)finalDamage);

            if (impactEffectPrefab != null)
            {
                Instantiate(
                    impactEffectPrefab,
                    transform.position,
                    Quaternion.identity
                );
            }

            Destroy(gameObject);
        }
    }
}