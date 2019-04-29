using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Figures out what state the controller should be in
/// </summary>
[System.Serializable]
public abstract class StateManager : MonoBehaviour
{
    [HideInInspector]
    public EnemyControllerComponent Controller;

    protected EnemyState State
    {
        get => Controller.State;
        set => Controller.State = value;
    }

    public abstract void CheckState();
}
