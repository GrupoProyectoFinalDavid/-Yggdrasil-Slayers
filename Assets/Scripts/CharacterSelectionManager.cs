using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Personajes disponibles")]
    public GameObject[] characterPrefabs;

    [Header("Botones de selección")]
    public Button[] characterButtons;
    public Button randomButton;

    [Header("Botón jugar")]
    public Button playButton;

    [Header("UI de stats")]
    public PlayerStatsUI playerStatsUI;

    [Header("Colores de botones")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.green;

    private int selectedCharacterIndex = -1;
    private bool randomSelected = false;
    private GameObject previewInstance;

    private void Start()
    {
        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i;
            characterButtons[i].onClick.AddListener(() => SelectCharacter(index));
        }

        randomButton.onClick.AddListener(SelectRandom);
        playButton.onClick.AddListener(Play);

        SelectCharacter(0);
    }

    private void SelectCharacter(int index)
    {
        selectedCharacterIndex = index;
        randomSelected = false;

        UpdateButtonVisuals();
        PreviewCharacterStats(index);
    }

    private void SelectRandom()
    {
        selectedCharacterIndex = -1;
        randomSelected = true;

        UpdateButtonVisuals();

        int previewIndex = Random.Range(0, characterPrefabs.Length);
        PreviewCharacterStats(previewIndex);
    }

    private void PreviewCharacterStats(int index)
    {
        if (playerStatsUI == null) return;
        if (characterPrefabs == null || index < 0 || index >= characterPrefabs.Length) return;

        if (previewInstance != null)
            Destroy(previewInstance);

        previewInstance = Instantiate(characterPrefabs[index]);
        previewInstance.SetActive(false);

        PlayerStats stats = previewInstance.GetComponent<PlayerStats>();

        if (stats != null)
            playerStatsUI.SetPlayer(stats);
        else
            Debug.LogWarning($"El prefab '{characterPrefabs[index].name}' no tiene PlayerStats en el root.");
    }

    private void Play()
    {
        if (GameManagerPersonajes.Instance == null)
        {
            Debug.LogError("No hay GameManager en la escena.");
            return;
        }

        GameObject selectedPrefab = characterPrefabs[GetFinalIndex()];

        if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }

        GameManagerPersonajes.Instance.StartGameWithCharacter(selectedPrefab);
    }

    public GameObject GetSelectedCharacterPrefab()
    {
        return characterPrefabs[GetFinalIndex()];
    }

    private int GetFinalIndex()
    {
        if (randomSelected)
            return Random.Range(0, characterPrefabs.Length);

        return selectedCharacterIndex;
    }

    private void UpdateButtonVisuals()
    {
        foreach (Button btn in characterButtons)
            ResetButtonColor(btn);

        ResetButtonColor(randomButton);

        if (randomSelected)
            SetButtonSelected(randomButton);
        else if (selectedCharacterIndex >= 0 && selectedCharacterIndex < characterButtons.Length)
            SetButtonSelected(characterButtons[selectedCharacterIndex]);
    }

    private void ResetButtonColor(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor      = normalColor;
        colors.selectedColor    = normalColor;
        colors.highlightedColor = normalColor;
        button.colors = colors;
    }

    private void SetButtonSelected(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor      = selectedColor;
        colors.selectedColor    = selectedColor;
        colors.highlightedColor = selectedColor;
        button.colors = colors;
    }

    private void OnDestroy()
    {
        if (previewInstance != null)
            Destroy(previewInstance);
    }
}