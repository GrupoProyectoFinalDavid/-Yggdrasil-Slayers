using UnityEngine;

public class FireArrowRainArea : MonoBehaviour
{
    private RuntimePowerUp powerUp;
    private LayerMask enemyLayer;

    private float duration;
    private float tickRate;
    private float radius;

    private float timer;
    private float tickTimer;

    private AudioSource audioSource;

    public void Init(
        RuntimePowerUp powerUp,
        LayerMask enemyLayer,
        AudioClip loopSound,
        float loopVolume
    )
    {
        this.powerUp = powerUp;
        this.enemyLayer = enemyLayer;

        duration = powerUp.GetDuration();
        tickRate = powerUp.GetTickRate();
        radius = powerUp.GetRadius();

        tickTimer = tickRate;

        if (loopSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.clip = loopSound;
            audioSource.loop = true;
            audioSource.volume = loopVolume;
            audioSource.spatialBlend = 0f;
            audioSource.PlayDelayed(0.01f);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        tickTimer -= Time.deltaTime;

        if (tickTimer <= 0f)
        {
            tickTimer = tickRate;

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position,
                radius,
                enemyLayer
            );

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    hit.GetComponent<Enemy>()
                        .TakeDamage((int)powerUp.GetDamage());
                }
            }
        }

        if (timer >= duration)
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }

            Destroy(gameObject);
        }
    }
}