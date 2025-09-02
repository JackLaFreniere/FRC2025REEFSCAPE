using UnityEngine;
using UnityEngine.SceneManagement;

namespace FRC2025
{
    public class ExitGame : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("MenuScene");
            }
        }
    }
}