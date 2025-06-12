using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuButton : MonoBehaviour
{
    public void LoadGameScene(Object obj)
    {
        SceneManager.LoadScene("GameScene");
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
