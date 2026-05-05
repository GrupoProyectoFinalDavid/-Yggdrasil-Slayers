using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats base")]
    [Header("Vida")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float armor = 10f;
    public float movementSpeed = 5f;
    public float luck = 1f;
    public float greed = 1f;
    
    
    [Header("Multiplicadores")]
    public float healthMultiplier = 1f;
    public float damageMultiplier = 1f;
    public float attackSpeedMultiplier = 1f;
    public float areaMultiplier = 1f;
    public float cooldownMultiplier = 1f;
    public float effectDurationMultiplier = 1f;

    void Start()
    {
        currentHealth = maxHealth;
    }
    public void ResetStats()
    {

        healthMultiplier = 1f;
        damageMultiplier = 1f;
        attackSpeedMultiplier = 1f;
        areaMultiplier = 1f;
        cooldownMultiplier = 1f;
        effectDurationMultiplier = 1f;
        currentHealth = maxHealth;
    }
}