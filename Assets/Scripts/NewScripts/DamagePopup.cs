using UnityEngine;
using TMPro;
using System.Collections;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float lifetime = 1f;
    public float fadeDuration = 0.3f;
    private Transform target;
    private Vector3 offset;

    private float randomSide;
    private float arcAmplitude = 0.4f;
    private float arcSpeed = 6f;
    private float time;

    private Color originalColor;

    public void Setup(int damage, Transform target, Vector3 offset)
    {
        this.target = target;
        this.offset = offset;

        if (text == null)
            text = GetComponentInChildren<TextMeshProUGUI>();

        text.text = damage.ToString();
        originalColor = text.color;

        randomSide = Random.value < 0.5f ? -1f : 1f;
        time = 0f;

        transform.localScale = Vector3.one * 1.3f;
        transform.position += Vector3.up * 0.2f;

        StartCoroutine(LifeRoutine());
    }

    void Update()
    {
        if (target == null) return;

        time += Time.deltaTime;

        // posición base siguiendo al enemigo
        Vector3 basePos = target.position + offset;

        // dirección lateral perpendicular (UI fake “left/right drift”)
        Vector3 right = Vector3.right * randomSide;

        // arco tipo seno
        float arc = Mathf.Sin(time * arcSpeed) * arcAmplitude;

        transform.position = basePos + right * arc;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            Vector3.one,
            Time.deltaTime * 10f
        );
    }

    IEnumerator LifeRoutine()
    {
        yield return new WaitForSeconds(lifetime);

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);

            text.color = new Color(
                originalColor.r,
                originalColor.g,
                originalColor.b,
                alpha
            );

            yield return null;
        }

        Destroy(gameObject);
    }
}