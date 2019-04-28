using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public int ItemPrice;

    // Start is called before the first frame update
    List<SpriteRenderer> delays = new List<SpriteRenderer>();
    IEnumerator purchaseDelay;
    Light indicatorLight;
    float baseIntensity;
    TextMesh priceText;
    SpriteRenderer careCoinSymbol;
    SpriteRenderer goneIcon;
    bool available;

    void Start()
    {
        delays.Add(transform.Find("delay1").GetComponent<SpriteRenderer>());
        delays.Add(transform.Find("delay2").GetComponent<SpriteRenderer>());
        delays.Add(transform.Find("delay3").GetComponent<SpriteRenderer>());
        ResetDelays();
        indicatorLight = transform.Find("light").GetComponent<Light>();
        baseIntensity = indicatorLight.intensity;

        priceText = transform.Find("price").GetComponent<TextMesh>();
        priceText.text = ItemPrice + "cc";

        careCoinSymbol = transform.Find("CareCoinSymbol").GetComponent<SpriteRenderer>();
        goneIcon = transform.Find("goneIcon").GetComponent<SpriteRenderer>();
        goneIcon.gameObject.SetActive(false);
        available = true;
    }

    void ResetDelays() {
        delays.ForEach((SpriteRenderer delay) => delay.enabled = false);
    }

    void LightOn() {
        indicatorLight.intensity = baseIntensity * 2;
    }

    void LightOff() {
        indicatorLight.intensity = baseIntensity;
    }

    void OnTriggerEnter(Collider other)
    {
        if (available && other.tag == "Player")
        {
            if (other.gameObject.GetComponent<HealthComponent>().Current >= ItemPrice)
            {
                purchaseDelay = BuyButSlowlyJustInCaseThePlayerChangesTheirMindOrDoesNotRealizeWhatIsHappening(other.gameObject);
                StartCoroutine(purchaseDelay);
                LightOn();
            } else
            {
                Debug.Log("Not enough moneys");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && purchaseDelay != null) {
            StopCoroutine(purchaseDelay);
            ResetDelays();
            LightOff();
        }
    }

    IEnumerator BuyButSlowlyJustInCaseThePlayerChangesTheirMindOrDoesNotRealizeWhatIsHappening(GameObject player) {
        foreach (var delay in delays)
        {
            yield return new WaitForSeconds(0.5f);
            delay.enabled = true;
        }
        Debug.Log("Buy the thing!");
        player.GetComponent<HealthComponent>().Remove(ItemPrice);
        player.GetComponent<PlayerController>().AddUpgrade("placeholder");
        available = false;
        goneIcon.gameObject.SetActive(true);
        priceText.text = "";
        var ccsColor = careCoinSymbol.color;
        ccsColor.a = 0.0f;
        careCoinSymbol.color = ccsColor;
    }
}
