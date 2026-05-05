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
        switch (data.passiveType)
        {
            case PowerUpData.PassiveType.Damage:
                ApplyDamagePassive(stats);
                break;

            case PowerUpData.PassiveType.MaxLife:
                ApplyLifeBoostPassive(stats);
                break;

            case PowerUpData.PassiveType.AttackSpeed:
                ApplyAttackSpeedPassive(stats);
                break;

            case PowerUpData.PassiveType.MovementSpeed:
                ApplyMovementSpeedPassive(stats);
                break;

            case PowerUpData.PassiveType.Area:
                ApplyAreaPassive(stats);
                break;

            case PowerUpData.PassiveType.Cooldown:
                ApplyCooldownPassive(stats);
                break;

            case PowerUpData.PassiveType.Duration:
                ApplyDurationPassive(stats);
                break;
        }
    }

    void ApplyDamagePassive(PlayerStats stats)
    {
        if (data.damagePerLevel == null || data.damagePerLevel.Length == 0)
            return;

        float value = data.damagePerLevel[level - 1];

        stats.damageMultiplier += value;

        Debug.Log($"Damage passive aplicada: +{value} | Total: {stats.damageMultiplier}");
    }

    void ApplyLifeBoostPassive(PlayerStats stats)
    {
        if (data.lifeBoostPerLevel == null || data.lifeBoostPerLevel.Length == 0)
            return;

        float value = data.lifeBoostPerLevel[level - 1];

        // convertimos a multiplicador porcentual
        float multiplierIncrease = value;

        stats.healthMultiplier += multiplierIncrease;

        // recalcular vida máxima basada en multiplicador
        stats.maxHealth = 100f * stats.healthMultiplier;

        // opcional: ajustar vida actual proporcionalmente
        stats.currentHealth = stats.maxHealth;

        Debug.Log($"Health Boost: +{multiplierIncrease * 100f}% | Total multiplier: {stats.healthMultiplier}");
    }

    void ApplyAttackSpeedPassive(PlayerStats stats)
    {
        stats.attackSpeedMultiplier += 0.1f;
    }

    void ApplyMovementSpeedPassive(PlayerStats stats)
    {
        stats.movementSpeed += 0.5f;
    }

    void ApplyAreaPassive(PlayerStats stats)
    {
        stats.areaMultiplier += 0.1f;
    }

    void ApplyCooldownPassive(PlayerStats stats)
    {
        stats.cooldownMultiplier -= 0.1f;
    }

    void ApplyDurationPassive(PlayerStats stats)
    {
        stats.effectDurationMultiplier += 0.1f;
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

    public int GetProjectileCount()
    {
        if (data.projectileCountPerLevel == null || data.projectileCountPerLevel.Length == 0)
            return 1;

        return data.projectileCountPerLevel[level - 1];
    }
}