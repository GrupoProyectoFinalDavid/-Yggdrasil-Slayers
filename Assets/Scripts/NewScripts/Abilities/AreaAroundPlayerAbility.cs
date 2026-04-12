using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/AreaAroundPlayer")]
public class AreaAroundPlayerAbility : AbilityBehaviour
{
    public float radius;
    public float tickRate;

    private float timer;

    public override void Execute(GameObject owner, RuntimePowerUp powerUp)
    {
        timer -= Time.deltaTime;

        if (timer > 0) return;

        timer = tickRate;

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