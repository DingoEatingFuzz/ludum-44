using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortaler : MonoBehaviour
{
    public string NextSceneName;
    private GameStateComponent StateData;

    public void Start()
    {
        StateData = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameStateComponent>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var data = StateData.GetStoredData();
            StateData.StoreData();
            SceneManager.LoadScene(NextSceneName == "" ? data.NextSceneName : NextSceneName);
        }
    }
}
