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

    /// <summary>
    /// Copies persistent data
    /// </summary>
    public void StoreData(PersistentData Data, System.Action Callback)
    {
        this.Data = Data;
        Callback?.Invoke();
    }
}


/// <summary>
/// Contains stuff that sticks around between levels
/// </summary>
[System.Serializable]
public struct PersistentData
{
    // health etc
}

