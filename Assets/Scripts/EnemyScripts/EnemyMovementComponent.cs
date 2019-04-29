using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementComponent : MonoBehaviour
{
    public float Movespeed = 8f;

    [HideInInspector]
    public Vector3 MoveTarget;

    [HideInInspector]
    public GameObject LookAt;

    // Update is called once per frame
    void Update()
    {
        var Position = transform.position;
        var Difference = Position - MoveTarget;
        var MoveDirection = Difference.normalized;
        
        if (Difference.magnitude < Movespeed)
        {
            transform.position += MoveDirection * Movespeed;
        }

        if (LookAt != null)
        {
            var LookDirection = Position - LookAt.transform.position;
            LookDirection.Normalize();
            var Rotation = Quaternion.LookRotation(LookDirection, transform.up);
        }
    }
}
