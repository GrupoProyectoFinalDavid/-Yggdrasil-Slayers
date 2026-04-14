using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Oleadas")]
    public int totalWaves = 3;
    public float waveDuration = 60f;

    private int currentWave = 0;
    private float waveTimer = 0f;
    private bool waveActive = false;
    private bool allWavesFinished = false;
    private float totalGameTime = 0f;
    private bool gameStarted = false;

    void Update()
    {
        if (gameStarted && !allWavesFinished)
            totalGameTime += Time.deltaTime;

        if (waveActive)
            HandleWaveTimer();
    }

    public void StartNextWave()
    {
        if (currentWave >= totalWaves)
        {
            allWavesFinished = true;
            Debug.Log("¡Todas las oleadas han terminado!");
            return;
        }

        currentWave++;
        waveTimer = 0f;
        waveActive = true;
        gameStarted = true;

        Debug.Log($"── Oleada {currentWave} / {totalWaves} iniciada ──");
    }

    void HandleWaveTimer()
    {
        waveTimer += Time.deltaTime;

        if (waveTimer >= waveDuration)
            EndCurrentWave();
    }

    void EndCurrentWave()
    {
        waveActive = false;
        Debug.Log($"── Oleada {currentWave} terminada ──");

        if (currentWave < totalWaves)
            Invoke(nameof(StartNextWave), 3f);
        else
        {
            allWavesFinished = true;
            Debug.Log("¡Has sobrevivido todas las oleadas!");
        }
    }

    public int CurrentWave => currentWave;
    public int TotalWaves => totalWaves;
    public float WaveTimeRemaining => Mathf.Max(0f, waveDuration - waveTimer);
    public bool IsWaveActive => waveActive;
    public bool AllWavesFinished => allWavesFinished;
    public float TotalGameTime => totalGameTime;
    public bool GameStarted => gameStarted;
}