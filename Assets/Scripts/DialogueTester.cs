using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var DialogueManager = gameObject.GetComponent<DialogueManager>();
        Coroutine dialogue;
        if (!DialogueManager.IsOpen && Input.GetButtonDown("PayRespects")) {
            dialogue = StartCoroutine(SpawnDialogue());
        }
    }

    IEnumerator SpawnDialogue() {
        var DialogueManager = gameObject.GetComponent<DialogueManager>();
        yield return DialogueManager.Write("This is a test for", "olympus");
        yield return DialogueManager.Write("Speakonia", "olympus");
    }
}
