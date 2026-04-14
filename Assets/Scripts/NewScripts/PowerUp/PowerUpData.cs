using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/PowerUpData")]
public class PowerUpData : ScriptableObject
{
    public string powerUpName;
    public bool isPassive;

    public float[] damagePerLevel;
    public float[] cooldownPerLevel;
    public float[] durationPerLevel;
    public float[] radiusPerLevel;
    public int maxLevel = 5;

    public AbilityBehaviour[] abilities;
}