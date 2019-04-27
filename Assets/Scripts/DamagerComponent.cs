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

    
}
