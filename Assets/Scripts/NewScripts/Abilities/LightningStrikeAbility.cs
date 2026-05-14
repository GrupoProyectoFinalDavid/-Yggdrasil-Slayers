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

    [Header("Audio")]
    public AudioClip strikeSound;
    [Range(0f, 1f)] public float strikeVolume = 1f;

    public override void Execute(GameObject owner, RuntimePowerUp powerUp)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            owner.transform.position,
            detectionRadius,
            enemyLayer
        );

        if (hits.Length == 0)
        {
            powerUp.FinishAbility();
            return;
        }

        owner.GetComponent<PowerUpManager>()
            .StartCoroutine(
                MultiStrikeRoutine(owner, powerUp, hits)
            );
    }

    IEnumerator MultiStrikeRoutine(
        GameObject owner,
        RuntimePowerUp powerUp,
        Collider2D[] hits
    )
    {
        int strikeCount = powerUp.GetProjectileCount();

        PowerUpManager manager =
            owner.GetComponent<PowerUpManager>();

        System.Collections.Generic.Dictionary<Transform, int> targetCounts =
            new System.Collections.Generic.Dictionary<Transform, int>();

        for (int i = 0; i < strikeCount; i++)
        {
            Collider2D[] currentHits =
                Physics2D.OverlapCircleAll(
                    owner.transform.position,
                    detectionRadius,
                    enemyLayer
                );

            if (currentHits.Length == 0)
                break;

            Collider2D selectedTarget = null;

            // Intentar encontrar enemigo NO repetido
            System.Collections.Generic.List<Collider2D> availableTargets =
                new System.Collections.Generic.List<Collider2D>();

            foreach (var hit in currentHits)
            {
                if (hit == null)
                    continue;

                if (!targetCounts.ContainsKey(hit.transform))
                {
                    availableTargets.Add(hit);
                }
            }

            // Si hay enemigos libres, elegir uno
            if (availableTargets.Count > 0)
            {
                selectedTarget =
                    availableTargets[
                        Random.Range(0, availableTargets.Count)
                    ];

                if (!targetCounts.ContainsKey(selectedTarget.transform))
                {
                    targetCounts[selectedTarget.transform] = 0;
                }
            }
            else
            {
                // Todos usados -> repetir uno aleatorio
                selectedTarget =
                    currentHits[
                        Random.Range(0, currentHits.Length)
                    ];
            }

            if (selectedTarget == null)
                continue;

            Vector3 offset = Vector3.zero;

            // Registrar cuántos rayos tiene este enemigo
            if (!targetCounts.ContainsKey(selectedTarget.transform))
            {
                targetCounts[selectedTarget.transform] = 0;
            }

            targetCounts[selectedTarget.transform]++;

            // Si ya tenía uno o más rayos -> aplicar offset
            if (targetCounts[selectedTarget.transform] > 1)
            {
                offset =
                    (Vector3)Random.insideUnitCircle * 0.8f;
            }

            manager.StartCoroutine(
                StrikeRoutine(
                    selectedTarget.transform,
                    powerUp,
                    offset
                )
            );

            yield return new WaitForSeconds(0.05f);
        }
        // esperar suficiente tiempo para que terminen los rayos
        yield return new WaitForSeconds(
            warningDuration + 1f
        );

        powerUp.FinishAbility();
    }

    IEnumerator StrikeRoutine(
        Transform target, 
        RuntimePowerUp powerUp,
        Vector3 offset
    ){
        if (target == null){
            yield break;
        }

        // Spawn warning
        GameObject warning = Instantiate(
            warningPrefab,
            target.position + offset,
            Quaternion.identity
        );

        Vector3 lastKnownPosition = target.position + offset;

        // Seguir posición del enemigo manualmente
        float timer = 0f;

        while (timer < warningDuration)
        {
            if (target != null)
            {
                lastKnownPosition =
                    target.position + offset;

                warning.transform.position =
                    lastKnownPosition;
            }
            else
            {
                // enemigo muerto:
                // mantener última posición válida
                warning.transform.position =
                    lastKnownPosition;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Congelar warning en la última posición
        Vector3 impactPos = lastKnownPosition;

        // Lanzar rayo
        GameObject impact = Instantiate(
            impactPrefab,
            impactPos,
            Quaternion.identity
        );

        // Sonido del rayo
        if (strikeSound != null)
        {
            AudioSource.PlayClipAtPoint(
                strikeSound,
                impactPos,
                strikeVolume
            );
        }

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