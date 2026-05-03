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

        // ── Tiempo total de partida ───────────────────────
        int totalSeconds = Mathf.FloorToInt(gameManager.TotalGameTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        totalTimeText.text = $"{minutes:D2}:{seconds:D2}";

        // ── Detectar evento activo ────────────────────────
        WaveEvent     activeWave     = gameManager.GetActiveWaveEvent();
        SurvivalEvent activeSurvival = gameManager.GetActiveSurvivalEvent();

        if (activeWave != null)
        {
            // Modo oleadas
            waveText.text = $"Oleada {activeWave.CurrentWaveIndex} / {activeWave.TotalWaves}";

            int enemies = activeWave.EnemiesAlive;
            timerText.text  = $"Enemigos: {enemies}";
            timerText.color = enemies > 0 ? Color.red : Color.green;

            statusText.text  = activeWave.WaitingForNextWave ? "Siguiente oleada en breve..." : "";
            statusText.color = Color.white;
        }
        else if (activeSurvival != null)
        {
            // Modo supervivencia
            waveText.text = "¡Modo Supervivencia!";

            float t = activeSurvival.TimeRemaining;
            int m   = Mathf.FloorToInt(t) / 60;
            int s   = Mathf.FloorToInt(t) % 60;
            timerText.text  = $"{m:D2}:{s:D2}";
            timerText.color = t <= 30f ? Color.red : Color.white;

            statusText.text  = $"Enemigos vivos: {activeSurvival.EnemiesAlive}";
            statusText.color = Color.white;
        }
        else if (gameManager.AllRoomsCleared)
        {
            // Victoria
            waveText.text   = "¡Has sobrevivido!";
            timerText.text  = "--";
            timerText.color = Color.white;
            statusText.text  = "Todas las salas limpias";
            statusText.color = Color.yellow;
        }
        else
        {
            // Sin evento activo
            waveText.text   = "Entra en una sala para empezar";
            timerText.text  = "--";
            timerText.color = Color.white;
            statusText.text  = "";
        }
    }
}