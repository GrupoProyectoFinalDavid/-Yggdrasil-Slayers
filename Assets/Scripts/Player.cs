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

    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;

    // Lista de power-ups
    private List<PowerUpType> powerUps = new List<PowerUpType>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Movimiento
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        movement = new Vector2(moveX, moveY).normalized;

        // Animación
        if (animator != null)
            animator.SetFloat("speed", movement.sqrMagnitude);

        // Log continuo de power-ups
        Debug.Log("PowerUps: " + string.Join(", ", powerUps));
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Vida del jugador: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("El jugador ha muerto");
        // Aquí puedes poner animación, reinicio de nivel, game over, etc.
        gameObject.SetActive(false);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    // Método para añadir power-ups
    public void AddPowerUp(PowerUpType type)
    {
        if (!powerUps.Contains(type))
        {
            powerUps.Add(type);

            // Intentar combinaciones en bucle
            bool combined;
            do
            {
                combined = powerUpManager.TryCombine(powerUps);
            }
            while (combined);
        }
    }
}