using System;
using System.Collections;
using System.Collections.Generic;

public class Item {
  public string name;
  public int price;
}

public static class ItemDB
{
  public static Dictionary<string, Item> Weapons = new Dictionary<string, Item>() {
    { "laser", new Item { name = "Laser", price = 200 } },
    { "gatling", new Item { name = "Super Gatling", price = 300 } },
    { "plasma", new Item { name = "Plasma Cannon", price = 500 } },
  };

  public static Dictionary<string, Item> Upgrades = new Dictionary<string, Item>() {
    { "healthUp", new Item { name = "+ Max Health", price = 200 } },
    { "fireRateUp", new Item { name = "+ Fire Rate", price = 300 } },
    { "damageUp", new Item { name = "+ Damage", price = 400 } },
  };

  public static Dictionary<string, Item> Charity = new Dictionary<string, Item>() {
    { "food", new Item { name = "Food on the Table", price = 200 } },
    { "clothes", new Item { name = "New Cothes", price = 300 } },
    { "rent", new Item { name = "Rent", price = 500 } },
  };
}
