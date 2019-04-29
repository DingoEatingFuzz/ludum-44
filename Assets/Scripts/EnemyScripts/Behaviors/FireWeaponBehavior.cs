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
        IsRunning = false;  
    }

    public override void Run(GameObject Owner, EnemyControllerComponent Controller)
    {
        IsRunning = true;
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
}
