using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float damage;
    private float speed;
    private GameObject owner;
    public GameObject impactEffectPrefab;

    public void Init(GameObject owner, float damage, float speed)
    {
        this.owner = owner;
        this.damage = damage;
        this.speed = speed;

        // Evitar colisiones con el propietario
        Collider2D myCollider = GetComponent<Collider2D>();
        Collider2D ownerCollider = owner.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(myCollider, ownerCollider);
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
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