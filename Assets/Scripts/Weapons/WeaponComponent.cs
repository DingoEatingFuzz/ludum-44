using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    [Tooltip("Where this weapon spawns it's projectiles")]
    public GameObject ProjectileSpawnLocation;

    [Tooltip("Projectile to spawn")]
    public GameObject ProjectileType;

    /// <summary>
    /// Awake
    /// </summary>
    protected void Awake()
    {
        if (ProjectileSpawnLocation == null)
        {
            throw new UnassignedReferenceException("Did you forget to specify the spawn location?");
        }

        if (ProjectileType == null)
        {
            throw new UnassignedReferenceException("No projectile specified");
        }
    }
}
