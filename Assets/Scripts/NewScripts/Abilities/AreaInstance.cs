using UnityEngine;

public class AreaInstance : MonoBehaviour
{
    private float duration;
    private float tickRate;
    private float timer;
    private float tickTimer;
    private float radius;

    private float damage;
    private GameObject owner;
    public System.Action OnDestroyed;

    public void Init(float duration, float tickRate, float damage, GameObject owner, float radius)
    {
        this.duration = duration;
        this.tickRate = tickRate;
        this.damage = damage;
        this.owner = owner;
        this.radius = radius;

        Debug.Log("Area owner: " + owner.name);
    }

    void Update()
    {
        // Seguir al player
        if (owner != null)
            transform.position = owner.transform.position;

        // Duración total
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            Destroy(gameObject);
            return;
        }

        // Tick de daño
        tickTimer -= Time.deltaTime;
        if (tickTimer > 0) return;

        tickTimer = tickRate;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1f); // luego ajustas radio

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponent<Enemy>().TakeDamage((int)damage);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }
}