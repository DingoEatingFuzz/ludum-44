using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoBehavior : Behavior
{
    public string Message;

    public override void Abort()
    {
    }

    public override void Run(GameObject Owner)
    {
        Debug.Log("[[[ ECHO: " + Message + " ]]]");
    }
}
