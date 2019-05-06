using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DialogueSystem;

[System.Serializable]
public class PortraitPair {
    public string Key;
    public Sprite Portrait;
}

//[System.Serializable]
//public class Dialogue
//{
//    public List<MessagePair> Messages;
//    public int Priority = 0;
//    public bool IsInteruptable;
//}

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
        public bool IsOpen { get; protected set; }

        protected bool AdvanceScript = false;



        // Update is called once per frame
        void Update()
        {
            AdvanceScript = Input.GetButton("PayRespects");
        }

        /// <summary>
        /// Adds a dialogue to the queue
        /// </summary>
        /// <param name="Dialogue">Dialogue to add</param>
        public void AddToQueue(Dialogue Dialogue)
        {
            DialogueQueue.Add(Dialogue.Clone());

            if (DialogueCoroutine is null)
            {
                DialogueCoroutine = StartCoroutine(RunThroughDialogue());
            }
        }

        protected IEnumerator RunThroughDialogue()
        {
            IsOpen = true;
            if (DialogueQueue.Count == 0)
            {
                DialogueCoroutine = null;

            } else
            {
                MessagePair Message;

                var DialogueCanvas = Instantiate(this.DialogueCanvas);
                var MessageBox = DialogueCanvas.transform.Find("Speech").GetComponent<Text>();
                var Portrait = DialogueCanvas.transform.Find("Portrait").GetComponent<Image>();

                float EndTime;

                while (DialogueQueue.Count > 0)
                {
                    if (DialogueQueue[0].Messages.Count == 0)
                    {
                        DialogueQueue.Remove(DialogueQueue[0]);
                        continue;
                    }


                    Message = DialogueQueue[0].Messages[0];
                    Portrait.sprite = Message.CharacterSprite;
                    MessageBox.text = Message.Message;

                    yield return new WaitForSeconds(.5f);

                    EndTime = Time.time + WaitForInputTime;

                    yield return new WaitUntil(() => Time.time >= EndTime || AdvanceScript);

                    DialogueQueue[0].Messages.Remove(Message);
                }

                Destroy(DialogueCanvas);
                IsOpen = false;
                DialogueCoroutine = null;

            }
        }
    }
}