using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public PowerUpData powerUpData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PowerUpManager manager = other.GetComponent<PowerUpManager>();
            manager.AddPowerUp(powerUpData);

            Destroy(gameObject);
        }
    }
}