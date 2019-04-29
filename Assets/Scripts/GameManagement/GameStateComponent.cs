using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateComponent : MonoBehaviour
{
    public static GameStateComponent Instance;

    protected PersistentData Data;

    /// <summary>
    /// Makes sure there is only ever one instance
    /// </summary>
    protected void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    public void StoreData()
    {
        PersistentData data = new PersistentData(GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>());
        this.Data = data;
    }

    public PersistentData GetStoredData()
    {
        return this.Data;
    }

}


/// <summary>
/// Contains stuff that sticks around between levels
/// </summary>
[System.Serializable]
public struct PersistentData
{
    public float CurrentHealth;
    public float MaxHealth;
    public string ActiveWeaponName;

    //powerups

    public PersistentData(PlayerController Player)
    {
        HealthComponent PlayerHealth = Player.gameObject.GetComponent<HealthComponent>();
        CurrentHealth = PlayerHealth.Current;
        MaxHealth = PlayerHealth.Maximum;
        ActiveWeaponName = Player.ActiveWeaponName;
    }
}

