using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Abilities/FireArrowRain")]
public class FireArrowRainAbility : AbilityBehaviour
{
    [Header("Targeting")]
    public float detectionRadius = 8f;
    public LayerMask enemyLayer;

    [Header("Prefabs")]
    public GameObject areaPrefab;

    public override void Execute(GameObject owner, RuntimePowerUp powerUp)
{
    // Buscar enemigos cerca del player
    Collider2D[] hits = Physics2D.OverlapCircleAll(
        owner.transform.position,
        detectionRadius,
        enemyLayer
    );

    // Si no hay enemigos, no hacer nada
    if (hits.Length == 0) return;

    // Punto aleatorio EXACTAMENTE a distancia 3
    Vector3 bestPosition = owner.transform.position;
    int bestEnemyCount = -1;

    int attempts = 8; // cuantos puntos probar

    for (int i = 0; i < attempts; i++)
    {
        // Punto aleatorio en circunferencia
        Vector2 randomOffset =
            Random.insideUnitCircle.normalized * 3f;

        Vector3 testPosition =
            owner.transform.position +
            new Vector3(randomOffset.x, randomOffset.y, 0f);

        // Contar enemigos cerca de ese punto
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(
            testPosition,
            powerUp.GetRadius(),
            enemyLayer
        );

        int enemyCount = nearbyEnemies.Length;

        // Guardar mejor posición
        if (enemyCount > bestEnemyCount)
        {
            bestEnemyCount = enemyCount;
            bestPosition = testPosition;
        }
    }

    Vector3 spawnPosition = bestPosition;

    owner.GetComponent<PowerUpManager>()
        .StartAbilityCoroutine(
            AreaRoutine(
                spawnPosition,
                powerUp
            )
        );
}

    IEnumerator AreaRoutine(
        Vector3 position,
        RuntimePowerUp powerUp
    )
    {
        GameObject area = Instantiate(
            areaPrefab,
            position,
            Quaternion.identity
        );

        float duration = powerUp.GetDuration();
        float tickRate = powerUp.GetTickRate();
        float radius = powerUp.GetRadius();

        float timer = 0f;
        float tickTimer = 0f;

        while (timer < duration)
        {
            tickTimer -= Time.deltaTime;

            if (tickTimer <= 0f)
            {
                tickTimer = tickRate;

                Collider2D[] hits = Physics2D.OverlapCircleAll(
                    position,
                    radius,
                    enemyLayer
                );

                foreach (var hit in hits)
                {
                    if (hit.CompareTag("Enemy"))
                    {
                        hit.GetComponent<Enemy>()
                            .TakeDamage((int)powerUp.GetDamage());
                    }
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(area);
    }
}