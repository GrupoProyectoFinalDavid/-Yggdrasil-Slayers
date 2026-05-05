using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSliderSync : MonoBehaviour
{
    private Slider slider;
    private bool isSyncing = false;

    void Awake()
    {
        slider = GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 0.2f;
    }

    void OnEnable()
    {
        if (MusicManager.Instance != null)
        {
            slider.value = MusicManager.Instance.GetVolume(); 
            MusicManager.Instance.OnVolumeChanged += SyncFromManager;
        }

        slider.onValueChanged.AddListener(OnSliderMoved);
    }

    void OnDisable()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.OnVolumeChanged -= SyncFromManager;

        slider.onValueChanged.RemoveListener(OnSliderMoved);
    }

    void OnSliderMoved(float value)
    {
        if (isSyncing) return;
        MusicManager.Instance?.SetVolume(value);
    }

    void SyncFromManager(float value)
    {
        if (Mathf.Approximately(slider.value, value)) return;
        isSyncing = true;
        slider.value = value;
        isSyncing = false;
    }
}