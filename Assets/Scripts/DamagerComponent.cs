using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Damage
{
    public float Amount = 10f;
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
        other.GetComponent<DamageableComponent>()?.Damage(Instigator, DamageData.Amount);
    }
}
