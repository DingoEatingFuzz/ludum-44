using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PortraitPair {
    public string Key;
    public Sprite Portrait;
}

public class DialogueManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject DialogueCanvas;
    public List<PortraitPair> Portraits;

    bool AwaitingConfirmation = true;
    public int WaitFrames = 0;
    bool TimeUp = false;
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
            WaitFrames = 0;
        };
        if (WaitFrames < 800)
        {
            WaitFrames++;
        }
        else
        {
            AwaitingConfirmation = false;
            WaitFrames = 0;
        }
    }

    public IEnumerator Write(string Message, string Character) {
        var dialogue = Instantiate(DialogueCanvas);
        var characterSprite = CharacterSpriteFor(Character);

        var textBox = dialogue.transform.Find("Speech").GetComponent<Text>();
        textBox.text = Message;

        var portrait = dialogue.transform.Find("Portrait").GetComponent<Image>();
        portrait.sprite = characterSprite;

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

    Sprite CharacterSpriteFor(string Character) {
 //       Debug.Log(Character);
//        Debug.Log(Portraits);
        return Portraits.Find(p => p.Key == Character)?.Portrait;
    }
}
