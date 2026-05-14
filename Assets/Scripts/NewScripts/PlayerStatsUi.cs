using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Referencia al jugador")]
    public PlayerStats playerStats;

    [Header("Stats base - Textos")]
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI movementSpeedText;
    public TextMeshProUGUI luckText;
    public TextMeshProUGUI greedText;

    [Header("Multiplicadores - Textos")]
    public TextMeshProUGUI healthMultiplierText;
    public TextMeshProUGUI damageMultiplierText;
    public TextMeshProUGUI attackSpeedMultiplierText;
    public TextMeshProUGUI areaMultiplierText;
    public TextMeshProUGUI cooldownMultiplierText;
    public TextMeshProUGUI effectDurationMultiplierText;

    [Header("Actualización")]
    [Tooltip("Si está activo, actualiza la UI cada frame. Si no, llama a RefreshUI() manualmente.")]
    public bool autoRefresh = true;

    void Update()
    {
        if (autoRefresh && playerStats != null)
            RefreshUI();
    }

    /// <summary>
    /// Llama a este método cuando quieras actualizar la UI manualmente
    /// (por ejemplo, tras aplicar un item o subir de nivel).
    /// </summary>
    public void RefreshUI()
    {
        // Stats base
        SetText(lifeText,          "Vida",           playerStats.life,                   "F0");
        SetText(armorText,         "Armadura",        playerStats.armor,                  "F0");
        SetText(movementSpeedText, "Velocidad",       playerStats.movementSpeed,           "F1");
        SetText(luckText,          "Suerte",          playerStats.luck,                   "F2");
        SetText(greedText,         "Codicia",         playerStats.greed,                  "F2");

        // Multiplicadores
        SetMultiplierText(healthMultiplierText,         "Vida x",         playerStats.healthMultiplier);
        SetMultiplierText(damageMultiplierText,         "Daño x",         playerStats.damageMultiplier);
        SetMultiplierText(attackSpeedMultiplierText,    "Vel. Ataque x",  playerStats.attackSpeedMultiplier);
        SetMultiplierText(areaMultiplierText,           "Área x",         playerStats.areaMultiplier);
        SetMultiplierText(cooldownMultiplierText,       "Cooldown x",     playerStats.cooldownMultiplier);
        SetMultiplierText(effectDurationMultiplierText, "Duración Efecto x", playerStats.effectDurationMultiplier);
    }

    /// <summary>
    /// Asigna un jugador distinto en tiempo de ejecución y refresca la UI.
    /// Útil si tienes varios jugadores y quieres cambiar cuál se muestra.
    /// </summary>
    public void SetPlayer(PlayerStats newPlayer)
    {
        playerStats = newPlayer;
        RefreshUI();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void SetText(TextMeshProUGUI label, string statName, float value, string format)
    {
        if (label != null)
            label.text = $"{statName}: {value.ToString(format)}";
    }

    private void SetMultiplierText(TextMeshProUGUI label, string statName, float value)
    {
        if (label == null) return;

        // Colorea en verde si > 1, rojo si < 1, blanco si = 1
        string color = value > 1f ? "#00FF88" : value < 1f ? "#FF4444" : "white";
        label.text = $"{statName} <color={color}>{value:F2}</color>";
    }
}