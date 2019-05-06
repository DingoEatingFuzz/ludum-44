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
    // Start is called before the first frame update
    bool AwaitingConfirmation = true;
    bool isOpen = false;

    public bool IsOpen
    {
        get
        {
            return isOpen;
        }
    }
}
  