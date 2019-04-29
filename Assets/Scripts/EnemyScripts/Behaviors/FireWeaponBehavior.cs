using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWeaponBehavior : Behavior
{
    [Tooltip("How long the firing burst should last")]
    public float FiringTime = 1f;

    [Tooltip("How long until it can fire again")]
    public float Cooldown = 1f;

    protected Coroutine Routine;

    protected bool IsFiring;
    protected EnemyControllerComponent Controller;

    public override void Abort()
    {
        StopFiring(false);
        StopAllCoroutines();
        SetIsRunning(false);
        SetHasAborted();
    }

    public override void Run(GameObject Owner, EnemyControllerComponent Controller)
    {
        SetIsRunning(true);
        SetHasRun();
        this.Controller = Controller;
        StartFiring();
    }

    protected void StartFiring(bool StartRoutine = true)
    {
        IsFiring = true;
        Controller.Weapon.StartFiring();
        if (StartRoutine)
            Routine = StartCoroutine(DeferStopFiring()); 
    }

    protected IEnumerator DeferStopFiring()
    {
        yield return new WaitForSeconds(FiringTime);
        StopFiring();
    }

    private void StopFiring(bool StartRoutine = true)
    {
        IsFiring = false;
        Controller.Weapon.StopFiring();
        if (StartRoutine)
            Routine = StartCoroutine(DeferStartFiring());
    }

    protected IEnumerator DeferStartFiring()
    {
        yield return new WaitForSeconds(Cooldown);
        StartFiring();
    }

    protected override void SetHasRun()
    {
        HasRun = true;
    }

    protected override void SetHasAborted()
    {
        HasAborted = true;
    }

    protected override void SetIsRunning(bool Running)
    {
        IsRunning = Running;
    }

    protected override void SetIsAborting(bool Aborting)
    {
        IsAborting = Aborting;
    }
}
