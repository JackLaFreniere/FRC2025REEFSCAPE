using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuButton : MonoBehaviour
{
    private VisualElement root;

    private static AsyncOperation gameSceneLoadOp;

    private readonly Dictionary<string, System.Action> buttonActions = new()
    {
        { "PlayButton", () => LoadGameScene() },
        { "ExitButton", () => ExitMenuScene() }
    };

    private void Start()
    {
        StartGameSceneLoad();
    }

    private void OnEnable()
    {
        UpdateUIDocument();

        MainMenuAddCallBack();
    }

    private void OnDisable()
    {
        MainMenuRemoveCallBack();
    }

    /// <summary>
    /// Goes through all buttons in the dictionary and adds a callback to run a method when clicked.
    /// </summary>
    private void MainMenuAddCallBack()
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
    private void MainMenuRemoveCallBack()
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
        root = GetComponent<UIDocument>().rootVisualElement;
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
    private static void LoadGameScene()
    {
        gameSceneLoadOp.allowSceneActivation = true;
    }

    /// <summary>
    /// Exits and closes the project.
    /// </summary>
    private static void ExitMenuScene()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
