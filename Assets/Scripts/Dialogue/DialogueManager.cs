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
        public GameObject DialogueIndicatorCanvas;
        public GameObject DialogueCanvas;
        protected List<Dialogue> DialogueQueue = new List<Dialogue>();
        protected Coroutine DialogueCoroutine;
        public float WaitForInputTime = 7f;
        protected int QueueCount => DialogueQueue.Count;
        protected GameObject DialogueIndicator = null;

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
            //Debug.Assert((DialogueQueue.Count == 0) == (DialogueCoroutine is null), "Something has gone wrong with the dialogue manager");

            #region Display the dialogue indicator if there's messages waiting and the dialogue coroutine isn't running
            if (!DialogueIndicator && QueueCount > 0 && DialogueCoroutine is null)
            {
                DialogueIndicator = Instantiate(DialogueIndicatorCanvas);
                Text NumMessages = DialogueIndicator.transform.Find("NumMessages").GetComponent<Text>();
                Text MessagesWaiting = DialogueIndicator.transform.Find("MessagesWaiting").GetComponent<Text>();
                NumMessages.text = QueueCount.ToString();
                //Make sure to have proper grammar
                if(QueueCount == 1)
                {
                    MessagesWaiting.text = "Message Waiting [Space]";
                }
                Debug.Log($"Indicator created, should show {NumMessages.text}");
                if (!DialogueIndicator.activeInHierarchy)
                {
                    DialogueIndicator.SetActive(true);
                }
            }
            #endregion

            #region Run the dialogue if it's not already running
            if (DialogueCoroutine is null && DialogueIndicator.activeInHierarchy && AdvanceScript)
            {
                //Clear Dialogue Indicator
                Text NumMessages = DialogueIndicator.transform.Find("NumMessages").GetComponent<Text>();
                NumMessages.text = "0";
                DialogueIndicator.SetActive(false);

                //Show Dialogue
                DialogueCoroutine = StartCoroutine(RunThroughDialogue());
            }
            #endregion
        }

        /// <summary>
        /// Sorts <paramref name="Dialogue"/> into queue if it has not already been added
        /// </summary>
        /// <param name="Dialogue">Dialogue to add</param>
        /// <returns>True if dialogue was added</returns>
        public bool AddToQueue(Dialogue Dialogue, bool ResetDialogue = true)
        {
            var Added = false;
            if (DialogueQueue.Contains(Dialogue) == false)
            {
                Dialogue.ResetEnumerator();                
                DialogueQueue.Add(Dialogue);
                DialogueQueue = DialogueQueue.OrderByDescending(d => d.Priority).ToList();
                //Debug.Log($"Added {Dialogue} to the queue, total dialogues is {DialogueQueue.Count}");
                Added = true;

                #region Update dialogue messages waiting indicator when new messages are received
                if (DialogueIndicator && DialogueCoroutine is null)
                {
                    Text NumMessages = DialogueIndicator.transform.Find("NumMessages").GetComponent<Text>();
                    Text MessagesWaiting = DialogueIndicator.transform.Find("MessagesWaiting").GetComponent<Text>();
                    if (NumMessages.text != QueueCount.ToString())
                    {
                        if (QueueCount > 1)
                        {
                            MessagesWaiting.text = "Messages Waiting [Space]";
                        }
                        NumMessages.text = QueueCount.ToString();
                        Debug.Log($"Updated, should show {NumMessages.text}");
                        DialogueIndicator.SetActive(true);
                    }
                }
                #endregion

                // Since we don't want it to run through the dialogue when stuff is added
                // this should move somewhere else, replaced by the stuff you have below
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
                    }
                    else
                    {
                        DialogueQueue.Remove(CurrentDialogue);
                        CurrentDialogue = null;
                        continue;
                    }
                    #endregion

                    Portrait.sprite = CurrentMessage.CharacterSprite;
                    MessageBox.text = CurrentMessage.DisplayMessage;

                    EndTime = Time.time + CurrentMessage.AutoAdvance;

                    yield return new WaitForSeconds(.5f);

                    // Another condition can be added to the WaitUntil like a CloseDialogue bool 
                    yield return new WaitUntil(() => Time.time >= EndTime || AdvanceScript);

                    // Then something here to determine if CloseDialogue was the reason, and if so
                    // break the loop (which will not advance the dialogue and then close the canvas per
                    // the Destroy below the while
                    // E.g., if (CloseDialogue == true) { break; } (`break` tells the loop to abort)
                    // If you WANT the message that is open to be discarded then you would remove it from the
                    // CurrentMessage from CurrentDialogue (like in the "get the current message" region above)
                    // If you want to abort the whole dialogue and not just the current message then you jsut
                    // remove CurrentDialogue from the dialogue queue (like in the "Get the current dialogue" region above)

                } while (DialogueQueue.Count > 0);

                Destroy(DialogueCanvas);
            }
            PostDialogueCoroutine();
        }

        protected void PostDialogueCoroutine()
        {
            // This is called when the dialogue is finished running, which normally would be when 
            // the queue is empty, but will also now be when the dialogue is closed, so if there is still dialogue
            // then you'd want to remake the indicator here (and update the message count text
            DialogueCoroutine = null;
        }
    }
}