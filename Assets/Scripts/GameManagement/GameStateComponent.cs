using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        var currentSceneName = SceneManager.GetActiveScene().name;
        var nextSceneName = this.Data.NextSceneName == null ? "Level2" : this.Data.NextSceneName;
        if (currentSceneName.Contains("Level"))
        {
            int sceneNum = int.Parse(currentSceneName.Substring(currentSceneName.LastIndexOf("l") + 1)) + 1;
            nextSceneName = "Level" + sceneNum;
        }

        PersistentData data = new PersistentData(GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>(), nextSceneName);
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
    //public int currentLevel;
    public float CurrentHealth;
    public float MaxHealth;
    public float DamageModifier;
    public float FireRateModifier;
    public string ActiveWeaponName;
    public string NextSceneName;


    //powerups

    public PersistentData(PlayerController Player, String SceneName)
    {
        HealthComponent PlayerHealth = Player.gameObject.GetComponent<HealthComponent>();
        CurrentHealth = PlayerHealth.Current;
        MaxHealth = PlayerHealth.Maximum;
        DamageModifier = Player.DamageModifier;
        FireRateModifier = Player.FireRateModifier;
        ActiveWeaponName = Player.ActiveWeaponName;
        NextSceneName = SceneName;
    }
}

