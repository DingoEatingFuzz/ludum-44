using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyWeaponComponent : MonoBehaviour
{
    [Tooltip("Minimum time between shots")]
    public float FiringInterval = .5f;

    [Tooltip("Size of fuzzy area around the spawn location")]
    public float Jitter = 0f;

    [Tooltip("Spread of bullets in degrees\nNote: E.g., 60 means 30 off center in both directions")]
    public float Spread = 0f;

    [Tooltip("Which projectile to shoot")]
    public GameObject ProjectileType;

    [Tooltip("Audio to play when weapon fires")]
    public AudioClip FiringSound;

    /// <summary>
    /// True if the gun is being fired
    /// </summary>
    public bool IsFiring { get; protected set; }

    protected bool CanFire;
    protected float NextShotTime;
    protected AudioSource AudioSource;

    /// <summary>
    /// Awake
    /// </summary>
    protected void Awake()
    {
        CanFire = false;
        AudioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Update
    /// </summary>
    protected void Update()
    {
        if (CanFire && Time.time >= NextShotTime)
        {
            Fire();
        }
    }

    /// <summary>
    /// Starts the gun firing
    /// </summary>
    public void StartFiring()
    {
        CanFire = true;
    }

    /// <summary>
    /// Stops the gun from firing
    /// </summary>
    public void StopFiring()
    {
        CanFire = false;
    }

    /// <summary>
    /// Fires the gun once
    /// </summary>
    public void Fire()
    {
        NextShotTime = Time.time + FiringInterval;
        var SpawnLoction = transform.position;
        var SpawnRotation = transform.rotation;
        var LocationOffset = new Vector3(Random.Range(0f, Jitter) - Jitter / 2f, Random.Range(0f, Jitter) - Jitter / 2f, 0f);
        var RotationOffset = Quaternion.AngleAxis(Random.Range(-Spread, Spread), transform.up);
        var Projectile = Instantiate(ProjectileType, SpawnLoction + LocationOffset, SpawnRotation * RotationOffset);
        Projectile.GetComponent<DamagerComponent>().Instigator = gameObject;
        AudioSource.PlayOneShot(FiringSound);
    }
}
