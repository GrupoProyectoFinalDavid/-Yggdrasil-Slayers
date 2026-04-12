using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Explosion")]
public class ExplosionAbility : AbilityBehaviour
{
    public float radius;

    public override void Execute(GameObject owner, RuntimePowerUp powerUp)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(owner.transform.position, radius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponent<Enemy>().TakeDamage((int)powerUp.GetDamage());
            }
        }
    }
}