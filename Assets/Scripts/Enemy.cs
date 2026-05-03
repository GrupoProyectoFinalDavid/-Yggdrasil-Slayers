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

    [Header("Rotación / Bamboleo")]
    public float wobbleSpeed = 8f;       // Velocidad de la oscilación
    public float wobbleAmount = 12f;     // Grados máximos de inclinación
    public bool wobbleWhileIdle = false; // ¿Bambolear también al estar quieto?

    // Estado interno
    private bool isFacingRight = true;
    private bool isMoving = false;
    private float wobbleTime = 0f;
    private float currentWobbleAngle = 0f;

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

        ApplyWobble();
    }

    // ─── Bamboleo ───────────────────────────────────────────────────────────

    void ApplyWobble()
    {
        if (!isMoving && !wobbleWhileIdle)
        {
            // Vuelve suavemente a 0 cuando está quieto
            currentWobbleAngle = Mathf.Lerp(currentWobbleAngle, 0f, Time.deltaTime * wobbleSpeed);
        }
        else
        {
            wobbleTime += Time.deltaTime * wobbleSpeed;
            float sineValue = Mathf.Sin(wobbleTime);
            currentWobbleAngle = sineValue * wobbleAmount;
        }

        // Mantiene el flip horizontal y aplica el bamboleo en Z
        float yRotation = isFacingRight ? 0f : 180f;
        transform.rotation = Quaternion.Euler(0f, yRotation, currentWobbleAngle);
    }

    // ─── Comportamiento ──────────────────────────────────────────────────────

    void RangedBehaviour()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            MoveTowardsPlayer();
        }
        else
        {
            isMoving = false;
            LookAtPlayer();
            Shoot();
        }
    }

    void FollowPlayer()
    {
        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        isMoving = true;
        LookAtPlayer();
    }

    void LookAtPlayer()
    {
        // Solo actualiza el flag; la rotación real la aplica ApplyWobble()
        if (player.position.x < transform.position.x)
            isFacingRight = false;
        else if (player.position.x > transform.position.x)
            isFacingRight = true;
    }

    // ─── Disparo ─────────────────────────────────────────────────────────────

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

    // ─── Daño / Muerte ───────────────────────────────────────────────────────

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