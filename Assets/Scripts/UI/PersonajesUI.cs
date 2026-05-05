using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PersonajesUI : MonoBehaviour
{   
    public string gameSceneName = "Personajes";
    
    public GameObject optionsPanel;

    [Header("Música")]
    public AudioClip Boton;
    public TMP_Text songNameText; 

    [Header("Volumen")]
    public Slider volumeSlider; // ← solo para el show/hide del hover

    void Start()
    {   
        if (volumeSlider != null)
            volumeSlider.gameObject.SetActive(false); // oculto por defecto

        UpdateSongName();
        SetupVolumeButtonHover();
    }

    void UpdateSongName()
    {
        if (songNameText != null && MusicManager.Instance != null)
            songNameText.text = MusicManager.Instance.GetCurrentSongName();
    }

    public void ClosePanel(GameObject panel)
    {
        MusicManager.Instance?.PlaySFX(Boton);
        if (panel != null) panel.SetActive(false);
    }

    void SetupVolumeButtonHover()
    {
        GameObject volBtn = GameObject.Find("VolumeButton");
        if (volBtn == null || volumeSlider == null) return;

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

    public void PlayGame()   
    { 
        MusicManager.Instance?.PlaySFX(Boton);
        SceneManager.LoadScene(gameSceneName);
    }

    public void ExitToMenu()
    {
        MusicManager.Instance?.PlaySFX(Boton);
        SceneManager.LoadScene("MainMenuUI");
    }
    
    public void OpenOptions()
    {
        MusicManager.Instance?.PlaySFX(Boton);
        optionsPanel.SetActive(true);
    }

    public void NextSong()
    {
        MusicManager.Instance?.NextSong();
        UpdateSongName();
    }

    public void PreviousSong()
    {
        MusicManager.Instance?.PreviousSong();
        UpdateSongName();
    }

    public void TogglePause() => MusicManager.Instance?.TogglePause();
}