using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public abstract class Behavior : MonoBehaviour
{
    [Tooltip("During which states should this script run?")]
    public List<EnemyState> StatesToRun;

    //[Tooltip("Abort between states to run?")]
    //public bool AbortOnStateChange = false;

    [Tooltip("Only execute this script one time")]
    public bool RunOnce = false;

    [HideInInspector]
    public EnemyData Data;

    protected bool HasRun = false;
    protected bool HasAborted = false;

    protected abstract void SetHasRun();
    protected abstract void SetHasAborted();
    protected abstract void SetIsRunning(bool Running);
    protected abstract void SetIsAborting(bool Aborting);

    public bool IsRunning { get; protected set; }
    public bool IsAborting { get; protected set; }
    public bool IsRunningOrAborting
    {
        get => IsRunning || IsAborting;
    }

    public abstract void Run();
    public abstract void Abort();
    public bool ShouldRun
    {
        get => (RunOnce && !HasRun && !IsRunningOrAborting) || (!RunOnce && !IsRunningOrAborting);
    }

    public bool ShouldAbort
    {
        get => (RunOnce && !HasAborted && !IsAborting) || (!RunOnce && !IsAborting);
    }
}