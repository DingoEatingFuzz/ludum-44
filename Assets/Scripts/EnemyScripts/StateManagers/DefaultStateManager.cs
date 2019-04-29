using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class DefaultStateManager : StateManager
{
    protected bool IsVisible
    {
        get => Data.Renderer.isVisible;
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
                Data.State = EnemyState.Engaged;
            } else
            {
                Data.State = EnemyState.Alerted;
            }
        } else
        {
            Data.State = EnemyState.Dormant;
        }
    }

    /// <summary>
    /// Checks if the first object in the direction of the player is the player
    /// </summary>
    /// <returns>True if the player is visible</returns>
    protected bool CanSeePlayer()
    {
        var RayStart = Data.Controller.transform.position;
        var RayEnd = Data.Player.transform.position;

        if (Physics.Raycast(RayStart, (RayEnd - RayStart).normalized, out RaycastHit Hit))
        {
            if (Hit.collider.gameObject == Data.Player)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Is this enemy on the player screen
    /// </summary>
}
