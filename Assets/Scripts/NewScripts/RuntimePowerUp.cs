using UnityEngine;

public class RuntimePowerUp
{
    public PowerUpData data;
    private int level = 0;

    private float timer;
    private GameObject owner;

    public RuntimePowerUp(PowerUpData data, GameObject owner)
    {
        this.data = data;
        this.owner = owner;
        LevelUp();
    }

    public void LevelUp()
    {
        if (level < data.maxLevel)
            level++;
    }

    public void Update()
    {
        if (data.isPassive) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Activate();
            timer = GetCooldown();
        }
    }

    private void Activate()
    {
        foreach (var ability in data.abilities)
        {
            ability.Execute(owner, this);
        }
    }

    public void ApplyPassive(PlayerStats stats)
    {
        // EJEMPLO simple
        stats.damageMultiplier += 0.1f * level;
    }

    public float GetDamage()
    {
        return data.damagePerLevel[level - 1];
    }

    public float GetCooldown()
    {
        return data.cooldownPerLevel[level - 1];
    }

    public int GetLevel()
    {
        return level;
    }
}