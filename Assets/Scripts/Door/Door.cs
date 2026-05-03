using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    [Header("Conexión entre salas")]
    public Door connectedDoor;           // La puerta destino en la otra sala
    public Room ownerRoom;               // La sala a la que pertenece esta puerta

    [Header("Spawn")]
    public Transform spawnPoint;         // Punto donde aparece el jugador al cruzar

    [Header("Visual de bloqueo")]
    [Tooltip("Collider que bloquea el paso físico cuando está cerrada")]
    public Collider2D blockingCollider;

    [Tooltip("Objeto visual cuando la puerta está BLOQUEADA (rejas, candado...)")]
    public GameObject lockedVisual;

    [Tooltip("Objeto visual cuando la puerta está ABIERTA")]
    public GameObject unlockedVisual;

    [Header("Estado inicial")]
    public bool startsLocked = false;

    // ── Estado ───────────────────────────────────────────────
    public bool IsLocked { get; private set; }

    private bool _isOnCooldown = false;

    // ────────────────────────────────────────────────────────
    //  Unity
    // ────────────────────────────────────────────────────────
    private void Awake()
    {
        if (startsLocked)
            Lock();
        else
            Unlock();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isOnCooldown)               return;
        if (IsLocked)                    return;  // bloqueada: no deja pasar
        if (!other.CompareTag("Player")) return;
        if (connectedDoor == null)       return;

        Debug.Log($"[Door] SpawnPoint de destino: {connectedDoor.spawnPoint.position}");
        Debug.Log($"[Door] ConnectedDoor: {connectedDoor.name}");

        RoomManager.Instance.TransitionThroughDoor(
            other.GetComponent<Player>(), this, connectedDoor);
    }

    // ────────────────────────────────────────────────────────
    //  Lock / Unlock  (llamados desde Room.cs)
    // ────────────────────────────────────────────────────────
    public void Lock()
    {
        IsLocked = true;

        if (blockingCollider != null) blockingCollider.enabled = true;
        if (lockedVisual     != null) lockedVisual.SetActive(true);
        if (unlockedVisual   != null) unlockedVisual.SetActive(false);

        Debug.Log($"[Door] '{name}' BLOQUEADA.");
    }

    public void Unlock()
    {
        IsLocked = false;

        if (blockingCollider != null) blockingCollider.enabled = false;
        if (lockedVisual     != null) lockedVisual.SetActive(false);
        if (unlockedVisual   != null) unlockedVisual.SetActive(true);

        Debug.Log($"[Door] '{name}' DESBLOQUEADA.");
    }

    // ────────────────────────────────────────────────────────
    //  Cooldown  (evita re-trigger inmediato al cruzar)
    // ────────────────────────────────────────────────────────
    public void SetCooldown(float duration)
    {
        StartCoroutine(CooldownRoutine(duration));
    }

    private IEnumerator CooldownRoutine(float duration)
    {
        _isOnCooldown = true;
        yield return new WaitForSeconds(duration);
        _isOnCooldown = false;
    }
}