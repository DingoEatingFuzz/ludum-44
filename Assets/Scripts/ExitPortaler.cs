using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortaler : MonoBehaviour
{
    public string NextSceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameStateComponent>().StoreData();
            SceneManager.LoadScene(NextSceneName);
        }
    }
}
