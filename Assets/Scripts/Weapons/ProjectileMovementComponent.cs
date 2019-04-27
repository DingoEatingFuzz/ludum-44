using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ProjectileMovementComponent : MonoBehaviour
{
    [Tooltip("Speed in u/s the projectile moves")]
    public float Speed;

    [Tooltip("Time before projectile despawns")]
    public float SecondsToLast;

    protected Vector3 Velocity;

    /// <summary>
    /// Awake
    /// </summary>
    protected void Awake()
    {
        Velocity = transform.forward * Speed;
        Invoke("Despawn", SecondsToLast);
    }

    // TODO: Break movement into an overloadable function
    /// <summary>
    /// For now it just moves the projectile forward
    /// </summary>
    protected void Update()
    {
        transform.position += Velocity;
    }

    protected void Despawn()
    {
        Destroy(gameObject);
    }
}
