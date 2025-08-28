using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private UIDocument ui;
    private VisualElement root;
    private VisualElement panel;

    private AsyncOperation gameSceneLoadOp;

    private Dictionary<string, System.Action> buttonActions;

    private void Awake()
    {
        root = ui.rootVisualElement;
        panel = GetPanel("MainPanel");

        InitializeButtonActions();
    }

    private void Start()
    {
        StartGameSceneLoad();
    }

    private void OnEnable()
    {
        AddCallbacks();
    }

    private void OnDisable()
    {
        RemoveCallbacks();
    }

    /// <summary>
    /// Updates the button actions dictionary with the corresponding methods.
    /// </summary>
    private void InitializeButtonActions()
    {
        buttonActions = new Dictionary<string, System.Action>
        {
            { "MainPlayButton", LoadGameScene },
            { "MainExitButton", ExitMenuScene },
            { "MainSettingsButton", OpenSettingsPanel },
            { "SettingsBackButton", CloseSettingsPanel },
        };
    }

    /// <summary>
    /// Gets a specified button from the current UI Document.
    /// </summary>
    /// <param name="name">The name of the button.</param>
    /// <returns>The specified Button if found, null if not.</returns>
    private Button GetButton(string name)
    {
        Button button = root.Q<Button>(name);
        if (button == null)
        {
            Debug.LogWarning($"Button '{name}' not found.");
            return null;
        }

        return button;
    }

    /// <summary>
    /// Goes through all buttons in the dictionary and adds a callback to run a method when clicked.
    /// </summary>
    private void AddCallbacks()
    {
        foreach (var pair in buttonActions)
        {
            if (GetButton(pair.Key) is Button btn)
                btn.clicked += pair.Value;
        }
    }

    /// <summary>
    /// Goes through all buttons in the dictionary and removes the callback that ran when the button was clicked.
    /// </summary>
    private void RemoveCallbacks()
    {
        foreach (var pair in buttonActions)
        {
            if (GetButton(pair.Key) is Button btn)
                btn.clicked -= pair.Value;
        }
    }

    /// <summary>
    /// Obtains a specified panel from the current UI Document.
    /// </summary>
    /// <param name="name">The name of the desired panel.</param>
    /// <returns>The panel as a VisualElement.</returns>
    private VisualElement GetPanel(string name)
    {
        return root.Q<VisualElement>(name);
    }

    /// <summary>
    /// Changes the currently active panel to the specified one.
    /// </summary>
    /// <param name="name">The name of the desired panel.</param>
    private void UpdatePanel(string name)
    {
        panel.style.display = DisplayStyle.None;
        var newPanel = GetPanel(name);
        if (newPanel == null) // Error handling
        {
            Debug.LogWarning($"Panel '{name}' not found.");
            return;
        }
        panel = newPanel;
        panel.style.display = DisplayStyle.Flex;
    }

    /// <summary>
    /// Starts to load the Game Scene in the background.
    /// </summary>
    private void StartGameSceneLoad()
    {
        gameSceneLoadOp = SceneManager.LoadSceneAsync("FRC2025Scene", LoadSceneMode.Single);
        gameSceneLoadOp.allowSceneActivation = false;
    }

    /// <summary>
    /// Confirms the action of opening the Game Scene.
    /// </summary>
    private void LoadGameScene()
    {
        gameSceneLoadOp.allowSceneActivation = true;
    }

    /// <summary>
    /// Exits and closes the project.
    /// </summary>
    private void ExitMenuScene()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Closes the Main Panel and opens the Settings Panel.
    /// </summary>
    private void OpenSettingsPanel()
    {
        UpdatePanel("SettingsPanel");
    }

    /// <summary>
    /// Closes the Settings Panel and opens the Main Panel.
    /// </summary>
    private void CloseSettingsPanel()
    {
        UpdatePanel("MainPanel");
    }
}