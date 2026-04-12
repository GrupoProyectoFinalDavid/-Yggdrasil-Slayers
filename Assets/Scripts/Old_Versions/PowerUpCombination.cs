using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class PowerUpCombination
{
    public List<PowerUpType> recipe; // Los que se necesitan
    public PowerUpType result;         // El resultado

    public bool Matches(List<PowerUpType> current)
    {
        return recipe.All(r => current.Contains(r));
    }
}