using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Referencias")]
    public WaveManager waveManager;
    public SpawnManager spawnManager;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !waveManager.GameStarted && !waveManager.AllWavesFinished)
        {
            waveManager.StartNextWave();
        }
    }
}