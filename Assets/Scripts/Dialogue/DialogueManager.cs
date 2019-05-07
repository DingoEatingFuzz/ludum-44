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
        public int QueueCount => DialogueQueue.Count;
        public bool IndicatorDisplayed = false; // Check if DialogueIndicatorCanvas is null instead of manually keeping track of it

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

            // This is redundant with the property QueueCount. The `=> DialogueQueue.Count` part tells 
            // VS to treat it like a variable, but it acts like a function so it'll always be the current count
            var QueueCount = DialogueQueue.Count;


            #region Run the dialogue if the player presses the open key
            // Basically, if there is dialogue and there is an indicator canvas (i.e., if it's not null) then
            // start the display routine (like what is currently int he AddToQueue function
            #endregion

            #region Check if the player is pressing the close key
            // If you go with the CloseDialogue bool route then you would just set that to true when the appropriate key is pressed
            // Similar to how AdvanceScript is working
            #endregion

            // There should probably be some sort of "cooldown" to prevent opening and closing rapidly, but it probably won't actually be an issue
            // If I were to make something like that I would use a "NextAllowedOpen" and "NextAllowedClose" with Time.time + cooldown duration
            // then check that the time is past that before doing anything with open and close

            // Also, the delay before you can advance the dialogue will still be in place after you open the dialogue, since it's basically like
            // it's going the first time, so a cooldown between opening the dialogue and being able to advance would be redundant
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

                // Since we don't want it to run through the dialogue when stuff is added
                // this should move somewhere else, replaced by the stuff you have below
                #region Run the dialogue if it's not already running
                if (DialogueCoroutine is null)
                {
                    DialogueCoroutine = StartCoroutine(RunThroughDialogue());
                }
                #endregion



                #region Display the indicator if it's not displayed
                if (IndicatorDisplayed == true) // you can use (DialogueIndicator != null) and you don't have to manually keep track of it
                {
                    #region
                    // The dialogue should be destroyed when we're done with it, not right before we need a new one
                    Destroy(this.DialogueIndicatorCanvas);
                    var DialogueIndicator = Instantiate(this.DialogueIndicatorCanvas);
                    #endregion


                    DialogueIndicatorCanvas.SetActive(true); // This shouldn't be needed afaik
                    var NumMessages = DialogueIndicator.transform.Find("NumMessages").GetComponent<Text>();
                    NumMessages.text = QueueCount.ToString();
                    Debug.Log($"Should show {NumMessages.text}");
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
            if (DialogueQueue.Count > 0)
            {

                // Since the intent of the RunThroughDialogue is to do just that the waiting should happen outside
                // this function, and then call it when we're no longer waiting
                #region Wait on indicator
                IndicatorDisplayed = true;
                yield return new WaitUntil(() => AdvanceScript);
                IndicatorDisplayed = false;
                Destroy(DialogueIndicatorCanvas); // Leave this though since the indicator doesn't need to be open when the dialogue is playing
                #endregion

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