using UnityEngine;

public class RuntimePowerUp
{
    public PowerUpData data;
    public int level = 0;

    private float timer;
    private GameObject owner;
    private bool isAbilityRunning;

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
        
        if (isAbilityRunning)
                    return;
        
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Activate();
        }
    }

    private void Activate()
    {
        isAbilityRunning = true;

        foreach (var ability in data.abilities)
        {
            ability.Execute(owner, this);
        }
    }

    public void FinishAbility()
    {
        isAbilityRunning = false;
        timer = GetCooldown();
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
        int index = Mathf.Clamp(
            level - 1,
            0,
            data.damagePerLevel.Length - 1
        );

        return data.damagePerLevel[index];
    }

    public float GetCooldown()
    {
        int index = Mathf.Clamp(
            level - 1,
            0,
            data.cooldownPerLevel.Length - 1
        );

        return data.cooldownPerLevel[index];
    }

    public int GetLevel()
    {
        return level;
    }

    public int GetProjectileCount()
    {
        if (data.projectileCountPerLevel == null ||
            data.projectileCountPerLevel.Length == 0)
            return 1;

        int index = Mathf.Clamp(
            level - 1,
            0,
            data.projectileCountPerLevel.Length - 1
        );

        return data.projectileCountPerLevel[index];
    }

    public float GetDuration()
    {
        if (data.durationPerLevel == null ||
            data.durationPerLevel.Length == 0)
            return 1f;

        int index = Mathf.Clamp(
            level - 1,
            0,
            data.durationPerLevel.Length - 1
        );

        return data.durationPerLevel[index];
    }

    public float GetRadius()
    {
        if (data.radiusPerLevel == null ||
            data.radiusPerLevel.Length == 0)
            return 1f;

        int index = Mathf.Clamp(
            level - 1,
            0,
            data.radiusPerLevel.Length - 1
        );

        return data.radiusPerLevel[index];
    }

    public float GetTickRate()
    {
        if (data.tickRatePerLevel == null ||
            data.tickRatePerLevel.Length == 0)
            return 1f;

        int index = Mathf.Clamp(
            level - 1,
            0,
            data.tickRatePerLevel.Length - 1
        );

        return data.tickRatePerLevel[index];
    }
}