using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace DialogueSystem
{
    [RequireComponent(typeof(AudioSource))]
    [System.Serializable]
    public class DialogueManager : MonoBehaviour
    {
        // Start is called before the first frame update
        public AudioClip NewMessage;
        public GameObject DialogueIndicatorCanvas;
        public GameObject DialogueCanvas;
        protected List<Dialogue> DialogueQueue = new List<Dialogue>();
        protected Coroutine DialogueCoroutine;
        public float WaitForInputTime = 7f;
        protected int QueueNum => DialogueQueue.Count;
        protected GameObject DialogueIndicator = null;
        protected GameObject DialogueHud = null;
        protected Dialogue CurrentDialogue = null;
        protected MessagePair CurrentMessage = null;
        private AudioSource AudioSource;

        protected bool Dismiss;
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
            var QueueCount = QueueNum+1;
            AdvanceScript = Input.GetButton("PayRespects");
            Dismiss = Input.GetButton("Dismiss");

            #region Display the dialogue indicator if there's messages waiting and the dialogue coroutine isn't running
            if (!DialogueIndicator && QueueCount > 0 && DialogueCoroutine is null)
            {
                Text NumMessages = DialogueIndicator.transform.Find("NumMessages").GetComponent<Text>();
            }
            #endregion

            #region Run the dialogue if it's not already running
            if (DialogueCoroutine is null && DialogueIndicator.activeInHierarchy && AdvanceScript)
            {
                //Clear Dialogue Indicator
                Text NumMessages = DialogueIndicator.transform.Find("NumMessages").GetComponent<Text>();
                Text MessagesWaiting = DialogueIndicator.transform.Find("MessagesWaiting").GetComponent<Text>();
                MessagesWaiting.text = "Message Received";
                NumMessages.text = "0";
                DialogueIndicator.SetActive(false);

                //Show Dialogue
                DialogueHud.SetActive(true);
                DialogueCoroutine = StartCoroutine(RunThroughDialogue());
            }
            #endregion

            #region Remove dialogue from queue if dismissed
            if (QueueCount > 0 && Dismiss)
            {
                //Clear Dialogue Indicator
                Text NumMessages = DialogueIndicator.transform.Find("NumMessages").GetComponent<Text>();
                Text MessagesWaiting = DialogueIndicator.transform.Find("MessagesWaiting").GetComponent<Text>();
                MessagesWaiting.text = "Message Received";
                NumMessages.text = "0";
                DialogueIndicator.SetActive(false);
                DialogueQueue.Clear();
                DialogueQueue = new List<Dialogue>();
                QueueCount++;

                //Clear Dialogue if open
                if (DialogueCoroutine != null)
                {
                }
            }
            #endregion

        }
        void Awake()
        {
            if(!DialogueIndicator)
            {
                DialogueIndicator = Instantiate(DialogueIndicatorCanvas);
            }
            if(!DialogueHud)
            {
                DialogueHud = Instantiate(DialogueCanvas);
            }
        }

        /// <summary>
        /// Sorts <paramref name="Dialogue"/> into queue if it has not already been added
        /// </summary>
        /// <param name="Dialogue">Dialogue to add</param>
        /// <returns>True if dialogue was added</returns>
        public bool AddToQueue(Dialogue Dialogue, bool ResetDialogue = true)
        {
            AudioSource = GetComponent<AudioSource>();
            var Added = false;
            var QueueCount = QueueNum+1;
            if (DialogueQueue.Contains(Dialogue) == false)
            {
                AudioSource.PlayOneShot(NewMessage);
                Dialogue.ResetEnumerator();                
                DialogueQueue.Add(Dialogue);
                DialogueQueue = DialogueQueue.OrderByDescending(d => d.Priority).ToList();
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
                            MessagesWaiting.text = "Messages Received";
                        }
                        NumMessages.text = QueueCount.ToString();
                        DialogueIndicator.SetActive(true);
                    }
                }
                #endregion
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
            DialogueHud.SetActive(true);
            if (DialogueQueue.Count > 0)
            {

                var MessageBox = DialogueHud.transform.Find("Speech").GetComponent<Text>();
                var Portrait = DialogueHud.transform.Find("Portrait").GetComponent<Image>();

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

                    

                    if(Dismiss)
                    {
                        break;
                    }
                    else
                    {
                        yield return new WaitUntil(() => Time.time >= EndTime || AdvanceScript || Dismiss);
                    }

                } while (DialogueQueue.Count > 0);

                DialogueHud.SetActive(false);
            }
            PostDialogueCoroutine();
        }

        protected void PostDialogueCoroutine()
        {
            DialogueCoroutine = null;
        }
    }
}