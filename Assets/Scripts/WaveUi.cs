using UnityEngine;
using TMPro; // Si usas Text normal cambia TMP_Text por Text y el using por UnityEngine.UI

public class WaveUi : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_Text waveText;
    public TMP_Text timerText;
    public TMP_Text statusText; // Mensaje de "Pulsa SPACE" o "¡Victoria!"

    [Header("Referencia al GameManager")]
    public GameManager gameManager;

    void Update()
    {
        if (gameManager == null) return;

        // ── Número de oleada ──
        if (gameManager.IsWaveActive || gameManager.CurrentWave > 0)
        {
            waveText.text = $"Oleada  {gameManager.CurrentWave} / {gameManager.TotalWaves}";
        }
        else
        {
            waveText.text = "Pulsa SPACE para empezar";
        }

        // ── Tiempo restante ──
        if (gameManager.IsWaveActive)
        {
            float t = gameManager.WaveTimeRemaining;
            timerText.text = $"{Mathf.CeilToInt(t):D2} s";

            // Cambia a rojo cuando queden menos de 10 segundos
            timerText.color = t <= 10f ? Color.red : Color.white;
        }
        else
        {
            timerText.text = "--";
            timerText.color = Color.white;
        }

        // ── Mensaje de estado ──
        if (gameManager.AllWavesFinished)
        {
            statusText.text = "¡Has sobrevivido!";
            statusText.color = Color.yellow;
        }
        else if (!gameManager.IsWaveActive && gameManager.CurrentWave > 0)
        {
            statusText.text = "Siguiente oleada en breve...";
            statusText.color = Color.white;
        }
        else
        {
            statusText.text = "";
        }
    }
}