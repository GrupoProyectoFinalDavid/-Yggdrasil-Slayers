using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public PowerUpData powerUpData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detectado");

            PowerUpManager manager = other.GetComponent<PowerUpManager>();

            if (manager == null)
            {
                Debug.LogError("PowerUpManager es NULL");
                return;
            }

            if (powerUpData == null)
            {
                Debug.LogError("PowerUpData es NULL");
                return;
            }

            manager.AddPowerUp(powerUpData);

            Destroy(gameObject);
        }
    }
}