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
        
        var Position = gameObject.transform.position;
        if (DoMove)
        {
            var Difference = MoveTarget - Position;
            var MoveDirection = Difference;
            MoveDirection.z = 0f;
            MoveDirection.Normalize();

            if (Difference.magnitude >= Movespeed && CheckInFront(MoveDirection))
            {
                gameObject.transform.position += MoveDirection * Movespeed * Time.deltaTime;
                if (Difference.magnitude <= Movespeed)
                {
                    gameObject.transform.position = MoveTarget;
                }
            } else if (Difference.magnitude <= Movespeed)
            {
                RaiseArrived?.Invoke(this);
                DoMove = false;
            }
        }

        if (LookAt != null)
        {
            var LookDirection = LookAt.transform.position - Position;
            LookDirection.Normalize();
            var Rotation = Quaternion.LookRotation(LookDirection, gameObject.transform.up);
            gameObject.transform.rotation = Rotation;
        }
    }

    protected bool CheckInFront(Vector3 Direction)
    {
        var RayStart = gameObject.transform.position;
        var RayEnd = Direction;

        if (Physics.Raycast(RayStart, RayEnd, out RaycastHit Hit, Movespeed))
        {

            if (Hit.collider.gameObject.tag != "Enemy" && Hit.collider.gameObject.tag != "Player")
            {
                RaiseArrived?.Invoke(this);
                return false;
            }
        }

        return true;
    }
}
