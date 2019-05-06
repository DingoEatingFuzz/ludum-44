using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DialogueSystem
{
    [System.Serializable]
    //[CreateAssetMenu(fileName = "MovementData", menuName = "Scriptables/Movement/MovementData")]
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Scriptables/Dialogue/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [Tooltip("Messages for this dialogue")]
        public List<MessagePair> Messages;

        [Tooltip("Priority to play")]
        public Priority Priority = Priority.Normal;

        [Tooltip("Can other dialogues interrupt this one?")]
        public bool Interruptible = false;

        [Tooltip("Should this dialogue continue after being interrupted?")]
        public bool ResumeAfterInterrupt = false;

        public Dialogue Clone()
        {
            Dialogue Clone = CreateInstance<Dialogue>();

            Clone.Messages = Messages.Select(o => o).ToList();
            Clone.Priority = Priority;
            Clone.Interruptible = Interruptible;
            Clone.ResumeAfterInterrupt = ResumeAfterInterrupt;

            return Clone;
        }
    } 
}
