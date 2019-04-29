using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementComponent : MonoBehaviour
{
    public delegate void HandleArrived(object Sender);
    public HandleArrived RaiseArrived;

    public float Movespeed = 8f;

    protected Vector3 _MoveTarget;
    public Vector3 MoveTarget
    {
        get => _MoveTarget;
        set
        {
            if (value == _MoveTarget)
            {
                return;
            }

            _MoveTarget = value;
            DoMove = true;
        }
    }

    [HideInInspector]
    public GameObject LookAt;

    protected bool DoMove;

    // Update is called once per frame
    void Update()
    {
        var Position = transform.position;

        if (DoMove)
        {
            var Difference = Position - MoveTarget;
            var MoveDirection = Difference.normalized;
        
            if (Difference.magnitude < Movespeed)
            {
                transform.position += MoveDirection * Movespeed;
            } else
            {
                RaiseArrived?.Invoke(this);
                DoMove = false;
            }
        }

        if (LookAt != null)
        {
            var LookDirection = Position - LookAt.transform.position;
            LookDirection.Normalize();
            var Rotation = Quaternion.LookRotation(LookDirection, transform.up);
        }
    }


}
