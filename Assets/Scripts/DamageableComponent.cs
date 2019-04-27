using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageData
{
    public GameObject Instigator;
    public float Amount;
    // type of damage
}

[RequireComponent(typeof(HealthComponent))]
public class DamageableComponent : MonoBehaviour
{
    public delegate void HandleDamage(object Sender, DamageData Data);
    public HandleDamage RaiseDamage;

    protected HealthComponent Health;

    /// <summary>
    /// Awake
    /// </summary>
    protected void Awake()
    {
        Health = GetComponent<HealthComponent>();
    }

    /// <summary>
    /// Damages objects health
    /// </summary>
    /// <param name="Instigator">Object ultimately responsible for the damage</param>
    /// <param name="Amount">Amount of damage to deal</param>
    public void Damage(GameObject Instigator, float Amount /*damage type*/)
    {
        // react to different damage differently e.g., resistance

        var Removed = Health.Remove(Amount);
        RaiseDamage(gameObject, new DamageData() { Instigator = Instigator, Amount = Removed });
        if (Health.IsDepleted)
        {
            GetComponent<DeathComponent>()?.Died(Instigator);
        }
    }
}
