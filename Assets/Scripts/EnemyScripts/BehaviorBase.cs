using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Behavior : MonoBehaviour
{
    [Tooltip("During which state should this script run?")]
    public EnemyState Type;

    public bool IsRunning { get; protected set; }
    public bool IsAborting { get; protected set; }
    public bool IsRunningOrAborting()
    {
        return IsRunning || IsAborting;
    }

    public abstract void Run(GameObject Owner);
    public abstract void Abort();
}