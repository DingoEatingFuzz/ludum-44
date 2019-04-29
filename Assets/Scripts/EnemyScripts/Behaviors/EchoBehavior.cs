using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoBehavior : Behavior
{
    public string RunMessage;
    public string AbortMessage;

    public override void Abort()
    {
        SetIsAborting(true);
        Echo(AbortMessage);
        SetIsAborting(false);
        SetHasAborted();
    }

    public override void Run(GameObject Owner, EnemyControllerComponent Controller)
    {
        SetIsRunning(true);
        SetHasRun();
        Echo(RunMessage);
        SetIsRunning(false);
    }

    protected void Echo(string Message)
    {
        Debug.Log("[[[ ECHO: " + Message + " ]]]");
    }

    protected override void SetHasAborted()
    {
        HasAborted = true;
    }

    protected override void SetHasRun()
    {
        HasRun = true;
    }

    protected override void SetIsAborting(bool Aborting)
    {
        IsAborting = Aborting;
    }

    protected override void SetIsRunning(bool Running)
    {
        IsRunning = Running;
    }
}
