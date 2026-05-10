using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/PowerUpData")]
public class PowerUpData : ScriptableObject
{
    public string powerUpName;
    public bool isPassive;

    public enum PassiveType
    {
        None,
        Damage,
        MaxLife,
        AttackSpeed,
        MovementSpeed,
        Area,
        Cooldown,
        Duration
    }

    public PassiveType passiveType;

    public float[] damagePerLevel;
    public float[] lifeBoostPerLevel;
    public int[] projectileCountPerLevel;
    public float[] cooldownPerLevel;
    public float[] tickRatePerLevel;
    public float[] durationPerLevel;
    public float[] radiusPerLevel;
    public int maxLevel = 4;

    public AbilityBehaviour[] abilities;
}