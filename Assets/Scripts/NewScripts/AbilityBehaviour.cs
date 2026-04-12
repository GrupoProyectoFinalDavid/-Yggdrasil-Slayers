using UnityEngine;

public abstract class AbilityBehaviour : ScriptableObject
{
    public abstract void Execute(GameObject owner, RuntimePowerUp powerUp);
}