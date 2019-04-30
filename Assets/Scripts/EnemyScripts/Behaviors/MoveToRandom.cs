using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Wanders around near it's starting position
/// </summary>
[RequireComponent(typeof(EnemyMovementComponent))]
public class MoveToRandom : Behavior
{
    public float WanderRange = 20f;

    protected Vector3 StartingPosition;

    protected void Start()
    {
        StartingPosition = Data.Self.transform.position;
        Data.Movement.RaiseArrived += MoveFinished;
    }

    private void MoveFinished(object Sender)
    {
        IsRunning = false;
    }

    public override void Abort()
    {
    }

    public override void Run()
    {
        IsRunning = true;
        Data.Movement.MoveTarget = Random.insideUnitCircle * WanderRange;
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
