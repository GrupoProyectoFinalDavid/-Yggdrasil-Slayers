using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Abilities/ShootProjectile")]
public class ShootProjectileAbility : AbilityBehaviour
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float speed;

    public enum FireMode
    {
        Fan,        // abanico
        SingleTarget, // uno a uno al más cercano
        Random      // aleatorio entre enemigos
    }

    [Header("Fire Mode")]
    public FireMode fireMode;

    [Header("Single Target Settings")]
    public float singleTargetDelay = 0.15f;
    
    [Header("Targeting")]
    public float detectionRadius;
    public LayerMask enemyLayer;

    [Header("Spawn")]
    public float spawnOffset = 0.5f;
    

    public override void Execute(GameObject owner, RuntimePowerUp powerUp)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            owner.transform.position,
            detectionRadius,
            enemyLayer
        );

        if (hits.Length == 0) return;

        int projectileCount = powerUp.GetProjectileCount();

        switch (fireMode)
        {
            case FireMode.Fan:
                ShootFan(owner, powerUp, hits, projectileCount);
                break;

            case FireMode.SingleTarget:
            owner.GetComponent<PowerUpManager>()
                .StartAbilityCoroutine(
                    ShootSingleTargetCoroutine(owner, powerUp, hits, projectileCount)
                );
            break;

            case FireMode.Random:
                ShootRandom(owner, powerUp, hits, projectileCount);
                break;
        }
    }

    void ShootFan(GameObject owner, RuntimePowerUp powerUp, Collider2D[] hits, int count)
    {
        Transform targetEnemy = hits[Random.Range(0, hits.Length)].transform;
        if (targetEnemy == null) return;

        Vector2 baseDir = (targetEnemy.position - owner.transform.position).normalized;

        float spreadAngle = 15f; // grados entre proyectiles

        for (int i = 0; i < count; i++)
        {
            float offset = (i - (count - 1) / 2f) * spreadAngle;
            Vector2 dir = Quaternion.Euler(0, 0, offset) * baseDir;

            SpawnProjectile(owner, powerUp, dir);
        }
    }

    IEnumerator ShootSingleTargetCoroutine(GameObject owner, RuntimePowerUp powerUp, Collider2D[] hits, int count)
    {
        float delay = singleTargetDelay;

        for (int i = 0; i < count; i++)
        {
            Transform closestEnemy = GetClosestEnemy(owner, hits);
            if (closestEnemy == null) yield break;

            Vector2 dir = (closestEnemy.position - owner.transform.position).normalized;

            SpawnProjectile(owner, powerUp, dir);

            yield return new WaitForSeconds(delay);
        }
    }

    void ShootRandom(GameObject owner, RuntimePowerUp powerUp, Collider2D[] hits, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Collider2D randomEnemy = hits[Random.Range(0, hits.Length)];
            Vector2 dir = (randomEnemy.transform.position - owner.transform.position).normalized;

            SpawnProjectile(owner, powerUp, dir);
        }
    }

    void SpawnProjectile(GameObject owner, RuntimePowerUp powerUp, Vector2 direction)
    {
        Vector3 spawnPosition = owner.transform.position + (Vector3)(direction * spawnOffset);

        GameObject proj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.Init(owner, powerUp.GetDamage(), speed, direction, powerUp);
    }

    Transform GetClosestEnemy(GameObject owner, Collider2D[] hits)
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(owner.transform.position, hit.transform.position);

            if (dist < minDistance)
            {
                minDistance = dist;
                closest = hit.transform;
            }
        }

        return closest;
    }
}