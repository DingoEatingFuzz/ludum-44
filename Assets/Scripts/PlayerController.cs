using UnityEngine;
using System.Collections;

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
    public GameObject LaserGun;
    public GameObject GatlingGun;
    public GameObject PlasmaGun;

    private bool CanMove { get; set; }
    private Vector3 Direction;
    public Vector3 Velocity { get; protected set; }
    private float ChangeSpeed;

    private float RightAxis;
    private float UpAxis;

    /// <summary>
    /// Awake
    /// </summary>
    protected void Awake()
    {
        Velocity = Vector3.zero;
        Direction = transform.forward;
        ChangeSpeed = MoveSpeed / Thrust;

        if (LaserGun == null)
        {
            throw new UnassignedReferenceException("LaserGun has not been assigned");
        }

        if (GatlingGun == null)
        {
            throw new UnassignedReferenceException("GatlingGun has not been assigned");
        }

        if (PlasmaGun == null)
        {
            throw new UnassignedReferenceException("PlasmaGun has not been assigned");
        }

        ActivateWeapon(LaserGun);
    }

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        Activate();
    }

    protected void ActivateWeapon(GameObject Weapon)
    {
        Weapon?.SetActive(true);
    }

    /// <summary>
    /// Enables movement and firing
    /// </summary>
    void Activate()
    {
        CanMove = true;
        enabled = true;
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

        //if (Direction.z != 0f)
        //    Debug.LogWarning("DirectionZ is not zero!");
        //Debug.Log("Dot: " + Vector3.Dot(Direction, DesiredDirection));

        if (DesiredDirection != Direction)
        {
            // Speeds up turning larger distances
            var DirectionBoost = 2f - Vector3.Dot(Direction.normalized, DesiredDirection.normalized);

            // Aim to the side if 
            if (Mathf.Approximately(Vector3.Dot(Direction, DesiredDirection), -1f))
            {
                // Debug.Log("Using alt smoothing");
                Direction = Vector3.RotateTowards(Direction, Mathf.RoundToInt(Random.value) == 0 ? transform.right : -transform.right, 0.5f * Mathf.Deg2Rad * RotationRate * DirectionBoost * Time.deltaTime, 0f);
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
        var DesiredVelocity = transform.forward * MoveSpeed;
        if (AxisDirection == Vector3.zero)
        {
            DesiredVelocity = Vector3.zero;
        }
        var VelocityBoost = 2f - Vector3.Dot(CurrentVelocity.normalized, DesiredVelocity.normalized);

        CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, DesiredVelocity, ChangeSpeed * VelocityBoost * Time.deltaTime);

        // Move the ship
        transform.position += DesiredVelocity * Time.deltaTime;
    }
}