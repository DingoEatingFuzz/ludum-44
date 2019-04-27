using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Max speed of the ship")]
    [Range(0f, float.PositiveInfinity)]
    public float MoveSpeed = 10;

    [Tooltip("Speed of rotation")]
    [Range(0f, float.PositiveInfinity)]
    public float RotationRate = 360;

    [Tooltip("How quickly the ship goes from 0 to max in seconds")]
    [Range(0f, float.PositiveInfinity)]
    public float Thrust = .5f;

    private bool CanMove { get; set; }
    public Vector3 Velocity { get; protected set; }
    private float ChangeSpeed;

    private float RightAxis;
    private float UpAxis;

    //private IonCannon[] Weapons;


    /// <summary>
    /// Awake
    /// </summary>
    protected void Awake()
    {
        Velocity = Vector3.zero;
        ChangeSpeed = MoveSpeed / Thrust;

//        Weapons = GetComponents<IonCannon>();
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
        CanMove = true;
        enabled = true;
        SetWeaponsActive(true);
        //GetComponent<Collider>().isTrigger = false;
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
            Velocity = new Vector3(RightAxis, UpAxis, 0).normalized * MoveSpeed;

            UpdateMove();
            if (Velocity.magnitude != 0)
            {
                UpdateRotate();
            }
        }
    }

    /// <summary>
    /// Updates and applies velocity
    /// </summary>
    private void UpdateMove()
    {
        //var DesiredVelocity = new Vector3(RightAxis, UpAxis, 0).normalized;

        //Velocity = Vector3.MoveTowards(Velocity, DesiredVelocity * MoveSpeed, ChangeSpeed);
        transform.position = transform.position + Velocity * Time.deltaTime;
    }

    /// <summary>
    /// Updates and applies rotation
    /// </summary>
    private void UpdateRotate()
    {
        var rot = Quaternion.LookRotation(Velocity, transform.up);
        transform.rotation = rot;
      //  transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);


        // Smoothing is done in the input manager
        //var DesiredRotation = Input.GetAxis("Right") * RotationRate;
        //transform.Rotate(Vector3.up, DesiredRotation * Time.deltaTime);
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