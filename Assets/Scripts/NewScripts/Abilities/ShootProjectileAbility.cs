using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/ShootProjectile")]
public class ShootProjectileAbility : AbilityBehaviour
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float speed;
    
    [Header("Targeting")]
    public float detectionRadius;
    public LayerMask enemyLayer;

    [Header("Spawn")]
    public float spawnOffset = 0.5f;
    

    public override void Execute(GameObject owner, RuntimePowerUp powerUp)
    {
        // Busqueda de enemigos cerca del jugador
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            owner.transform.position,
            detectionRadius,
            enemyLayer
        );

        // Si no hay enemigos, no se dispara el proyectil
        if (hits.Length == 0) return;

        // Encontrar el enemigo más cercano
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float distance = Vector2.Distance(owner.transform.position, hit.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = hit.transform;
            }
        }

        if (closestEnemy == null) return; // Comprobación de seguridad

        // Calcular dirección
        Vector2 direction = (closestEnemy.position - owner.transform.position).normalized;

        // Calcular posición de spawn (para no salir dentro del player)
        Vector3 spawnPosition = owner.transform.position + (Vector3)(direction * spawnOffset);

        // Instanciar proyectil
        GameObject proj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        // Inicializar proyectil
        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.Init(owner, powerUp.GetDamage(), speed, direction, powerUp);
    }
}