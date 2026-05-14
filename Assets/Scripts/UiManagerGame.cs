using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManagerGame : MonoBehaviour
{
    public static UIManagerGame Instance { get; private set; }

    [Header("Panel de muerte")]
    public GameObject deathPanel;

    [Header("Nombre de la escena de menú")]
    public string menuSceneName = "MainMenu";

    [Header("Wave UI")]
    public TMP_Text waveText;
    public TMP_Text timerText;
    public TMP_Text totalTimeText;
    public TMP_Text statusText;

    private GameManager gameManager;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        gameManager = GameManager.Instance;

        if (gameManager == null)
            Debug.LogError("[UIManager] GameManager no encontrado.");

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    private void Update()
    {
        if (gameManager == null) return;
        UpdateWaveUI();
    }

    // ── Panel de muerte ──────────────────────────────────────────────────────

    public void ShowDeathPanel()
    {
        if (deathPanel != null)
            deathPanel.SetActive(true);
    }

    public void HideDeathPanel()
    {
        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    /// <summary>Llamado por el botón del panel de muerte.</summary>
    public void GoToMenu()
    {
        Time.timeScale = 1f; // por si el juego estaba pausado
        SceneManager.LoadScene(menuSceneName);
    }

    // ── Wave UI ──────────────────────────────────────────────────────────────

    private void UpdateWaveUI()
    {
        int totalSeconds   = Mathf.FloorToInt(gameManager.TotalGameTime);
        totalTimeText.text = $"{totalSeconds / 60:D2}:{totalSeconds % 60:D2}";

        WaveEvent     activeWave     = gameManager.GetActiveWaveEvent();
        SurvivalEvent activeSurvival = gameManager.GetActiveSurvivalEvent();

        if (activeWave != null)
        {
            waveText.text    = $"Oleada {activeWave.CurrentWaveIndex} / {activeWave.TotalWaves}";
            int enemies      = activeWave.EnemiesAlive;
            timerText.text   = $"Enemigos: {enemies}";
            timerText.color  = enemies > 0 ? Color.red : Color.green;
            statusText.text  = activeWave.WaitingForNextWave ? "Siguiente oleada en breve..." : "";
            statusText.color = Color.white;
        }
        else if (activeSurvival != null)
        {
            waveText.text    = "¡Modo Supervivencia!";
            float t          = activeSurvival.TimeRemaining;
            timerText.text   = $"{Mathf.FloorToInt(t) / 60:D2}:{Mathf.FloorToInt(t) % 60:D2}";
            timerText.color  = t <= 30f ? Color.red : Color.white;
            statusText.text  = $"Enemigos vivos: {activeSurvival.EnemiesAlive}";
            statusText.color = Color.white;
        }
        else if (gameManager.AllRoomsCleared)
        {
            waveText.text    = "¡Has sobrevivido!";
            timerText.text   = "--";
            timerText.color  = Color.white;
            statusText.text  = "Todas las salas limpias";
            statusText.color = Color.yellow;
        }
        else
        {
            waveText.text   = "Entra en una sala para empezar";
            timerText.text  = "--";
            timerText.color = Color.white;
            statusText.text = "";
        }
    }
}