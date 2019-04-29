using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Damage
{
    public float Amount = 10f;
    public float DamageIncreaseModifier = 1f;
    public bool DestroyOnHit = false;
}

public class DamagerComponent : MonoBehaviour
{
    [Tooltip("Type of damage to deal")]
    public Damage DamageData;

    [Tooltip("Object ultimately responsible for this damager")]
    public GameObject Instigator;

    /// <summary>
    /// Applys damage to the triggering component if they have a damageable component
    /// </summary>
    /// <param name="other"></param>
    protected void OnTriggerEnter(Collider other)
    {
        var Damageable = other.GetComponent<DamageableComponent>();
        if (Damageable != null && other.gameObject != Instigator)
        {
            var Player = Instigator.GetComponent<PlayerController>();
            var BonusDamage = (Player == null ? 0 : Player.DamageModifier) * DamageData.DamageIncreaseModifier;
            Damageable.Damage(Instigator, DamageData.Amount + BonusDamage);
            if (DamageData.DestroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
