using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    /// <summary>Prefab del personaje seleccionado en el menú principal.</summary>
    public GameObject SelectedCharacterPrefab { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>Llamado desde el menú de selección de personaje antes de cargar la escena.</summary>
    public void SetSelectedCharacter(GameObject prefab)
    {
        SelectedCharacterPrefab = prefab;
        Debug.Log($"[SceneLoader] Personaje seleccionado: '{prefab.name}'");
    }

    /// <summary>Carga una escena por nombre manteniendo este objeto entre escenas.</summary>
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}