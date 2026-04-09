using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Prefabs de enemigos")]
    public GameObject[] enemyPrefabs;

    [Header("Zona de spawn")]
    public Vector2 spawnAreaSize = new Vector2(20f, 20f);
    public Vector3 spawnAreaCenter = Vector3.zero;
    public int TotalWaves => totalWaves;
    [Header("Restricción respecto al player")]
    public float minDistanceFromPlayer = 5f;

    [Header("Control de spawn")]
    public int maxSpawnAttempts = 30;
    public float spawnInterval = 3f; // Segundos entre cada spawn dentro de la oleada

    [Header("Oleadas")]
    public int totalWaves = 3;
    public float waveDuration = 60f;

    // --- Estado interno de oleadas ---
    private int currentWave = 0;
    private float waveTimer = 0f;
    private float spawnTimer = 0f;
    private bool waveActive = false;
    private bool allWavesFinished = false;

    void Update()
    {
        // Solo SPACE para arrancar la oleada 1
        if (Input.GetKeyDown(KeyCode.Space) && currentWave == 0 && !allWavesFinished)
        {
            StartNextWave();
        }

        if (waveActive)
        {
            HandleWaveTimer();
            HandleSpawnTimer();
        }
    }

    // ──────────────────────────────────────────
    //  OLEADAS
    // ──────────────────────────────────────────

    void StartNextWave()
    {
        if (currentWave >= totalWaves)
        {
            allWavesFinished = true;
            Debug.Log("¡Todas las oleadas han terminado!");
            return;
        }

        currentWave++;
        waveTimer = 0f;
        spawnTimer = 0f;
        waveActive = true;

        Debug.Log($"── Oleada {currentWave} / {totalWaves} iniciada ──");
    }

    void HandleWaveTimer()
    {
        waveTimer += Time.deltaTime;

        if (waveTimer >= waveDuration)
        {
            EndCurrentWave();
        }
    }

    void EndCurrentWave()
    {
        waveActive = false;
        Debug.Log($"── Oleada {currentWave} terminada ──");

        if (currentWave < totalWaves)
        {
            Invoke(nameof(StartNextWave), 3f); // 3 segundos de pausa entre oleadas
        }
        else
        {
            allWavesFinished = true;
            Debug.Log("¡Has sobrevivido todas las oleadas!");
        }
    }
    // ──────────────────────────────────────────
    //  SPAWN PERIÓDICO DURANTE LA OLEADA
    // ──────────────────────────────────────────

    void HandleSpawnTimer()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnRandomEnemy();
        }
    }

    void SpawnRandomEnemy()
    {
        if (player == null)
        {
            Debug.LogWarning("No hay player asignado en el GameManager.");
            return;
        }

        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("No hay prefabs de enemigos asignados.");
            return;
        }

        Vector3 spawnPosition;
        bool validPositionFound = TryGetValidSpawnPosition(out spawnPosition);

        if (!validPositionFound)
        {
            Debug.LogWarning("No se encontró una posición válida para spawnear.");
            return;
        }

        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject newEnemy = Instantiate(enemyPrefabs[randomEnemyIndex], spawnPosition, Quaternion.identity);

        Enemy enemyScript = newEnemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.player = player;
        }
        else
        {
            Debug.LogWarning("El enemigo instanciado no tiene script Enemy.");
        }
    }

    // ──────────────────────────────────────────
    //  POSICIÓN DE SPAWN VÁLIDA
    // ──────────────────────────────────────────

    bool TryGetValidSpawnPosition(out Vector3 validPosition)
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            float randomX = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
            float randomY = Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f);

            Vector3 candidatePosition = new Vector3(
                spawnAreaCenter.x + randomX,
                spawnAreaCenter.y + randomY,
                spawnAreaCenter.z
            );

            if (Vector3.Distance(candidatePosition, player.position) >= minDistanceFromPlayer)
            {
                validPosition = candidatePosition;
                return true;
            }
        }

        validPosition = Vector3.zero;
        return false;
    }

    // ──────────────────────────────────────────
    //  GIZMOS
    // ──────────────────────────────────────────

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnAreaCenter, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0.1f));

        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
        }
    }

    // ──────────────────────────────────────────
    //  GETTERS PÚBLICOS (útiles para UI)
    // ──────────────────────────────────────────

    public int CurrentWave => currentWave;
    public float WaveTimeRemaining => Mathf.Max(0f, waveDuration - waveTimer);
    public bool IsWaveActive => waveActive;
    public bool AllWavesFinished => allWavesFinished;
}