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

    public override void Abort()
    {
        IsAborting = true;
        StopCoroutine(Routine);
        Data.Weapon.StopFiring();
        IsAborting = false;
        IsRunning = false;
    }

    public override void Run()
    {
        IsRunning = true;
        HasRun = true;
        Routine = StartCoroutine(FireWeapon());
    }

    protected IEnumerator FireWeapon()
    {
        Data.Weapon.StartFiring();
        yield return new WaitForSeconds(FiringTime);
        Data.Weapon.StopFiring();
        yield return new WaitForSeconds(Cooldown);
        IsRunning = false;
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
