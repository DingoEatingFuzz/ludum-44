using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortaler : MonoBehaviour
{
    public string NextSceneName;
    private GameStateComponent StateData;
    private List<Collider> Colliders;
    private List<ParticleSystem> Particles;
    private bool IsHidden = true;

    public void Start()
    {
        StateData = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameStateComponent>();
        Colliders = new List<Collider>(gameObject.GetComponentsInChildren<Collider>());
        foreach(var Col in Colliders) { Col.enabled = false; }

        Particles = new List<ParticleSystem>(gameObject.GetComponentsInChildren<ParticleSystem>());
        foreach(var Part in Particles) { Part.Pause(); }


    }

    public void Update()
    {
        if (IsHidden && GameObject.FindGameObjectsWithTag("Finish").Length == 0)
        {
            IsHidden = false;
            foreach(var Col in Colliders) { Col.enabled = true; }
            foreach (var Part in Particles) { Part.Play(); }
        }
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
