
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyControllerComponent : EnemyControllerComponent
{
    /// <summary>
    /// Awake
    /// </summary>
    protected void Awake()
    {
        State = Enemy.EnemyState.Dormant;
    }

    protected override void DoDormant()
    {

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
