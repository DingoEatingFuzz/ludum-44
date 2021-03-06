﻿using System.Collections;
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
    Item item;
    int itemPrice;
    string lookupKey;

    void Start()
    {
        delays.Add(transform.Find("delay1").GetComponent<SpriteRenderer>());
        delays.Add(transform.Find("delay2").GetComponent<SpriteRenderer>());
        delays.Add(transform.Find("delay3").GetComponent<SpriteRenderer>());
        ResetDelays();
        indicatorLight = transform.Find("light").GetComponent<Light>();
        baseIntensity = indicatorLight.intensity;

        careCoinSymbol = transform.Find("CareCoinSymbol").GetComponent<SpriteRenderer>();
        itemIcon = transform.Find("ItemIcon").GetComponent<SpriteRenderer>();
        poorIcon = transform.Find("poorIcon").GetComponent<SpriteRenderer>();
        poorIcon.gameObject.SetActive(false);
        available = true;

        // Attempt to fetch the provided item sprite
        ItemLookup lookup = AllSprites[0];

        if (ItemKey != "") {
            lookup = AllSprites.Find((ItemLookup l) => l.Type == Type && l.Key == ItemKey);
            itemSprite = lookup?.ItemSprite;
        }

        // If there is nothing set, for the set item is invalid, choose at random among the Type
        if (itemSprite == null) {
            var lookupsByType = AllSprites.FindAll((ItemLookup l) => l.Type == Type);
            lookup = lookupsByType[Mathf.FloorToInt(Random.Range(0, lookupsByType.Count))];
            itemSprite = lookup.ItemSprite;
        }
        itemIcon.sprite = itemSprite;
        lookupKey = lookup.Key;

        switch(Type) {
            case ItemType.Weapon:
                item = ItemDB.Weapons[lookupKey];
                break;
            case ItemType.Upgrade:
                item = ItemDB.Upgrades[lookupKey];
                break;
            case ItemType.Charity:
                item = ItemDB.Charity[lookupKey];
                break;
        }

        itemPrice = item.price;

        priceText = transform.Find("price").GetComponent<TextMesh>();
        priceText.text = itemPrice + "cc";
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
            if (other.gameObject.GetComponent<HealthComponent>().Current >= itemPrice)
            {
                purchaseDelay = BuyButSlowlyJustInCaseThePlayerChangesTheirMindOrDoesNotRealizeWhatIsHappening(other.gameObject);
                StartCoroutine(purchaseDelay);
                LightOn();
            } else
            {
                poorIcon.gameObject.SetActive(true);
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

        switch(Type) {
            case ItemType.Weapon:
                player.GetComponent<PlayerController>().ActivateWeapon(lookupKey);
                break;
            case ItemType.Upgrade:
                player.GetComponent<PlayerController>().ActivateUpgrade(lookupKey);
                //item = ItemDB.Upgrades[lookupKey];
                break;
            case ItemType.Charity:
                //item = ItemDB.Charity[lookupKey];
                break;
        }
        player.GetComponent<HealthComponent>().Remove(itemPrice);

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
