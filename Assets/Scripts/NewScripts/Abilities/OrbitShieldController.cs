using UnityEngine;
using System.Collections.Generic;

public class OrbitShieldController : MonoBehaviour
{
    [Header("Offset")]
    public Vector3 centerOffset = new Vector3(0f, 0.5f, 0f);

    private GameObject owner;

    private List<Transform> shields =
        new List<Transform>();

    private float orbitRadius;
    private float rotationSpeed;

    private float currentAngle;

    private GameObject shieldPrefab;
    private float shieldDamage;

    public void Init(
        GameObject owner,
        GameObject shieldPrefab,
        int shieldCount,
        float orbitRadius,
        float rotationSpeed,
        float damage
    )
    {
        this.owner = owner;
        this.orbitRadius = orbitRadius;
        this.rotationSpeed = rotationSpeed;
        this.shieldPrefab = shieldPrefab;
        this.shieldDamage = damage;
        this.rotationSpeed = rotationSpeed;

        for (int i = 0; i < shieldCount; i++)
        {
            GameObject shield =
                Instantiate(shieldPrefab, transform);

            OrbitShield shieldScript =
                shield.GetComponent<OrbitShield>();

            shieldScript.Init(damage);

            shields.Add(shield.transform);
        }
    }

    public void UpdateShields(
        int newCount,
        float damage
    )
    {
        shieldDamage = damage;

        // Crear escudos faltantes
        while (shields.Count < newCount)
        {
            GameObject shield =
                Instantiate(shieldPrefab, transform);

            OrbitShield shieldScript =
                shield.GetComponent<OrbitShield>();

            shieldScript.Init(shieldDamage);

            shields.Add(shield.transform);
        }

        // Actualizar daño de todos
        foreach (Transform shield in shields)
        {
            OrbitShield shieldScript =
                shield.GetComponent<OrbitShield>();

            shieldScript.Init(shieldDamage);
        }
    }

    void Update()
    {
        if (owner == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 centerPosition = owner.transform.position + centerOffset;

        transform.position = centerPosition;

        currentAngle += rotationSpeed * Time.deltaTime;

        float angleStep = 360f / shields.Count;

        for (int i = 0; i < shields.Count; i++)
        {
            float angle =
                currentAngle + angleStep * i;

            float rad = angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Cos(rad),
                Mathf.Sin(rad),
                0f
            ) * orbitRadius;

            shields[i].position = centerPosition + offset;
        }
    }
}