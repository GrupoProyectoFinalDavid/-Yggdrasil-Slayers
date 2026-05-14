using UnityEngine;
using System.Collections;


public class SurvivalEvent : RoomEvent
{

    [Header("Supervivencia")]
    public float survivalDuration = 180f;

    [Header("Prefabs de enemigos")]
    public GameObject[] enemyPrefabs;

    [Header("Referencia al player")]
    public Transform player;

    [Header("Spawn alrededor del jugador")]
    public Vector2 spawnAreaSize         = new Vector2(20f, 20f);
    public float   minDistanceFromPlayer = 5f;
    public int     maxSpawnAttempts      = 30;
    public float   spawnInterval         = 4f;

    [Tooltip("Máximo de enemigos vivos simultáneamente (0 = sin límite)")]
    public int maxEnemiesAlive = 10;

    public float TimeRemaining { get; private set; }
    public int   EnemiesAlive  { get; private set; }
    
    protected override void OnEventStart()
    {
        // Coge el player de GameManager si no está asignado
        if (player == null && GameManager.Instance != null)
            player = GameManager.Instance.Player;

        if (player == null)
            Debug.LogError("[SurvivalEvent] No hay referencia al player.");

        TimeRemaining = survivalDuration;
        EnemiesAlive  = 0;
        StartCoroutine(SurvivalTimer());
        StartCoroutine(SpawnLoop());
    }

    protected override void OnEventStop()
    {
        StopAllCoroutines();
    }
    
    private IEnumerator SurvivalTimer()
    {
        while (TimeRemaining > 0f)
        {
            yield return null;
            TimeRemaining -= Time.deltaTime;
        }

        TimeRemaining = 0f;
        Debug.Log("[SurvivalEvent] ¡Tiempo superado!");
        CompleteEvent();
    }

    private IEnumerator SpawnLoop()
    {
        while (IsRunning)
        {
            EnemiesAlive = CountAliveEnemies();

            bool underLimit = maxEnemiesAlive <= 0 || EnemiesAlive < maxEnemiesAlive;
            if (underLimit)
                SpawnRandomEnemy();

            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    private void SpawnRandomEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        if (!TryGetSpawnPosition(out Vector3 spawnPos))
        {
            Debug.LogWarning("[SurvivalEvent] No se encontró posición válida de spawn.");
            return;
        }

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        if (prefab == null) return;

        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null && player != null)
            enemyScript.player = player;

        EnemiesAlive++;
    }

    private bool TryGetSpawnPosition(out Vector3 result)
    {
        if (player == null) { result = Vector3.zero; return false; }

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            float x = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
            float y = Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f);

            Vector3 candidate = player.position + new Vector3(x, y, 0f);

            if (Vector3.Distance(candidate, player.position) >= minDistanceFromPlayer)
            {
                result = candidate;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }

    private int CountAliveEnemies()
    {
        return FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(player.position,
            new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0.1f));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
    }
}