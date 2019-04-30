using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DeathComponent : MonoBehaviour
{
    public AudioClip DeathSound;
    public delegate void HandleDied();
    public HandleDied RaiseDied;
    private AudioSource AudioSource;

    [Tooltip("Because what's a death component without the ability to not die")]
    public bool CanDie = true;

    /// <summary>
    /// Indicate the object has died
    /// </summary>
    /// <param name="Instigator">Obeject ultimately responsible for the death</param>
    ///
    public void Died(GameObject Instigator)
    {
        AudioSource = GetComponent<AudioSource>();
        if (CanDie)
        {
            AudioSource.PlayOneShot(DeathSound);
            RaiseDied?.Invoke();
        }
    }
}
