using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    private AsyncOperation gameSceneLoadOp;
    private void Start()
    {
        gameSceneLoadOp = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
        gameSceneLoadOp.allowSceneActivation = false;
    }

    public void LoadGameScene(Object obj)
    {
        gameSceneLoadOp.allowSceneActivation = true;
        Destroy(obj);
    }
    public void ExitMenuScene()
    {
        if (Application.isEditor)
        {
            //EditorApplication.ExitPlaymode();
        } 
        else
        {
            Application.Quit();
        }
    }
}
