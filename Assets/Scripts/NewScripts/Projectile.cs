using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float damage;
    private float speed;
    private GameObject owner;
    private RuntimePowerUp powerUp;
    private Vector2 direction;
    public GameObject impactEffectPrefab;

    public void Init(GameObject owner, float damage, float speed, Vector2 direction, RuntimePowerUp powerUp)
    {
        this.owner = owner;
        this.damage = damage;
        this.speed = speed;
        this.direction = direction;
        this.powerUp = powerUp;

        // Calcular ángulo hacia el enemigo más cercano
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Evitar colisiones con el propietario
        Collider2D myCollider = GetComponent<Collider2D>();
        Collider2D ownerCollider = owner.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(myCollider, ownerCollider);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage((int)damage);

            if (impactEffectPrefab != null)
            {
                Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}