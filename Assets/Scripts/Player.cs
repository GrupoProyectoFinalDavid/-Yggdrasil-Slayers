using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    private PowerUpManager powerUpManager;
    
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
        // Movimiento
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

    // Método para añadir power-ups
    public void AddPowerUp(PowerUpData data)
    {
        powerUpManager.AddPowerUp(data);
    }
    
    public Coroutine Teleport(Vector2 pos)
    {
        return StartCoroutine(TeleportRoutine(pos));
    }

    private IEnumerator TeleportRoutine(Vector2 pos)
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        yield return new WaitForFixedUpdate();
        rb.position = pos;
        transform.position = pos;

        yield return new WaitForFixedUpdate();
    }
    
}