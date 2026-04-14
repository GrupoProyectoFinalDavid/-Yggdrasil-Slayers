using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    private PowerUpManager powerUpManager = new PowerUpManager();

    [Header("Movimiento")]
    public float speed = 5f;

    [Header("Vida")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI")]
    public HealthBar healthBar;

    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;

    private List<PowerUpType> powerUps = new List<PowerUpType>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        movement = new Vector2(moveX, moveY).normalized;

        if (animator != null)
            animator.SetFloat("speed", movement.sqrMagnitude);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    public void TakeDamage(int amount)
    {

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        Debug.Log("Vida actual: " + currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("El jugador ha muerto");
        gameObject.SetActive(false);
    }

    public void AddPowerUp(PowerUpType type)
    {
        if (!powerUps.Contains(type))
        {
            powerUps.Add(type);

            bool combined;
            do
            {
                combined = powerUpManager.TryCombine(powerUps);
            }
            while (combined);
        }
    }
}