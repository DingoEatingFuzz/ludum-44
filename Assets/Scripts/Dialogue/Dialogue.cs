using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// Messages and configuration for a dialogue
    /// Note: Changes to these values will persist after runtime, so don't modify the list elements!
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Scriptables/Dialogue/Dialogue")]
    public class Dialogue : ScriptableObject, IEnumerable<MessagePair>
    {
        [Tooltip("Messages for this dialogue")]
        [SerializeField]
        private List<MessagePair> Messages;

        public Priority Priority { get => _Priority; }
        [Tooltip("Higher priority will play before lower priority")]
        [SerializeField]
        private Priority _Priority = Priority.Normal;

        public bool Interruptible { get => _Interruptible; }
        [Tooltip("Can other dialogues interrupt this one?")]
        [SerializeField]
        private bool _Interruptible = false;

        public bool ResumeAfterInterrupt { get => _ResumeAfterInterrupt; }
        [Tooltip("Should this dialogue continue after being interrupted?")]
        [SerializeField]
        private bool _ResumeAfterInterrupt = false;

        [System.NonSerialized]
        private IEnumerator<MessagePair> _Enumerator;
        /// <summary>
        /// Gets a persistent enumerator
        /// </summary>
        public IEnumerator<MessagePair> Enumerator {
            get
            {
                if (_Enumerator is null)
                {
                    _Enumerator = Messages.GetEnumerator();
                }
                return _Enumerator;
            }
        }

        public IEnumerator<MessagePair> GetEnumerator()
        {
            return ((IEnumerable<MessagePair>)Messages).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<MessagePair>)Messages).GetEnumerator();
        }
    }
}
