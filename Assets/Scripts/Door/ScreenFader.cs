using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.color = Color.clear;
    }

    public IEnumerator FadeOut(float duration)   // → negro
    {
        yield return Fade(0f, 1f, duration);
    }

    public IEnumerator FadeIn(float duration)    // → transparente
    {
        yield return Fade(1f, 0f, duration);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            _image.color = new Color(0, 0, 0, Mathf.Lerp(from, to, t));
            yield return null;
        }
        _image.color = new Color(0, 0, 0, to);
    }
}