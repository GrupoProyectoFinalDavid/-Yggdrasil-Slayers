using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    [Header("Config")]
    public GameObject projectilePrefab;
    public float cooldown = 1f;
    public float detectionRadius = 5f;

    private float lastShootTime;

    void Update()
    {
        if (Time.time < lastShootTime + cooldown) return;

        Transform target = GetClosestEnemy();

        if (target == null) return;

        Shoot(target);

        lastShootTime = Time.time;
    }

    Transform GetClosestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        float minDist = Mathf.Infinity;
        Transform closest = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);

                if (dist < minDist)
                {
                    minDist = dist;
                    closest = hit.transform;
                }
            }
        }

        return closest;
    }

    void Shoot(Transform target)
    {
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        Vector2 dir = (target.position - transform.position).normalized;

        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.SetDirection(dir);
    }
}