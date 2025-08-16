using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [Header("UI Trees")]
    [SerializeField] private VisualTreeAsset mainMenuTree;
    [SerializeField] private VisualTreeAsset settingsMenuTree;

    private UIDocument ui;
    private VisualElement root;

    private AsyncOperation gameSceneLoadOp;

    private Dictionary<string, System.Action> buttonActions;

    private void Awake()
    {
        UpdateUIDocument();
    }

    private void Start()
    {
        StartGameSceneLoad();
    }

    private void OnEnable()
    {
        UpdateUIDocument();

        AddCallbacks();
    }

    private void OnDisable()
    {
        RemoveCallbacks();
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
    /// Obtains the currently applied UIDocument and stores the rootVisualElements.
    /// </summary>
    private void UpdateUIDocument()
    {
        ui = GetComponent<UIDocument>();
        root = ui.rootVisualElement;

        buttonActions = new Dictionary<string, System.Action>
        {
        { "PlayButton", () => LoadGameScene() },
        { "ExitButton", () => ExitMenuScene() },
        { "SettingsButton", () => SettingsButton() },
        { "SettingsBackButton", () => SettingsBackButton() }
        };
    }

    /// <summary>
    /// Starts to load the Game Scene in the background.
    /// </summary>
    private void StartGameSceneLoad()
    {
        gameSceneLoadOp = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
        gameSceneLoadOp.allowSceneActivation = false;
    }

    /// <summary>
    /// Confirms the action of opening the Game Scene.
    /// </summary>
    private void LoadGameScene()
    {
        Debug.Log("Bruh");
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

    private void SettingsButton()
    {
        ChangeUI(settingsMenuTree);
    }

    private void SettingsBackButton()
    {
        ChangeUI(mainMenuTree);
    }

    private void ChangeUI(VisualTreeAsset tree)
    {
        RemoveCallbacks();
        ui.visualTreeAsset = tree;

        UpdateUIDocument(); 
        AddCallbacks();
    }
}
