using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform player;

    [Header("Animacion")]
    private Animator animator;

    [Header("Movimiento")]
    public float speed = 3f;

    [Header("Vida")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Daño")]
    public int damage = 10;
    public float attackCooldown = 1f; // tiempo entre golpes mientras toca al jugador
    private float lastAttackTime;

    private void Start()
    {
        currentHealth = maxHealth;

        // Solo busca si GameManager no asignó ya el player
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogWarning("[Enemy] No se encontró ningún objeto con tag 'Player'.");
        }
        else
        {
            // Animación
            animator = GetComponent<Animator>();
            animator.SetFloat("speed", speed);
        }
    }

    void Update()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Player playerScript = collision.gameObject.GetComponent<Player>();

                if (playerScript != null)
                {
                    playerScript.TakeDamage(damage);
                    lastAttackTime = Time.time;
                }
            }
        }
    }
}