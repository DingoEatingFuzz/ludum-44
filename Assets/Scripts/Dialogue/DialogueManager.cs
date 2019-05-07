using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace DialogueSystem
{
    [System.Serializable]
    public class DialogueManager : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject DialogueCanvas;
        protected List<Dialogue> DialogueQueue = new List<Dialogue>();
        protected Coroutine DialogueCoroutine;
        public float WaitForInputTime = 7f;

        private bool _AdvanceScript;
        protected bool AdvanceScript
        {
            get => _AdvanceScript;
            set
            {
                if (value != _AdvanceScript)
                {
                    _AdvanceScript = value;
                }
            }
        }


        // Update is called once per frame
        void Update()
        {
            AdvanceScript = Input.GetButton("PayRespects");
            Debug.Assert((DialogueQueue.Count == 0) == (DialogueCoroutine is null), "Something has gone wrong with the dialogue manager");
        }

        /// <summary>
        /// Sorts <paramref name="Dialogue"/> into queue if it has not already been added
        /// </summary>
        /// <param name="Dialogue">Dialogue to add</param>
        /// <returns>True if dialogue was added</returns>
        public bool AddToQueue(Dialogue Dialogue)
        {
            var Added = false;
            if (DialogueQueue.Contains(Dialogue) == false)
            {
                DialogueQueue.Add(Dialogue);
                DialogueQueue = DialogueQueue.OrderByDescending(d => d.Priority).ToList();
                Added = true;
                if (DialogueCoroutine is null)
                {
                    DialogueCoroutine = StartCoroutine(RunThroughDialogue());
                }
            }

            return Added;
        }

        public bool HasDialogue(Dialogue Dialogue)
        {
            return DialogueQueue.Contains(Dialogue);
        }

        /// <summary>
        /// Displays all dialogue in the queue
        /// </summary>
        /// <returns>IEnumerator</returns>
        protected IEnumerator RunThroughDialogue()
        {
            if (DialogueQueue.Count > 0)
            {
                Dialogue CurrentDialogue = null;
                MessagePair CurrentMessage = null;

                var DialogueCanvas = Instantiate(this.DialogueCanvas);
                var MessageBox = DialogueCanvas.transform.Find("Speech").GetComponent<Text>();
                var Portrait = DialogueCanvas.transform.Find("Portrait").GetComponent<Image>();

                float EndTime;

                do
                {
                    #region Get the current dialogue
                    if (CurrentDialogue is null)
                    {
                        CurrentDialogue = DialogueQueue.First();
                    }
                    else
                    {
                        Debug.Assert(CurrentDialogue != null);
                        if (CurrentDialogue.Interruptible && CurrentDialogue != DialogueQueue.First())
                        {
                            if (CurrentDialogue.ResumeAfterInterrupt == false)
                            {
                                DialogueQueue.Remove(CurrentDialogue);
                            }
                            CurrentDialogue = DialogueQueue.First();
                        }
                    }
                    #endregion

                    #region Get the current message
                    if (CurrentDialogue.Enumerator.MoveNext())
                    {
                        CurrentMessage = CurrentDialogue.Enumerator.Current;
                        Debug.Log($"Enumerator moved to {CurrentMessage}");
                    }
                    else
                    {
                        Debug.Log("Enumerator couldn't move to next");
                        DialogueQueue.Remove(CurrentDialogue);
                        CurrentDialogue = null;
                        continue;
                    }
                    #endregion

                    Portrait.sprite = CurrentMessage.CharacterSprite;
                    MessageBox.text = CurrentMessage.Message;

                    EndTime = Time.time + CurrentMessage.AutoAdvance;

                    yield return new WaitForSeconds(.5f);
                    yield return new WaitUntil(() => Time.time >= EndTime || AdvanceScript);

                    Debug.Log("Advancing dialogue");
                } while (DialogueQueue.Count > 0);

                Destroy(DialogueCanvas);
            }
            PostDialogueCoroutine();
        }

        protected void PostDialogueCoroutine()
        {
            DialogueCoroutine = null;
        }
    }
}