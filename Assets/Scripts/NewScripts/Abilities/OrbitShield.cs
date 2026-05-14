using UnityEngine;
using System.Collections.Generic;

public class OrbitShield : MonoBehaviour
{
    private float damage;

    [Header("Hit Cooldown")]
    public float enemyHitCooldown = 0.5f;

    [Header("Projectile Audio")]
    public AudioClip blockSound;

    [Range(0f, 1f)]
    public float blockVolume = 1f;

    private Dictionary<GameObject, float> hitTimers =
        new Dictionary<GameObject, float>();

    public void Init(float damage)
    {
        this.damage = damage;
    }

    void Update()
    {
        List<GameObject> keys =
            new List<GameObject>(hitTimers.Keys);

        foreach (var key in keys)
        {
            hitTimers[key] -= Time.deltaTime;

            if (hitTimers[key] <= 0f)
            {
                hitTimers.Remove(key);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ENEMIGOS
        if (other.CompareTag("Enemy"))
        {
            if (hitTimers.ContainsKey(other.gameObject))
                return;

            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage((int)damage);

                hitTimers.Add(
                    other.gameObject,
                    enemyHitCooldown
                );
            }
        }

        // PROYECTILES ENEMIGOS
        if (other.CompareTag("enemyProjectile"))
        {
            if (blockSound != null)
            {
                AudioSource.PlayClipAtPoint(
                    blockSound,
                    transform.position,
                    blockVolume
                );
            }

            Destroy(other.gameObject);
        }
    }
}