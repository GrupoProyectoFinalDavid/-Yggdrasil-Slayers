using UnityEngine;

public static class DamageCalculator
{
    public static float Calculate(float baseDamage, PlayerStats stats)
    {
        return baseDamage * stats.damageMultiplier;
    }
}