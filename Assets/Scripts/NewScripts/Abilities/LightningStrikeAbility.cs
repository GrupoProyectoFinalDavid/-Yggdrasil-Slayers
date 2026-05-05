using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Abilities/LightningStrike")]
public class LightningStrikeAbility : AbilityBehaviour
{
    [Header("Targeting")]
    public float detectionRadius = 6f;
    public LayerMask enemyLayer;

    [Header("Timing")]
    public float warningDuration = 0.7f;

    [Header("Area Damage")]
    public float impactRadius = 2.5f;
    public float damage;

    [Header("Prefabs")]
    public GameObject warningPrefab;
    public GameObject impactPrefab;

    public override void Execute(GameObject owner, RuntimePowerUp powerUp)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            owner.transform.position,
            detectionRadius,
            enemyLayer
        );

        if (hits.Length == 0) return;

        Collider2D target = hits[Random.Range(0, hits.Length)];

        owner.GetComponent<PowerUpManager>()
            .StartAbilityCoroutine(StrikeRoutine(target.transform, powerUp));
    }

    IEnumerator StrikeRoutine(Transform target, RuntimePowerUp powerUp)
    {
        if (target == null) yield break;

        // 1. Spawn warning
        GameObject warning = Instantiate(warningPrefab, target.position, Quaternion.identity);

        // 2. Hacer que siga al enemigo
        warning.transform.SetParent(target);

        yield return new WaitForSeconds(warningDuration);

        if (target == null)
        {
            Destroy(warning);
            yield break;
        }

        // 3. Spawn impacto en posición ACTUAL del enemigo
        Vector3 impactPos = target.position;

        Instantiate(impactPrefab, impactPos, Quaternion.identity);

        // 4. Daño en área en la posición ACTUAL
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            impactPos,
            impactRadius,
            enemyLayer
        );

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponent<Enemy>().TakeDamage((int)powerUp.GetDamage());
            }
        }

        Destroy(warning);
    }
}