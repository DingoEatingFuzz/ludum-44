using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpdateData
{
    public float Health;
}

public class HealthDepletedData
{

}

public class HealthComponent : MonoBehaviour
{
    public delegate void HandleUpdated(object Sender, HealthUpdateData Data);
    public HandleUpdated RaiseUpdated;

    public delegate void HandleDepleted(object Sender, HealthDepletedData Data);
    public HandleDepleted RaiseDepleted;

    [Tooltip("Maximum health")]
    public float Maximum;

    protected float _Current;
    /// <summary>
    /// The current health value
    /// </summary>
    public float Current
    {
        get => _Current;
        set
        {
            if (value == _Current)
            {
                return;
            }

            _Current = value;
            Mathf.Clamp(_Current, 0f, Maximum);

            RaiseUpdated(gameObject, new HealthUpdateData());

            if (_Current == 0f)
            {
                RaiseDepleted(gameObject, new HealthDepletedData());
            }
        }
    }

    public bool IsDepleted { get => Current == 0f; }

    /// <summary>
    /// Awake
    /// </summary>
    protected void Awake()
    {
        Current = Maximum;
    }

    /// <summary>
    /// Attemps to add <paramref name="Amount"/> to health and returns actual amount added
    /// </summary>
    /// <param name="Amount">Amount to add</param>
    /// <returns>Actual amount added</returns>
    public float Add(float Amount)
    {
        var Previous = Current;
        Current += Amount;
        return Mathf.Abs(Current - Previous);
    }

    /// <summary>
    /// Attempts to remove <paramref name="Amount"/> and returns actual amount removed
    /// </summary>
    /// <param name="Amount">Amount to remove</param>
    /// <returns
    public float Remove(float Amount)
    {
        var Previous = Current;
        Current -= Amount;

        return Mathf.Abs(Current - Previous);
    }

    /// <summary>
    /// Sets the current health
    /// </summary>
    /// <param name="Amount"></param>
    public void Set(float Amount)
    {
        Current = Amount;
    }
}
