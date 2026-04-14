using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Prefabs de enemigos")]
    public GameObject[] enemyPrefabs;

    [Header("Zona de spawn")]
    public Vector2 spawnAreaSize = new Vector2(20f, 20f);
    public Vector3 spawnAreaCenter = Vector3.zero;

    [Header("Restricción respecto al player")]
    public float minDistanceFromPlayer = 5f;

    [Header("Control de spawn")]
    public int maxSpawnAttempts = 30;
    public float spawnInterval = 3f;

    public WaveManager waveManager;   
    private float spawnTimer = 0f;

    void Update()
    {
        if (waveManager.IsWaveActive)
            HandleSpawnTimer();
    }

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
            Debug.LogWarning("No hay player asignado en el SpawnManager.");
            return;
        }

        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("No hay prefabs de enemigos asignados.");
            return;
        }

        Vector3 spawnPosition;
        if (!TryGetValidSpawnPosition(out spawnPosition))
        {
            Debug.LogWarning("No se encontró una posición válida para spawnear.");
            return;
        }

        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject newEnemy = Instantiate(enemyPrefabs[randomIndex], spawnPosition, Quaternion.identity);

        Enemy enemyScript = newEnemy.GetComponent<Enemy>();
        if (enemyScript != null)
            enemyScript.player = player;
        else
            Debug.LogWarning("El enemigo instanciado no tiene script Enemy.");
    }

    bool TryGetValidSpawnPosition(out Vector3 validPosition)
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            float randomX = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
            float randomY = Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f);

            Vector3 candidate = new Vector3(
                spawnAreaCenter.x + randomX,
                spawnAreaCenter.y + randomY,
                spawnAreaCenter.z
            );

            if (Vector3.Distance(candidate, player.position) >= minDistanceFromPlayer)
            {
                validPosition = candidate;
                return true;
            }
        }

        validPosition = Vector3.zero;
        return false;
    }

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
}