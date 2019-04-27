using Management.Persistent;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;









public class GameStateComponent : MonoBehaviour
{
    public static GameStateComponent Instance;

    protected PersistentData Data;

    /// <summary>
    /// Awake
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
    /// Capies 
    /// </summary>
    public void PrepareData(PersistentData Data, System.Action Callback)
    {
        this.Data = Data;
    }
}

namespace Management.Persistent
{
    /// <summary>
    /// Contains stuff that sticks around between levels
    /// </summary>
    [System.Serializable]
    public struct PersistentData
    {

    }
}
