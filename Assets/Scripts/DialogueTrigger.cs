using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MessagePair {
    public string Character;
    [TextArea]
    public string Message;
}

public class DialogueTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public List<MessagePair> Messages;
    public int MaxOccurrences = 1;
    private int Occurrences = 0;
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// If the player collides with this trigger, start a dialogue sequence.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && Occurrences < MaxOccurrences) {
            var dialogueManager = other.GetComponent<DialogueManager>();
            if (!dialogueManager.IsOpen) {
                Occurrences++;
                StartCoroutine(RunThroughDialogue(dialogueManager));
            }
        }
    }

    IEnumerator RunThroughDialogue(DialogueManager dialogueManager) {
        foreach (var message in Messages) {
            yield return dialogueManager.Write(message.Message, message.Character);
        }
    }
}
