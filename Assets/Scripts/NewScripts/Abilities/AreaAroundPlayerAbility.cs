using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/AreaAroundPlayer")]
public class AreaAroundPlayerAbility : AbilityBehaviour
{
    public float radius;
    public float tickRate;
    public float duration;
    public GameObject areaPrefab;
    private GameObject currentArea;

    public override void Execute(GameObject owner, RuntimePowerUp powerUp)
    {
        if (currentArea != null) return;

        GameObject area = Instantiate(areaPrefab, owner.transform.position, Quaternion.identity);
        currentArea = area;

        AreaInstance instance = area.GetComponent<AreaInstance>();
        instance.Init(duration, tickRate, powerUp.GetDamage(), owner, radius);

        // Cuando se destruya, liberar referencia
        instance.OnDestroyed += () => currentArea = null;
    }
}