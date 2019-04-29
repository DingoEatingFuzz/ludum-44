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
        SceneManager.LoadScene("MainMenu");
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
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (AwaitingConfirmation && Input.GetButton("PayRespects"))
        {
            AwaitingConfirmation = false;
        };
        if (AwaitingConfirmation == false)
        {
            SceneManager.LoadScene("MainMenu");
        };
    }
}