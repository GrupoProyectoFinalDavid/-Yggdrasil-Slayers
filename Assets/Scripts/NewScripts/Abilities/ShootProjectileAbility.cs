using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Abilities/ShootProjectile")]
public class ShootProjectileAbility : AbilityBehaviour
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float speed;

    [Header("Lifetime")]
    public float lifetime = 5f;

    [Header("Audio")]
    public AudioClip shootSound;
    [Range(0f, 1f)] public float shootVolume = 1f;

    public enum FireMode
    {
        Fan,        // abanico
        SingleTarget, // uno a uno al más cercano
        Random,      // aleatorio entre enemigos
        Parallel,      // todos a la vez
        ArcThrow       // lanzamiento en arco
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

    [Header("Parallel Settings")]
    public float parallelSpacing = 0.6f;
    public float verticalSpawnOffset = 1f;
    private Vector2 lastMoveDirection = Vector2.down;
    private Vector3 lastOwnerPosition;

    [Header("Arc Throw Settings")]
    public float arcForce = 4f;
    public float throwHeightOffset = 1f;
    

    public override void Execute(GameObject owner, RuntimePowerUp powerUp)
    {
        UpdateLastMoveDirection(owner);
        if (lastOwnerPosition == Vector3.zero)
        {
            lastOwnerPosition = owner.transform.position;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            owner.transform.position,
            detectionRadius,
            enemyLayer
        );

        if (hits.Length == 0){
            powerUp.FinishAbility();
            return;
        }

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

            case FireMode.Parallel:
                ShootParallel(owner, powerUp, projectileCount);
                break;

            case FireMode.ArcThrow:
                ShootArcThrow(owner, powerUp, hits, projectileCount);
                break;
        }
    }

    void UpdateLastMoveDirection(GameObject owner)
    {
        Vector3 currentPosition = owner.transform.position;

        Vector3 delta = currentPosition - lastOwnerPosition;

        // Solo actualizar si realmente hay movimiento
        if (delta.sqrMagnitude > 0.000001f)
        {
            Vector2 dir = new Vector2(delta.x, delta.y).normalized;

            // mantener 8 direcciones limpias
            dir.x = Mathf.Round(dir.x);
            dir.y = Mathf.Round(dir.y);

            if (dir.sqrMagnitude > 0.01f)
            {
                lastMoveDirection = dir.normalized;
            }
        }

        lastOwnerPosition = currentPosition;
    }

    void ShootFan(GameObject owner, RuntimePowerUp powerUp, Collider2D[] hits, int count)
    {
        Transform targetEnemy = hits[Random.Range(0, hits.Length)].transform;
        if (targetEnemy == null) return;

        Vector2 baseDir = (targetEnemy.position - owner.transform.position).normalized;

        float spreadAngle = 15f; // grados entre proyectiles

        PlayShootSound(owner.transform.position);
        for (int i = 0; i < count; i++)
        {
            float offset = (i - (count - 1) / 2f) * spreadAngle;
            Vector2 dir = Quaternion.Euler(0, 0, offset) * baseDir;

            SpawnProjectile(owner, powerUp, dir);
        }

        powerUp.FinishAbility();
    }

    IEnumerator ShootSingleTargetCoroutine(
        GameObject owner,
        RuntimePowerUp powerUp,
        Collider2D[] initialHits,
        int count
    )
    {
        float delay = singleTargetDelay;

        for (int i = 0; i < count; i++)
        {
            // Buscar enemigos ACTUALES
            Collider2D[] currentHits = Physics2D.OverlapCircleAll(
                owner.transform.position,
                detectionRadius,
                enemyLayer
            );

            // Si ya no hay enemigos
            if (currentHits.Length == 0)
            {
                powerUp.FinishAbility();
                yield break;
            }

            Transform closestEnemy =
                GetClosestEnemy(owner, currentHits);

            if (closestEnemy == null)
            {
                powerUp.FinishAbility();
                yield break;
            }

            Vector2 dir =
                (closestEnemy.position - owner.transform.position)
                .normalized;

            if (i == 0)
            {
                PlayShootSound(owner.transform.position);
            }

            SpawnProjectile(owner, powerUp, dir);

            yield return new WaitForSeconds(delay);
        }

        powerUp.FinishAbility();
    }

    void ShootRandom(GameObject owner, RuntimePowerUp powerUp, Collider2D[] hits, int count)
    {
        PlayShootSound(owner.transform.position);
        for (int i = 0; i < count; i++)
        {
            Collider2D randomEnemy = hits[Random.Range(0, hits.Length)];
            Vector2 dir = (randomEnemy.transform.position - owner.transform.position).normalized;

            SpawnProjectile(owner, powerUp, dir);
        }

        powerUp.FinishAbility();
    }

    void SpawnProjectile(
        GameObject owner,
        RuntimePowerUp powerUp,
        Vector2 direction
    )
    {
        Vector3 spawnPosition =
            owner.transform.position +
            (Vector3)(direction * spawnOffset);

        GameObject proj = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        Projectile projectile = proj.GetComponent<Projectile>();

        if (projectile == null)
        {
            Debug.LogError(
                "El prefab no tiene componente Projectile"
            );

            return;
        }

        projectile.Init(
            owner,
            powerUp.GetDamage(),
            speed,
            direction,
            powerUp,
            lifetime
        );
    }

    void PlayShootSound(Vector3 position)
    {
        if (shootSound != null)
        {
            AudioSource.PlayClipAtPoint(
                shootSound,
                position,
                shootVolume
            );
        }
    }

    Transform GetClosestEnemy(
        GameObject owner,
        Collider2D[] hits
    )
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            // MUY IMPORTANTE
            if (hit == null)
                continue;

            if (hit.transform == null)
                continue;

            float dist = Vector2.Distance(
                owner.transform.position,
                hit.transform.position
            );

            if (dist < minDistance)
            {
                minDistance = dist;
                closest = hit.transform;
            }
        }

        return closest;
    }

    void ShootParallel(
        GameObject owner,
        RuntimePowerUp powerUp,
        int count
    )
    {
        Vector2 direction = lastMoveDirection.normalized;

        if (direction == Vector2.zero)
        {
            direction = Vector2.down;
        }

        // perpendicular para separar proyectiles
        Vector2 perpendicular =
            new Vector2(-direction.y, direction.x);

        PlayShootSound(owner.transform.position);

        for (int i = 0; i < count; i++)
        {
            float sideOffset =
                (i - (count - 1) / 2f)
                * parallelSpacing;

            Vector3 spawnPosition =
                owner.transform.position +
                Vector3.up * verticalSpawnOffset +
                (Vector3)(perpendicular * sideOffset);

            SpawnParallelProjectile(
                owner,
                powerUp,
                direction,
                spawnPosition
            );
        }

        powerUp.FinishAbility();
    }

    void SpawnParallelProjectile(
        GameObject owner,
        RuntimePowerUp powerUp,
        Vector2 direction,
        Vector3 spawnPosition
    )
    {
        GameObject proj = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        Projectile projectile =
            proj.GetComponent<Projectile>();

        projectile.Init(
            owner,
            powerUp.GetDamage(),
            speed,
            direction,
            powerUp,
            lifetime
        );
    }

    void ShootArcThrow(
        GameObject owner,
        RuntimePowerUp powerUp,
        Collider2D[] hits,
        int count
    )
    {
        PlayShootSound(owner.transform.position);

        // SOLO UN ENEMIGO PARA TODOS LOS HACHAZOS
        Collider2D target = hits[Random.Range(0, hits.Length)];

        if (target == null)
        {
            powerUp.FinishAbility();
            return;
        }

        Vector2 direction =
            (target.transform.position - owner.transform.position).normalized;

        Vector3 spawnPositionBase =
            owner.transform.position + Vector3.up * throwHeightOffset;

        for (int i = 0; i < count; i++)
        {
            SpawnArcProjectile(
                owner,
                powerUp,
                direction,
                spawnPositionBase
            );
        }

        powerUp.FinishAbility();
    }

    void SpawnArcProjectile(
        GameObject owner,
        RuntimePowerUp powerUp,
        Vector2 direction,
        Vector3 spawnPosition
    )
    {
        GameObject proj = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        AxeProjectile projectile =
            proj.GetComponent<AxeProjectile>();

        if (projectile == null)
        {
            Debug.LogError(
                "El prefab no tiene AxeProjectile"
            );

            return;
        }

        projectile.Init(
            owner,
            powerUp.GetDamage(),
            speed,
            direction,
            powerUp,
            lifetime,
            arcForce
        );
    }   
}