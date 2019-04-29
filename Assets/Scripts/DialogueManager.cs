using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject DialogueCanvas;
    bool AwaitingConfirmation = true;
    bool isOpen = false;

    public bool IsOpen {
        get {
            return isOpen;
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (AwaitingConfirmation && Input.GetButton("PayRespects")) {
            AwaitingConfirmation = false;
        };
    }

    public IEnumerator Write(string Message) {
        var dialogue = Instantiate(DialogueCanvas);

        var textBox = dialogue.transform.Find("Speech").GetComponent<Text>();
        textBox.text = Message;

        // Immediately set the dialogue to the open state
        isOpen = true;
        yield return new WaitForSeconds(0.5f);

        // After a grace period, allow for confirmation
        // (to avoid accientally confirming during the same button press)
        AwaitingConfirmation = true;
        yield return new WaitUntil(() => !AwaitingConfirmation);

        // Destroy the instantiated dialogue and reset the manager state
        Destroy(dialogue);
        isOpen = false;
    }
}
