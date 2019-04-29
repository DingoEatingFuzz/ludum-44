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
[RequireComponent(typeof(Rigidbody))]
[System.Serializable]
public class DamageableComponent : MonoBehaviour
{
    public AudioClip DamageSound;
    public delegate void HandleDamage(object Sender, DamageData Data);
    public HandleDamage RaiseDamage;

    protected HealthComponent Health;
    protected DeathComponent Death;

    private AudioSource AudioSource;

    /// <summary>
    /// Awake
    /// </summary>
    protected void Awake()
    {
        Health = GetComponent<HealthComponent>();
        Death = GetComponent<DeathComponent>();
        AudioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Damages objects health
    /// </summary>
    /// <param name="Instigator">Object ultimately responsible for the damage</param>
    /// <param name="Amount">Amount of damage to deal</param>
    public void Damage(GameObject Instigator, float Amount /*damage type*/)
    {
        AudioSource.PlayOneShot(DamageSound);
        // react to different damage differently e.g., resistance
        var Removed = Health.Remove(Amount);
        RaiseDamage?.Invoke(gameObject, new DamageData() { Instigator = Instigator, Amount = Removed });
        if (Health.IsDepleted)
        {
            Death?.Died(Instigator);
        }
    }
}
