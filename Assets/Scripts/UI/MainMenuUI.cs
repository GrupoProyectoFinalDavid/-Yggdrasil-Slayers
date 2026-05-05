using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public string gameSceneName = "Personajes";

    [Header("Paneles")]
    public GameObject optionsPanel;
    public GameObject unlocksPanel;
    public GameObject upgradesPanel;

    [Header("Música")]
    public AudioClip Boton;

    [Header("Volumen")]
    public Slider volumeSlider; // ← solo para el show/hide del hover

    void Start()
    {
        if (optionsPanel  != null) optionsPanel.SetActive(false);
        if (unlocksPanel  != null) unlocksPanel.SetActive(false);
        if (upgradesPanel != null) upgradesPanel.SetActive(false);

        if (volumeSlider != null)
            volumeSlider.gameObject.SetActive(false); // oculto por defecto

        SetupVolumeButtonHover();
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

    public void QuitGame()
    {
        MusicManager.Instance?.PlaySFX(Boton);        
        Application.wantsToQuit += OnQuitApplication;
        Application.Quit();
    }
    private bool OnQuitApplication() { Debug.Log("Saliendo..."); return true; }

    public void Options()
    {
        MusicManager.Instance?.PlaySFX(Boton);
        if (optionsPanel  != null) optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    public void Unlocks()
    {
        MusicManager.Instance?.PlaySFX(Boton);
        if (unlocksPanel  != null) unlocksPanel.SetActive(!unlocksPanel.activeSelf);
    }

    public void Upgrades()
    {
        MusicManager.Instance?.PlaySFX(Boton);
        if (upgradesPanel != null) upgradesPanel.SetActive(!upgradesPanel.activeSelf);
    }

    public void ClosePanel(GameObject panel)
    {
        MusicManager.Instance?.PlaySFX(Boton);
        if (panel != null) panel.SetActive(false);
    }

    public void CloseAll()
    {
        MusicManager.Instance?.PlaySFX(Boton);
        if (optionsPanel  != null) optionsPanel.SetActive(false);
        if (unlocksPanel  != null) unlocksPanel.SetActive(false);
        if (upgradesPanel != null) upgradesPanel.SetActive(false);
    }

    public void NextSong()     => MusicManager.Instance?.NextSong();
    public void PreviousSong() => MusicManager.Instance?.PreviousSong();
    public void TogglePause()  => MusicManager.Instance?.TogglePause();
}