using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/ShootProjectile")]
public class ShootProjectileAbility : AbilityBehaviour
{
    public GameObject projectilePrefab;
    public float speed;

    public override void Execute(GameObject owner, RuntimePowerUp powerUp)
    {
        GameObject proj = GameObject.Instantiate(projectilePrefab, owner.transform.position, Quaternion.identity);

        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.Init(owner, powerUp.GetDamage(), speed);
    }
}