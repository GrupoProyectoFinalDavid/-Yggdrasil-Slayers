using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Personajes disponibles")]
    public GameObject[] characterPrefabs; // Aquí metes los 5 prefabs de personajes

    [Header("Botones de selección")]
    public Button[] characterButtons; // Los 5 botones de personajes
    public Button randomButton;

    [Header("Botón jugar")]
    public Button playButton;

    [Header("Colores de botones")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.green;

    private int selectedCharacterIndex = -1;
    private bool randomSelected = false;

    private void Start()
    {
        // Asignar eventos a los 5 botones de personaje
        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i;

            characterButtons[i].onClick.AddListener(() =>
            {
                SelectCharacter(index);
            });
        }

        // Botón aleatorio
        randomButton.onClick.AddListener(SelectRandom);

        // Botón jugar
        playButton.onClick.AddListener(Play);

        // Opcional: seleccionar el primer personaje por defecto
        SelectCharacter(0);
    }

    private void SelectCharacter(int index)
    {
        selectedCharacterIndex = index;
        randomSelected = false;

        UpdateButtonVisuals();
    }

    private void SelectRandom()
    {
        selectedCharacterIndex = -1;
        randomSelected = true;

        UpdateButtonVisuals();
    }

    private void UpdateButtonVisuals()
    {
        for (int i = 0; i < characterButtons.Length; i++)
        {
            ColorBlock colors = characterButtons[i].colors;
            colors.normalColor = normalColor;
            colors.selectedColor = normalColor;
            colors.highlightedColor = normalColor;
            characterButtons[i].colors = colors;
        }

        ColorBlock randomColors = randomButton.colors;
        randomColors.normalColor = normalColor;
        randomColors.selectedColor = normalColor;
        randomColors.highlightedColor = normalColor;
        randomButton.colors = randomColors;

        if (randomSelected)
        {
            SetButtonSelected(randomButton);
        }
        else if (selectedCharacterIndex >= 0 && selectedCharacterIndex < characterButtons.Length)
        {
            SetButtonSelected(characterButtons[selectedCharacterIndex]);
        }
    }

    private void SetButtonSelected(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = selectedColor;
        colors.selectedColor = selectedColor;
        colors.highlightedColor = selectedColor;
        button.colors = colors;
    }

    private void Play()
    {
        int finalCharacterIndex = selectedCharacterIndex;

        if (randomSelected)
        {
            finalCharacterIndex = Random.Range(0, characterPrefabs.Length);
        }

        GameObject selectedPrefab = characterPrefabs[finalCharacterIndex];

        Debug.Log("Personaje elegido: " + selectedPrefab.name);

        // Aquí luego usaremos selectedPrefab para spawnear el jugador.
    }

    public GameObject GetSelectedCharacterPrefab()
    {
        int finalCharacterIndex = selectedCharacterIndex;

        if (randomSelected)
        {
            finalCharacterIndex = Random.Range(0, characterPrefabs.Length);
        }

        return characterPrefabs[finalCharacterIndex];
    }
}