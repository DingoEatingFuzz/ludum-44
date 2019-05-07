using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem;
using System.Linq;


/// <summary>
/// Sends dialogue to the players dialogue manager when collided with by the player
/// </summary>
[System.Serializable]
public class DialogueTrigger : MonoBehaviour
{
    [Tooltip("Dialogue played on trigger")]
    public Dialogue Dialogue;

    [Tooltip("How many times can the dialogue be retriggered?")]
    public int Retriggerable = 0;

    [Tooltip("Should the dialogue reset if it's already been queued but is retriggered?")]
    public bool ResetOnRetrigger = false;

    protected int RemainingTriggers;

    /// <summary>
    /// Awake
    /// </summary>
    protected void Awake()
    {
        RemainingTriggers = 1 + Retriggerable;
    }

    /// <summary>
    /// Start
    /// </summary>
    protected void Start()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    /// <summary>
    /// If the player collides with this trigger, start a dialogue sequence.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && RemainingTriggers > 0)
        {
            var Added = other.GetComponent<DialogueManager>()?.AddToQueue(Dialogue, ResetOnRetrigger) == true;
            if (Added)
            {
                --RemainingTriggers;
            }
        }
    }
}
