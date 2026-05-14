using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/FireArrowRain")]
public class FireArrowRainAbility : AbilityBehaviour
{
    [Header("Targeting")]
    public float detectionRadius = 8f;
    public LayerMask enemyLayer;

    [Header("Spawn Settings")]
    public float spawnDistance = 3f;

    [Tooltip("Distancia mínima entre áreas")]
    public float minDistanceBetweenAreas = 2f;

    [Header("Prefabs")]
    public GameObject areaPrefab;

    [Header("Audio")]
    public AudioClip loopSound;
    [Range(0f, 1f)] public float loopVolume = 1f;

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

        int areaCount = powerUp.GetProjectileCount();
        Debug.Log("AREA COUNT: " + areaCount);

        List<Vector3> usedPositions = new List<Vector3>();

        for (int i = 0; i < areaCount; i++)
        {
            Vector3 chosenPosition = Vector3.zero;
            bool foundValid = false;

            for (int attempt = 0; attempt < 15; attempt++)
            {
                Vector2 randomOffset =
                    Random.insideUnitCircle.normalized * spawnDistance;

                Vector3 testPosition =
                    owner.transform.position +
                    new Vector3(randomOffset.x, randomOffset.y, 0f);

                bool tooClose = false;

                foreach (var used in usedPositions)
                {
                    if (Vector3.Distance(testPosition, used)
                        < minDistanceBetweenAreas)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (tooClose)
                    continue;

                chosenPosition = testPosition;
                foundValid = true;
                break;
            }

            if (!foundValid)
            {
                Vector2 fallback =
                    Random.insideUnitCircle.normalized * spawnDistance;

                chosenPosition =
                    owner.transform.position +
                    new Vector3(fallback.x, fallback.y, 0f);
            }

            usedPositions.Add(chosenPosition);
        }

        owner.GetComponent<PowerUpManager>()
            .StartCoroutine(
                FireRainRoutine(owner, powerUp, usedPositions)
            );
    }

    IEnumerator FireRainRoutine(
        GameObject owner,
        RuntimePowerUp powerUp,
        List<Vector3> positions
    )
    {
        foreach (Vector3 pos in positions)
        {
            GameObject area = Instantiate(
                areaPrefab,
                pos,
                Quaternion.identity
            );

            FireArrowRainArea areaScript =
                area.GetComponent<FireArrowRainArea>();

            if (areaScript == null)
            {
                areaScript =
                    area.AddComponent<FireArrowRainArea>();
            }

            areaScript.Init(
                powerUp,
                enemyLayer,
                loopSound,
                loopVolume
            );
        }

        yield return new WaitForSeconds(
            powerUp.GetDuration()
        );

        powerUp.FinishAbility();
    }
}