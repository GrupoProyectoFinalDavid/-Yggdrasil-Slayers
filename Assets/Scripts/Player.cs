using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    private PowerUpManager powerUpManager;
    private PlayerStats stats;


    [Header("Movimiento")]
    public float speed = 5f;

    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;

    // Lista de power-ups
    private List<PowerUpType> powerUps = new List<PowerUpType>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        stats = GetComponent<PlayerStats>();

        stats.currentHealth = stats.maxHealth;
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
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    public void TakeDamage(int amount)
    {
        stats.currentHealth -= amount;

        Debug.Log("Vida del jugador: " + stats.currentHealth);

        if (stats.currentHealth <= 0)
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
        return (int)stats.currentHealth;
    }

    // Método para añadir power-ups
    public void AddPowerUp(PowerUpData data)
    {
        powerUpManager.AddPowerUp(data);
    }
}