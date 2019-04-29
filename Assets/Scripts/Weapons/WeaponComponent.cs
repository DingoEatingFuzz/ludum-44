using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WeaponComponent : MonoBehaviour
{
    [Tooltip("Where this weapon spawns its projectiles")]
    public GameObject ProjectileSpawnLocation;

    [Tooltip("Projectile to spawn")]
    public GameObject ProjectileType;

    [Tooltip("Delay between shots, in seconds")]
    public float ShotDelay = 0.5f;

    [Tooltip("Shot delay decrease per FireRate upgrade")]
    public float FireRateDecrease = 0.2f;

    [Tooltip("The amount of distance from the ProjectileSpawnLocation a projectile can spawn")]
    public float SpawnJitter = 0.0f;

    [Tooltip("The amount in degrees a projectile can devaite from perfect accuracy")]
    public float Inaccuracy = 0.0f;

    [Tooltip("How much health you lost with each shot")]
    public float HealthCost = 2.0f;

    public AudioClip firingSound;

    private float Cooldown = 0.0f;
    private bool CooldownIsActive = false;
    private HealthComponent PlayerHealth;
    private PlayerController Player;
    private AudioSource AudioSource;
    /// <summary>
    /// Awake
    /// </summary>
    protected void Awake()
    {
        gameObject.SetActive(false);
        Player = gameObject.transform.parent.gameObject.GetComponent<PlayerController>();
        PlayerHealth = gameObject.transform.parent.gameObject.GetComponent<HealthComponent>();
        AudioSource = GetComponent<AudioSource>();

        if (ProjectileSpawnLocation == null)
        {
            throw new UnassignedReferenceException("Did you forget to specify the spawn location?");
        }

        if (ProjectileType == null)
        {
            throw new UnassignedReferenceException("No projectile specified");
        }
    }

    public void Shoot() {
        PlayerHealth.Remove(HealthCost);
        var BasePosition = ProjectileSpawnLocation.transform.position;
        var Offset = new Vector3(Random.Range(0, SpawnJitter) - SpawnJitter/2, Random.Range(0, SpawnJitter) - SpawnJitter/2, 0);

        var BaseRotation = transform.rotation;
        var RotationOffset = Quaternion.Euler(Random.Range(0, Inaccuracy) - Inaccuracy/2, Random.Range(0, Inaccuracy) - Inaccuracy/2, 0);
        var Projectile = Instantiate(ProjectileType, BasePosition + Offset, BaseRotation * RotationOffset);
        Projectile.GetComponent<DamagerComponent>().Instigator = gameObject.transform.parent.gameObject;
        // Start the cooldown
        CooldownIsActive = true;
        AudioSource.Play();
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            UpdateRotate();
            // Track time elapsed since cooldown started
            if (CooldownIsActive)
            {
                Cooldown += Time.deltaTime;
            }

            // Reset cooldown states
            if (Cooldown > (ShotDelay - (Player.FireRateModifier * FireRateDecrease)))
            {
                Cooldown = 0.0f;
                CooldownIsActive = false;
            }

            if (!CooldownIsActive && Input.GetButton("PrimaryFire"))
            {
                // Allow for shooting
                Shoot();
            }
        }
    }

    void UpdateRotate()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;
        Quaternion rot = Quaternion.LookRotation(mousePos - transform.position, transform.up);
        transform.rotation = rot;
    }
}
