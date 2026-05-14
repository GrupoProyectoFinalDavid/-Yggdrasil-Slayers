using UnityEngine;

public class AreaInstance : MonoBehaviour
{
    private float duration;
    private float tickRate;
    private float tickTimer;
    private float radius;

    private float damage;
    private GameObject owner;

    private Vector3 offset; // 👈 NUEVO

    public System.Action OnDestroyed;

    public void Init(
        float duration,
        float tickRate,
        float damage,
        GameObject owner,
        float radius,
        Vector3 offset // 👈 NUEVO
    )
    {
        this.duration = duration;
        this.tickRate = tickRate;
        this.damage = damage;
        this.owner = owner;
        this.radius = radius;
        this.offset = offset;

        tickTimer = 0f;

        transform.localScale = Vector3.one * radius;
    }

    void Update()
    {
        if (owner != null)
            transform.position = owner.transform.position + offset; // 👈 AQUÍ FIX

        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            OnDestroyed?.Invoke();
            Destroy(gameObject);
            return;
        }

        tickTimer -= Time.deltaTime;
        if (tickTimer > 0) return;

        tickTimer = tickRate;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            radius
        );

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponentInParent<Enemy>();

                if (enemy != null)
                    enemy.TakeDamage((int)damage);
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