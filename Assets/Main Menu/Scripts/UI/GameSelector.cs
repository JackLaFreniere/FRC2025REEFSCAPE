using UnityEngine;
using UnityEngine.UIElements;

public class GameSelector : MonoBehaviour
{
    [SerializeField] private UIDocument ui;
    private VisualElement root;
    private VisualElement imagePanel;

    private DropdownField gameDropdown;


    [SerializeField] private GameIcons[] gameChoices;

    private void Awake()
    {
        root = ui.rootVisualElement;
        gameDropdown = root.Q<DropdownField>("GameSelector");
        imagePanel = root.Q<VisualElement>("ImagePanel");

        SetupDropdown();
    }

    /// <summary>
    /// Initializes the dropdown with the game choices provided in the inspector.
    /// </summary>
    private void SetupDropdown()
    {
        gameDropdown.choices.Clear();

        foreach (GameIcons game in gameChoices)
        {
            gameDropdown.choices.Add(game.iconName);
        }

        if (gameDropdown.index == -1) gameDropdown.index = 0;

        gameDropdown.RegisterValueChangedCallback(evt => { OnValueChanged(gameDropdown.index); });
    }

    private void OnValueChanged(int index)
    {
        imagePanel.style.backgroundImage = new StyleBackground(gameChoices[index].iconTexture);
    }
}
