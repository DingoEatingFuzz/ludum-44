using Enemy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public enum EnemyState
    {
        Dormant,
        Alerted,
        Engaged,
        Retreating
    }
}

[RequireComponent(typeof(SphereCollider))]
public abstract class EnemyControllerComponent : MonoBehaviour
{
    protected EnemyState State;


    /// <summary>
    /// Update
    /// </summary>
    protected virtual void Update()
    {
        switch (State)
        {
            case EnemyState.Dormant:
                DoDormant();
                break;
            case EnemyState.Alerted:
                DoAlerted();
                break;
            case EnemyState.Engaged:
                DoEngaged();
                break;
            case EnemyState.Retreating:
                DoRetreating();
                break;
            default:
                break;
        }
    }

    protected abstract void DoAlerted();

    protected abstract void DoRetreating();

    protected abstract void DoEngaged();

    protected abstract void DoDormant();
}
