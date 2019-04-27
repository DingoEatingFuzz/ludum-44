using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Max speed of the ship")]
    [Range(0f, float.PositiveInfinity)]
    public float MoveSpeed = 70;

    [Tooltip("Speed of rotation")]
    [Range(0f, float.PositiveInfinity)]
    public float RotationRate = 360;

    [Tooltip("How quickly the ship goes from 0 to max in seconds")]
    [Range(0f, float.PositiveInfinity)]
    public float Thrust = .5f;

    private bool CanMove { get; set; }
    public Vector3 Velocity { get; protected set; }
    private float ChangeSpeed;
    protected Rigidbody RB;

    //private IonCannon[] Weapons;


    /// <summary>
    /// Awake
    /// </summary>
    protected void Awake()
    {
        Velocity = Vector3.zero;
        ChangeSpeed = MoveSpeed / Thrust;

//        Weapons = GetComponents<IonCannon>();
        RB = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
    }

    /// <summary>
    /// Enables movement and firing
    /// </summary>
    void Activate()
    {
        CanMove = true;
        enabled = true;
        SetWeaponsActive(true);
        GetComponent<Collider>().isTrigger = false;
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        if (CanMove)
        {
            UpdateMove();
            UpdateRotate();
        }

        if (RB.velocity.z != 0)
        {
            Debug.Log("Velocity: " + RB.velocity + "\nAngular: " + RB.angularVelocity);
        }
    }

    /// <summary>
    /// Updates and applies velocity
    /// </summary>
    private void UpdateMove()
    {
        var Axis = Input.GetAxis("Up");
        if (Axis == -1)
        {
            Axis = -0.5f;
        }

        var DesiredVelocity = transform.forward * (Axis * MoveSpeed);
        var Dot = Vector3.Dot(Velocity.normalized, DesiredVelocity.normalized);

        var TurnBoost = 1f + 2 * (1f - Mathf.Abs(Dot));
        var Slowdown = Axis == 0f ? 0.2f : 1f;

        Velocity = Vector3.MoveTowards(GetComponent<Rigidbody>().velocity, DesiredVelocity, ChangeSpeed * TurnBoost * Slowdown * Time.deltaTime);
        //transform.position = transform.position + Velocity * Time.deltaTime;
        GetComponent<Rigidbody>().velocity = Velocity;
    }

    /// <summary>
    /// Updates and applies rotation
    /// </summary>
    private void UpdateRotate()
    {
        // Smoothing is done in the input manager
        var DesiredRotation = Input.GetAxis("Right") * RotationRate;
        transform.Rotate(Vector3.up, DesiredRotation * Time.deltaTime);

        if (DesiredRotation != 0f)
        {
            RB.angularVelocity = Vector3.zero;
        }
    }

    void Deactivate()
    {
        // Notify GameManager
    }

    /// <summary>
    /// Changes weapons firing ability
    /// </summary>
    /// <param name="Active">Weapons can fire</param>
    void SetWeaponsActive(bool Active)
    {
//        foreach (var Weapon in Weapons)
//        {
//            Weapon.CanShoot = Active;
//        }
    }
}