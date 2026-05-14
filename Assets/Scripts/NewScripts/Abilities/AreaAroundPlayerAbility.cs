using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Abilities/AreaAroundPlayer")]
public class AreaAroundPlayerAbility : AbilityBehaviour
{
    public float baseRadius;
    public float baseTickRate;
    public float baseDuration;
    public float baseDamage;

    public float yOffset = 1.5f;

    public GameObject areaPrefab;

    private AreaInstance currentArea;

    public override void Execute(GameObject owner, RuntimePowerUp powerUp)
    {
        if (currentArea != null)
            return;

        float radius = baseRadius + powerUp.GetRadius();
        float tickRate = Mathf.Max(0.1f, baseTickRate - powerUp.GetTickRate());
        float duration = baseDuration + powerUp.GetDuration();
        float damage = baseDamage + powerUp.GetDamage();

        Vector3 offset = Vector3.up * yOffset;

        GameObject area = Instantiate(
            areaPrefab,
            owner.transform.position + offset,
            Quaternion.identity
        );

        currentArea = area.GetComponent<AreaInstance>();

        currentArea.Init(
            duration,
            tickRate,
            damage,
            owner,
            radius,
            offset // 👈 IMPORTANTE
        );

        currentArea.OnDestroyed += () =>
        {
            currentArea = null;
            owner.GetComponent<PowerUpManager>()
                .StartCoroutine(Unlock(powerUp));
        };

        area.transform.localScale = Vector3.one * radius;
    }

    private IEnumerator Unlock(RuntimePowerUp powerUp)
    {
        yield return null;
        powerUp.FinishAbility();
    }
}