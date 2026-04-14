using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_Text waveText;
    public TMP_Text totalTimeText;

    [Header("Referencia al WaveManager")]
    public WaveManager waveManager;

    void Update()
    {
        if (waveManager == null) return;

        // ── Número de oleada ──
        if (waveManager.IsWaveActive || waveManager.CurrentWave > 0)
            waveText.text = $"Oleada {waveManager.CurrentWave} / {waveManager.TotalWaves}";
        else
            waveText.text = "Pulsa SPACE para empezar";

        // ── Tiempo total ──
        if (waveManager.GameStarted)
        {
            int totalSeconds = Mathf.FloorToInt(waveManager.TotalGameTime);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            totalTimeText.text = $"{minutes:D2}:{seconds:D2}";
        }
        else
        {
            totalTimeText.text = "00:00";
        }
    }
}