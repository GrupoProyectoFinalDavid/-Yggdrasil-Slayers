using System.Collections.Generic;

public class PowerUpManager
{
    private List<PowerUpCombination> combinations = new List<PowerUpCombination>()
    {
        new PowerUpCombination
        {
            recipe = new List<PowerUpType>
            {
                PowerUpType.Speed,
                PowerUpType.DoubleJump
            },
            result = PowerUpType.AirDash
        }
    };

    public bool TryCombine(List<PowerUpType> current)
    {
        foreach (var combo in combinations)
        {
            if (combo.Matches(current))
            {
                // eliminar los requeridos
                foreach (var r in combo.recipe)
                {
                    current.Remove(r);
                }

                // añadir resultado
                current.Add(combo.result);

                return true;
            }
        }

        return false;
    }
}