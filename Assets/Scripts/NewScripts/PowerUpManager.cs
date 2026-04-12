using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    private List<RuntimePowerUp> activePowerUps = new List<RuntimePowerUp>();

    private PlayerStats stats;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        foreach (var powerUp in activePowerUps)
        {
            powerUp.Update();
        }
    }

    public void AddPowerUp(PowerUpData data)
    {
        RuntimePowerUp existing = activePowerUps.Find(p => p.data == data);

        if (existing != null)
        {
            existing.LevelUp();
        }
        else
        {
            RuntimePowerUp newPU = new RuntimePowerUp(data, gameObject);
            activePowerUps.Add(newPU);

            if (data.isPassive)
                newPU.ApplyPassive(stats);
        }
    }
}