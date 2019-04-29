using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class WeaponLookup  {
    [Tooltip("Find the WeapoonKey")]
    public string Key;
    public GameObject WeaponObject;
}

public class PlayerController : MonoBehaviour
{
    [Tooltip("Max speed of the ship")]
    [Range(0f, float.PositiveInfinity)]
    public float MoveSpeed = 10;

    [Tooltip("Speed of rotation")]
    [Range(0f, float.PositiveInfinity)]
    public float RotationRate = 300;

    [Tooltip("How quickly the ship goes from 0 to max in seconds")]
    [Range(0.001f, float.PositiveInfinity)]
    public float Thrust = .5f;

    [Header("Weapons")]
    public List<WeaponLookup> Weapons;
    public string ActiveWeaponName;

    [HideInInspector]
    public float FireRateModifier = 0;

    [HideInInspector]
    public float DamageModifier = 0;

    [Header("Debug")]
    public bool DoDebug = false;

    private bool CanMove { get; set; }
    private Vector3 Direction;
    public Vector3 Velocity { get; protected set; }
    private float ChangeSpeed;

    private float RightAxis;
    private float UpAxis;
    private AudioSource HornSource;

    /// <summary>
    /// Awake
    /// </summary>
    protected void Awake()
    {
        Velocity = Vector3.zero;
        Direction = transform.forward;
        ChangeSpeed = MoveSpeed / Thrust;
    }

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        Activate();
    }

    /// <summary>
    /// Enables movement and firing
    /// </summary>
    void Activate()
    {
        PersistentData StateData = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameStateComponent>().GetStoredData();
        ActiveWeaponName = ActiveWeaponName != null ? ActiveWeaponName : "laser";
        if (StateData.MaxHealth > 0)
        {
            DamageModifier = StateData.DamageModifier;
            FireRateModifier = StateData.FireRateModifier;
            ActiveWeaponName = StateData.ActiveWeaponName;

            HealthComponent PlayerHealth = gameObject.GetComponent<HealthComponent>();
            PlayerHealth.Maximum = StateData.MaxHealth;
            PlayerHealth.Set(StateData.CurrentHealth);
        }
        ActivateWeapon(ActiveWeaponName);

        CanMove = true;
        enabled = true;
        HornSource = gameObject.GetComponent<AudioSource>();

    }

    public void ActivateWeapon(string WeaponName)
    {
        if (WeaponName != "")
        {
            GameObject lookup = Weapons.Find((WeaponLookup w) => w.Key == WeaponName)?.WeaponObject.gameObject;
            if (lookup != null)
            {
                Weapons.Find((WeaponLookup w) => w.Key == ActiveWeaponName)?.WeaponObject.gameObject.SetActive(false);
                lookup.SetActive(true);
            }
            ActiveWeaponName = WeaponName;
        }
    }

    public void ActivateUpgrade(string UpgradeName)
    {
        switch (UpgradeName)
        {
            case "fireRateUp":
                FireRateModifier++;
                break;
            case "damageUp":
                DamageModifier++;
                break;
            case "healthUp":
                HealthComponent PlayerHealth = gameObject.GetComponent<HealthComponent>();
                PlayerHealth.Maximum = PlayerHealth.Maximum + 50;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        if (CanMove)
        {
            RightAxis = Input.GetAxis("Right");
            UpAxis = Input.GetAxis("Up");

            UpdateMoveTwoPointO();
        }

        if (Input.GetButton("SecondaryFire"))
        {
            HornSource.Play();
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameStateComponent>().StoreData();
            SceneManager.LoadScene("SampleScene");
        }
    }

    /// <summary>
    /// Updates and applies velocity
    /// </summary>
    private void UpdateMove()
    {

        transform.position = transform.position + Velocity * Time.deltaTime;

    }

    /// <summary>
    /// Better movement because Andrew likes it ~:::S M O O T H:::~
    /// </summary>
    protected void UpdateMoveTwoPointO()
    {
        #region Rotation

        // Smoothly update the rotate
        var AxisDirection = new Vector3(RightAxis, UpAxis, 0f).normalized;
        var DesiredDirection = AxisDirection == Vector3.zero ? Direction : AxisDirection;

        if (Mathf.Abs(Direction.z) < 0.001f)
        {
            Direction.z = 0f;
        }

        if (DoDebug)
        {
            if (Direction.z != 0f)
            {
                Debug.LogWarning("DirectionZ is not zero!");
            }
            Debug.Log("Dot: " + Vector3.Dot(Direction, DesiredDirection));
            Debug.Log("Direction: " + Direction + " :: Desired: " + DesiredDirection);
        }

        if (DesiredDirection != Direction)
        {
            // Speeds up turning larger distances
            var DirectionBoost = 2f - Vector3.Dot(Direction.normalized, DesiredDirection.normalized);

            // ~~~ If there's a problem with it flipping derpy it's probably because of this ~~~
            if (Vector3.Dot(Direction, DesiredDirection) < -.99f)
            {
                if (DoDebug)
                {
                    Debug.Log("Using alt smoothing");
                }
                Direction = Vector3.RotateTowards(Direction, Mathf.RoundToInt(Random.value) == 0 ? transform.right : -transform.right, Mathf.Deg2Rad * RotationRate * DirectionBoost * Time.deltaTime, 0f);
            }
            else
            {
                Direction = Vector3.RotateTowards(Direction, DesiredDirection, Mathf.Deg2Rad * RotationRate * DirectionBoost * Time.deltaTime, 0f);
            }
        }

        // Rotate to axis direction
        transform.rotation = Quaternion.LookRotation(Direction, transform.up);

        #endregion

        // Smoothly update the velocity
        var CurrentVelocity = Velocity;
        var DesiredVelocity = AxisDirection * MoveSpeed;
        if (AxisDirection == Vector3.zero)
        {
            DesiredVelocity = Vector3.zero;
        }
        var VelocityBoost = 2f - Vector3.Dot(CurrentVelocity.normalized, DesiredVelocity.normalized);

        CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, DesiredVelocity, ChangeSpeed * VelocityBoost * Time.deltaTime);

        // Move the ship
        transform.position += DesiredVelocity * Time.deltaTime;
    }

    public void AddUpgrade(string placeholder)
    {
        Debug.Log("Purchased " + placeholder);

    }
}
