
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyControllerComponent : EnemyControllerComponent
{

    protected SpriteRenderer SpriteRenderer;

    /// <summary>
    /// Awake
    /// </summary>
    protected void Awake()
    {
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (SpriteRenderer == null)
        {
            throw new UnassignedReferenceException("Couldn't find a sprite render");
        }

        State = EnemyState.Dormant;
    }

    protected override void DoDormant()
    {
        // Regen health
    }

    protected override void DoAlerted()
    {
        throw new System.NotImplementedException();
    }

    protected override void DoEngaged()
    {
        throw new System.NotImplementedException();
    }

    protected override void DoRetreating()
    {
        throw new System.NotImplementedException();
    }
}
