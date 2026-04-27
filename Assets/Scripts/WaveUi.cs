using UnityEngine;
using TMPro;

public class WaveUi : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_Text waveText;
    public TMP_Text timerText;
    public TMP_Text totalTimeText;
    public TMP_Text statusText;

    [Header("Referencia al GameManager")]
    public GameManager gameManager;

    void Update()
    {
        if (gameManager == null) return;

        // ── Número de oleada ──
        if (gameManager.IsWaveActive || gameManager.CurrentWave > 0)
        {
            waveText.text = $"Oleada {gameManager.CurrentWave} / {gameManager.TotalWaves}";
        }
        else
        {
            waveText.text = "Pulsa SPACE para empezar";
        }

        // ── Tiempo restante de oleada ──
        if (gameManager.IsWaveActive)
        {
            float t = gameManager.WaveTimeRemaining;
            timerText.text = $"{Mathf.CeilToInt(t):D2} s";
            timerText.color = t <= 10f ? Color.red : Color.white;
        }
        else
        {
            timerText.text = "--";
            timerText.color = Color.white;
        }

        // ── Tiempo total de partida ──
        if (gameManager.GameStarted)
        {
            int totalSeconds = Mathf.FloorToInt(gameManager.TotalGameTime);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            totalTimeText.text = $"{minutes:D2}:{seconds:D2}";
        }
        else
        {
            totalTimeText.text = "00:00";
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