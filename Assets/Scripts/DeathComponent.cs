using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathComponent : MonoBehaviour
{
    [Tooltip("Because what's a death component without the ability to not die")]
    public bool CanDie = true;

    /// <summary>
    /// Indicate the object has died
    /// </summary>
    /// <param name="Instigator">Obeject ultimately responsible for the death</param>
    public void Died(GameObject Instigator)
    {
        Debug.LogWarning("No implementation for Died");
    }
}
