using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : Behavior
{
    protected Coroutine Routine;

    public override void Abort()
    {
        IsRunning = false;
    }

    public override void Run()
    {
        IsRunning = true;

        Data.Movement.LookAt = Data.Player;
        Routine = StartCoroutine(Waitbecausereasons());
    }

    protected IEnumerator Waitbecausereasons()
    {
        yield return new WaitForSeconds(1f);
        IsRunning = false;
    }

    protected override void SetHasAborted()
    {
        throw new System.NotImplementedException();
    }

    protected override void SetHasRun()
    {
        throw new System.NotImplementedException();
    }

    protected override void SetIsAborting(bool Aborting)
    {
        throw new System.NotImplementedException();
    }

    protected override void SetIsRunning(bool Running)
    {
        throw new System.NotImplementedException();
    }
}
