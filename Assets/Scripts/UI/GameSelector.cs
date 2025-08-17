using UnityEngine;
using UnityEngine.UIElements;

public class GameSelector : MonoBehaviour
{
    [SerializeField] private UIDocument ui;
    private VisualElement root;
    private DropdownField gameDropdown;

    [SerializeField] private string[] gameChoices;

    private void Awake()
    {
        root = ui.rootVisualElement;
        gameDropdown = root.Q<DropdownField>("GameSelector");

        SetupDropdown();
    }

    /// <summary>
    /// Initializes the dropdown with the game choices provided in the inspector.
    /// </summary>
    private void SetupDropdown()
    {
        gameDropdown.choices.Clear();

        foreach (string str in gameChoices)
        {
            gameDropdown.choices.Add(str);
        }

        gameDropdown.index = 0;
    }
}
