using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    // Start is called before the first frame update
    List<SpriteRenderer> delays = new List<SpriteRenderer>();
    IEnumerator purchaseDelay;
    Light light;
    float baseIntensity;

    void Start()
    {
        delays.Add(transform.Find("delay1").GetComponent<SpriteRenderer>());
        delays.Add(transform.Find("delay2").GetComponent<SpriteRenderer>());
        delays.Add(transform.Find("delay3").GetComponent<SpriteRenderer>());
        ResetDelays();
        light = transform.Find("light").GetComponent<Light>();
        baseIntensity = light.intensity;
    }

    void ResetDelays() {
        delays.ForEach((SpriteRenderer delay) => delay.enabled = false);
    }

    void LightOn() {
        light.intensity = baseIntensity * 2;
    }

    void LightOff() {
        light.intensity = baseIntensity;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            purchaseDelay = BuyButSlowlyJustInCaseThePlayerChangesTheirMindOrDoesNotRealizeWhatIsHappening();
            StartCoroutine(purchaseDelay);
            LightOn();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") {
            StopCoroutine(purchaseDelay);
            ResetDelays();
            LightOff();
        }
    }

    IEnumerator BuyButSlowlyJustInCaseThePlayerChangesTheirMindOrDoesNotRealizeWhatIsHappening() {
        foreach (var delay in delays) {
            yield return new WaitForSeconds(0.5f);
            delay.enabled = true;
        }
        Debug.Log("Buy the thing!");
    }
}
