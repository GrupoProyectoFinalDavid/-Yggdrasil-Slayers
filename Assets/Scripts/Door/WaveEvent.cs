using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveEvent : RoomEvent
{
    [System.Serializable]
    public class EnemyWave
    {
        public GameObject[] enemyPrefabs;
        public float delayBeforeNextWave = 3f;
    }
    
    [Header("Oleadas")]
    public EnemyWave[] waves;

    [Header("Referencia al player")]
    public Transform player;

    [Header("Spawn alrededor del jugador")]
    public Vector2 spawnAreaSize         = new Vector2(20f, 20f);
    public float   minDistanceFromPlayer = 5f;
    public int     maxSpawnAttempts      = 30;

    public int  CurrentWaveIndex   { get; private set; }
    public int  TotalWaves         => waves != null ? waves.Length : 0;
    public int  EnemiesAlive       { get; private set; }
    public bool WaitingForNextWave { get; private set; }
    
    private List<GameObject> _aliveEnemies = new List<GameObject>();
    
    protected override void OnEventStart()
    {
        // Coge el player de GameManager si no está asignado
        if (player == null && GameManager.Instance != null)
            player = GameManager.Instance.Player;

        if (player == null)
            Debug.LogError("[WaveEvent] No hay referencia al player.");

        CurrentWaveIndex = 0;
        StartCoroutine(RunWaves());
    }

    protected override void OnEventStop()
    {
        StopAllCoroutines();
        DestroyAliveEnemies();
    }
    
    private IEnumerator RunWaves()
    {
        for (int i = 0; i < waves.Length; i++)
        {
            CurrentWaveIndex   = i + 1;
            WaitingForNextWave = false;

            Debug.Log($"[WaveEvent] Iniciando oleada {CurrentWaveIndex} / {TotalWaves}");
            SpawnWave(waves[i]);

            yield return new WaitUntil(() => AllEnemiesDead());

            Debug.Log($"[WaveEvent] Oleada {CurrentWaveIndex} limpiada.");

            if (i < waves.Length - 1)
            {
                WaitingForNextWave = true;
                yield return new WaitForSeconds(waves[i].delayBeforeNextWave);
            }
        }

        CompleteEvent();
    }

    private void SpawnWave(EnemyWave wave)
    {
        _aliveEnemies.Clear();
        EnemiesAlive = 0;

        if (wave.enemyPrefabs == null || wave.enemyPrefabs.Length == 0)
        {
            Debug.LogWarning($"[WaveEvent] La oleada {CurrentWaveIndex} no tiene prefabs de enemigos.");
            return;
        }

        foreach (GameObject prefab in wave.enemyPrefabs)
        {
            if (prefab == null) continue;

            if (!TryGetSpawnPosition(out Vector3 spawnPos))
            {
                Debug.LogWarning("[WaveEvent] No se encontró posición válida de spawn.");
                continue;
            }

            GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null && player != null)
                enemyScript.player = player;

            _aliveEnemies.Add(enemy);
            EnemiesAlive++;
        }
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
    
    private bool AllEnemiesDead()
    {
        _aliveEnemies.RemoveAll(e => e == null);
        EnemiesAlive = _aliveEnemies.Count;
        return EnemiesAlive == 0;
    }

    private void DestroyAliveEnemies()
    {
        foreach (var e in _aliveEnemies)
            if (e != null) Destroy(e);
        _aliveEnemies.Clear();
        EnemiesAlive = 0;
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