using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void GotoMainScene()
    {
        SceneManager.LoadScene("Level1");
    }
    public void GotoMenuScene()
    {
        SceneManager.LoadScene("MainMenuRedux");
    }
    public void doExitGame()
    {
        Application.Quit();
    }
}
  