using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class PlayerSpawn : MonoBehaviour
{
    [Header("Punto de spawn")]
    public Transform spawnPoint;

    [Header("Cinemachine")]
    public CinemachineCamera virtualCamera; // ← CinemachineCamera, no CinemachineVirtualCamera

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        GameObject prefab = SceneLoader.Instance != null
            ? SceneLoader.Instance.SelectedCharacterPrefab
            : null;

        if (prefab == null)
        {
            Debug.LogError("[PlayerSpawner] No hay prefab de personaje seleccionado.");
            return;
        }

        Vector3    pos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        GameObject player = Instantiate(prefab, pos, rot);
        player.SetActive(true);

        if (GameManager.Instance != null)
            GameManager.Instance.RegisterPlayer(player.transform);
        else
            Debug.LogWarning("[PlayerSpawner] GameManager no encontrado.");

        StartCoroutine(AssignCameraNextFrame(player.transform));
    }

    private IEnumerator AssignCameraNextFrame(Transform target)
    {
        yield return null;

        if (virtualCamera != null)
        {
            virtualCamera.Follow = target;
            Debug.Log("[PlayerSpawner] Cinemachine asignado al jugador.");
        }

        // Asigna el player a todos los eventos activos
        foreach (var waveEvent in FindObjectsByType<WaveEvent>(FindObjectsSortMode.None))
            waveEvent.player = target;

        foreach (var survivalEvent in FindObjectsByType<SurvivalEvent>(FindObjectsSortMode.None))
            survivalEvent.player = target;

        Debug.Log("[PlayerSpawner] Player asignado a todos los eventos.");
    }
}