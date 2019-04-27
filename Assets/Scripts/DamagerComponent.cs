using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Damage
{
    public float Amount;
}

public class DamagerComponent : MonoBehaviour
{
    [Tooltip("Type of damage to deal")]
    public Damage DamageData;

    [Tooltip("Object ultimately responsible for this damager")]
    public GameObject Instigator;

    protected void OnTriggerEnter(Collider Other)
    {
        Other.GetComponent<DamageableComponent>()?.Damage(Instigator, DamageData.Amount);
    }
}
