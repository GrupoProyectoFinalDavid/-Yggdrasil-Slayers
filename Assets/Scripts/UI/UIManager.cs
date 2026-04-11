using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject PausaPanel;
    public GameObject optionsPanel;
    
    [Header("Música")]
    public AudioClip Boton;

    [Header("Volumen")]
    public Slider volumeSlider;

    void Start()
    {   
        if (PausaPanel != null)  PausaPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        if (volumeSlider != null && MusicManager.Instance != null)
        {
            volumeSlider.gameObject.SetActive(false);
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 0.2f;
            volumeSlider.value    = MusicManager.Instance.GetVolume();
            volumeSlider.onValueChanged.AddListener(MusicManager.Instance.SetVolume);
        }

        SetupVolumeButtonHover();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsPanel != null && optionsPanel.activeSelf)
                BackToPause();
            else
                Pausa();
        }
    }

    public void ClosePanel(GameObject panel)
    {
        MusicManager.Instance?.PlaySFX(Boton);
        if (panel != null) panel.SetActive(false);
    }

    void SetupVolumeButtonHover()
    {
        GameObject volBtn = GameObject.Find("VolumeButton");
        if (volBtn == null) return;

        EventTrigger trigger = volBtn.GetComponent<EventTrigger>()
                               ?? volBtn.AddComponent<EventTrigger>();

        EventTrigger.Entry onEnter = new EventTrigger.Entry();
        onEnter.eventID = EventTriggerType.PointerEnter;
        onEnter.callback.AddListener(_ => volumeSlider.gameObject.SetActive(true));
        trigger.triggers.Add(onEnter);

        EventTrigger.Entry onExit = new EventTrigger.Entry();
        onExit.eventID = EventTriggerType.PointerExit;
        onExit.callback.AddListener(_ =>
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                    volumeSlider.GetComponent<RectTransform>(),
                    Input.mousePosition, null))
            {
                volumeSlider.gameObject.SetActive(false);
            }
        });
        trigger.triggers.Add(onExit);
    }

    public void ContinueGame()
    {
        MusicManager.Instance?.PlaySFX(Boton);
        PausaPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        MusicManager.Instance?.PlaySFX(Boton);
        SceneManager.LoadScene("MainMenuUI");
    }
    
    public void Pausa()
    {
        MusicManager.Instance?.PlaySFX(Boton);
        bool isPaused = !PausaPanel.activeSelf;

        PausaPanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        EventSystem.current.SetSelectedGameObject(null);

        if (isPaused)
        {
            foreach (var anim in PausaPanel.GetComponentsInChildren<Animator>(true))
            {
                anim.updateMode = AnimatorUpdateMode.UnscaledTime; // ← necesario con timeScale 0
                anim.Play(anim.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
            }
        }
    }
    
    public void OpenOptions()
    {
        MusicManager.Instance?.PlaySFX(Boton);
        optionsPanel.SetActive(true);
    }

    public void BackToPause()
    {
        MusicManager.Instance?.PlaySFX(Boton);
        optionsPanel.SetActive(false);
        PausaPanel.SetActive(true);
    }
}