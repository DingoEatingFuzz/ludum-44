using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon,
    Upgrade,
    Charity
}

[System.Serializable]
public class ItemLookup  {
    [Tooltip("Find the ItemKey in the ItemDB static class")]
    public string Key;
    public Sprite ItemSprite;
    public ItemType Type;
}

public class ShopItem : MonoBehaviour
{
    public int ItemPrice;
    public ItemType Type;

    [Tooltip("Leave blank to choose a random item of the Type")]
    public string ItemKey;
    public List<ItemLookup> AllSprites;

    // Start is called before the first frame update
    List<SpriteRenderer> delays = new List<SpriteRenderer>();
    IEnumerator purchaseDelay;
    Light indicatorLight;
    float baseIntensity;
    TextMesh priceText;
    SpriteRenderer careCoinSymbol;
    SpriteRenderer itemIcon;
    SpriteRenderer poorIcon;
    Sprite itemSprite;
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
        itemIcon = transform.Find("ItemIcon").GetComponent<SpriteRenderer>();
        poorIcon = transform.Find("poorIcon").GetComponent<SpriteRenderer>();
        poorIcon.gameObject.SetActive(false);
        available = true;

        // Attempt to fetch the provided item sprite
        Debug.Log("ItemKey: " + ItemKey);
        if (ItemKey != "") {
            var lookup = AllSprites.Find((ItemLookup l) => l.Type == Type && l.Key == ItemKey);
            itemSprite = lookup?.ItemSprite;
        }
        // If there is nothing set, for the set item is invalid, choose at random among the Type
        Debug.Log("ItemSprite: " + itemSprite);
        if (itemSprite == null) {
            var lookupsByType = AllSprites.FindAll((ItemLookup l) => l.Type == Type);
            itemSprite = lookupsByType[Mathf.FloorToInt(Random.Range(0, lookupsByType.Count))].ItemSprite;
            Debug.Log("No Sprite, chose one at random: " + itemSprite);
        }
        itemIcon.sprite = itemSprite;
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
                poorIcon.gameObject.SetActive(true);
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
        poorIcon.gameObject.SetActive(false);
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

        var color = careCoinSymbol.color;
        color.a = 0.25f;
        careCoinSymbol.color = color;

        color = priceText.color;
        color.a = 0.25f;
        priceText.color = color;

        color = itemIcon.color;
        color.a = 0.25f;
        itemIcon.color = color;

        transform.Find("Burst").GetComponent<ParticleSystem>()?.gameObject.SetActive(false);

    }
}
