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

        // Offset fijo para evitar vibraciones al girar el enemigo
        Vector3 offset = Vector3.zero;

        // Spawn warning
        GameObject warning = Instantiate(
            warningPrefab,
            target.position + offset,
            Quaternion.identity
        );

        // Seguir posición del enemigo manualmente
        float timer = 0f;

        while (timer < warningDuration)
        {
            if (target == null)
            {
                Destroy(warning);
                yield break;
            }

            // Actualizar posición
            warning.transform.position = target.position + offset;

            timer += Time.deltaTime;
            yield return null;
        }

        // Congelar warning en la última posición
        Vector3 impactPos = warning.transform.position;

        // Lanzar rayo
        GameObject impact = Instantiate(
            impactPrefab,
            impactPos,
            Quaternion.identity
        );

        // Aplicar daño JUSTO cuando empieza el rayo
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            impactPos,
            impactRadius,
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

        // Esperar a que termine la animación del rayo
        Animator impactAnimator = impact.GetComponent<Animator>();

        if (impactAnimator != null)
        {
            float clipLength =
                impactAnimator.GetCurrentAnimatorStateInfo(0).length;

            yield return new WaitForSeconds(clipLength);
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        // Ahora sí destruir warning
        Destroy(warning);
    }
}