using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem;



[System.Serializable]
public class DialogueTrigger : MonoBehaviour
{
    public Dialogue Dialogue;

    public int MaxOccurrences = 1;
    private int Occurrences = 0;
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    /// <summary>
    /// If the player collides with this trigger, start a dialogue sequence.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && Occurrences < MaxOccurrences)
        {
            other.GetComponent<DialogueManager>()?.AddToQueue(Dialogue.Clone());
            Occurrences++;
        }
    }
}
