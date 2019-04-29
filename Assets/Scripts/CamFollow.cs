using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform PlayerTransform;
    private Vector3 Center;
    private float MaxX = 30;
    private float MaxY = 30;

    // Start is called before the first frame update
    void Start()
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        Center = new Vector3(0, 0, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerTransform != null)
        {
            var NewPosition = new Vector3(PlayerTransform.position.x, PlayerTransform.position.y, transform.position.z);
            if (Mathf.Abs(NewPosition.x) > MaxX)
            {
                NewPosition.x = transform.position.x;
            }
            if (Mathf.Abs(NewPosition.y) > MaxY)
            {
                NewPosition.y = transform.position.y;
            }
            transform.position = NewPosition;
        }
    }
}
