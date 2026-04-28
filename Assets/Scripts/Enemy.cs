using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform player;

    [Header("Animacion")]
    private Animator animator;

    [Header("Movimiento")]
    public float speed = 3f;

    [Header("Tipo de enemigo")]
    public bool isRangedEnemy = false;
    public float stopDistance = 6f;

    [Header("Disparo ranged")]
    public GameObject enemyProjectilePrefab;
    public Transform shootPoint;
    public float shootCooldown = 1.5f;
    public int projectileDamage = 20;
    public float projectileSpeed = 6f;
    private float lastShootTime;

    [Header("Vida")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Daño por contacto")]
    public int damage = 10;
    public float attackCooldown = 1f;
    private float lastAttackTime;

    private void Start()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();

        if (animator != null)
            animator.SetFloat("speed", speed);

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogWarning("[Enemy] No se encontró ningún objeto con tag 'Player'.");
        }
    }

    void Update()
    {
        if (player == null) return;

        if (isRangedEnemy)
            RangedBehaviour();
        else
            FollowPlayer();
    }

    void RangedBehaviour()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            FollowPlayer();
        }
        else
        {
            LookAtPlayer();
            Shoot();
        }
    }

    void FollowPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;

        transform.position += direction * speed * Time.deltaTime;

        LookAtPlayer();
    }

    void LookAtPlayer()
    {
        if (player.position.x < transform.position.x)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        else if (player.position.x > transform.position.x)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void Shoot()
    {
        if (enemyProjectilePrefab == null || shootPoint == null) return;

        if (Time.time < lastShootTime + shootCooldown) return;

        lastShootTime = Time.time;

        GameObject projectile = Instantiate(
            enemyProjectilePrefab,
            shootPoint.position,
            Quaternion.identity
        );

        EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();

        if (projectileScript != null)
        {
            Vector2 direction = (player.position - shootPoint.position).normalized;
            projectileScript.Init(direction, projectileSpeed, projectileDamage);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
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