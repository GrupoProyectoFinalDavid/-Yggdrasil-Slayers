using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Abilities/OrbitShield")]
public class OrbitShieldAbility : AbilityBehaviour
{
    [Header("Shield")]
    public GameObject shieldPrefab;

    [Header("Orbit")]
    public float orbitRadius = 2f;
    public float rotationSpeed = 120f;

    public override void Execute(GameObject owner, RuntimePowerUp powerUp)
    {
        if (owner == null) return;

        owner.GetComponent<PowerUpManager>()
            .StartAbilityCoroutine(
                ShieldRoutine(owner, powerUp)
            );
    }

    IEnumerator ShieldRoutine(
        GameObject owner,
        RuntimePowerUp powerUp
    )
    {
        GameObject container =
            new GameObject("OrbitShields");

        OrbitShieldController controller =
            container.AddComponent<OrbitShieldController>();

        controller.Init(
            owner,
            shieldPrefab,
            powerUp.GetProjectileCount(),
            orbitRadius,
            rotationSpeed,
            powerUp.GetDamage()
        );

        // Mantener activo
        yield return new WaitForSeconds(
            powerUp.GetDuration()
        );

        Destroy(container);
        
        powerUp.FinishAbility();
    }
}