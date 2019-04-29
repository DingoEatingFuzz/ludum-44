using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class DefaultStateManager : StateManager
{
    protected GameObject Player;

    public DefaultStateManager(EnemyControllerComponent Controller) : base(Controller)
    {
    }

    /// <summary>
    /// Awake
    /// </summary>
    protected void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("player");
        if (Player == null)
        {
            throw new MissingReferenceException("Couldn't find the player");
        }
    }

    /// <summary>
    /// CheckState
    /// </summary>
    public override void CheckState()
    {
        if (IsVisible)
        {
            if (CanSeePlayer())
            {
                State = EnemyState.Engaged;
            } else
            {
                State = EnemyState.Alerted;
            }
        } else
        {
            State = EnemyState.Dormant;
        }
    }

    /// <summary>
    /// Checks if the first object in the direction of the player is the player
    /// </summary>
    /// <returns>True if the player is visible</returns>
    protected bool CanSeePlayer()
    {
        var RayStart = Controller.Weapon.ProjectileSpawnLocation.transform.position;
        var RayEnd = Player.transform.position;

        if (Physics.Raycast(RayStart, (RayEnd - RayStart).normalized, out RaycastHit Hit))
        {
            if (Hit.collider.gameObject == Player)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Is this enemy on the player screen
    /// </summary>
    protected bool IsVisible
    {
        get => Controller.Renderer.isVisible;
    }
}
