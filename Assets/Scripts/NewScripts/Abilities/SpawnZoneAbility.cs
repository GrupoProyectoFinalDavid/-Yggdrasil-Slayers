using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/SpawnZone")]
public class SpawnZoneAbility : AbilityBehaviour
{
    public GameObject zonePrefab;

    public override void Execute(GameObject owner, RuntimePowerUp powerUp)
    {
        GameObject.Instantiate(zonePrefab, owner.transform.position, Quaternion.identity);
    }
}