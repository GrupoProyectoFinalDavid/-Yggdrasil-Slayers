using UnityEngine;

public class Enemy : MonoBehaviour
{
    private PowerUpManager powerUpManager;

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

    [Header("Damage Popup")]
    public GameObject damagePopupPrefab;
    public Vector3 popupOffset = new Vector3(0, 1f, 0);

    public System.Action OnDeath;

    void Start()
    {
        currentHealth = maxHealth;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
            powerUpManager = playerObj.GetComponent<PowerUpManager>();
        }

        int playerLevel = GetPlayerLevelSafe();

        maxHealth = Mathf.Max(1, Mathf.RoundToInt(maxHealth * (1f + (playerLevel - 1) * 0.25f)));
        currentHealth = maxHealth;
    }

    void Update()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;

        // Movimiento
        transform.position += direction * speed * Time.deltaTime;

        // Rotación en función del eje X (izquierda/derecha)
        if (direction.x < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if (direction.x > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        ShowDamagePopup(amount);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        OnDeath?.Invoke();
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

    void ShowDamagePopup(int amount)
    {
        if (damagePopupPrefab == null) return;

        Vector3 pos = transform.position + popupOffset;

        GameObject popup = Instantiate(damagePopupPrefab, pos, Quaternion.identity);

        DamagePopup dp = popup.GetComponent<DamagePopup>();

        if (dp != null)
        {
            dp.Setup(amount, transform, popupOffset);
        }
    }

    int GetPlayerLevelSafe()
    {
        if (powerUpManager == null)
            return 1;

        return powerUpManager.GetPlayerLevel();
    }
}