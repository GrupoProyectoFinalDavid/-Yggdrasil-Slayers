using UnityEngine;

public class GameManagerPersonajes : MonoBehaviour
{
    public static GameManagerPersonajes Instance { get; private set; }

    [SerializeField] private string gameSceneName = "GameScene";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>Llamado por CharacterSelectionManager al pulsar Play.</summary>
    public void StartGameWithCharacter(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("[GameManagerPersonajes] Prefab nulo.");
            return;
        }

        if (SceneLoader.Instance == null)
        {
            Debug.LogError("[GameManagerPersonajes] SceneLoader no encontrado.");
            return;
        }

        SceneLoader.Instance.SetSelectedCharacter(prefab);
        SceneLoader.Instance.LoadScene(gameSceneName);
    }
}