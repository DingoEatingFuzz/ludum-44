using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyState
{
    Dormant,
    Alerted,
    Engaged,
    Retreating
}

public enum Strategy
{
    ExecuteAll,
    ExecuteRandom,
    ExecuteSequential
}

public class EnemyData
{
    // Nothing here yet
}

[System.Serializable]
public class BehaviorConfig
{
    [Tooltip("Which behavior(s) to run on state")]
    public Behavior[] Behaviors;

    [Tooltip("How should multiple behaviors be handled")]
    public Strategy Strategy;

    public EnemyData Data { get; protected set; } = new EnemyData();

    /// <summary>
    /// Removes null values from the behaviors
    /// </summary>
    public void Cleanup()
    {
        Behaviors = Behaviors.Where(b => b != null).ToArray();
    }

    public Behavior CurrentBehavior;

    protected int _currentIndex = 0;
    // Used by the sequential strategy
    public int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            if (value != _currentIndex && (value < 0 || value >= Behaviors.Length))
            {
                _currentIndex = 0;
            } else
            {
                _currentIndex = value;
            }
        }
    }

    /// <summary>
    /// Gets 
    /// </summary>
    /// <returns></returns>
    public List<Behavior> GetNext()
    {
        var BehaviorPick = new List<Behavior>();

        switch (Strategy)
        {
            case Strategy.ExecuteAll:
                BehaviorPick = Behaviors.Where(e => !e.IsRunningOrAborting()).ToList();
                break;
            case Strategy.ExecuteRandom:
                BehaviorPick.Concat(new[] { Behaviors.Where(e => !e.IsRunningOrAborting()).ElementAt(new System.Random().Next(Behaviors.Length))});
                break;
            case Strategy.ExecuteSequential:
                BehaviorPick.Concat(new[] { Behaviors[CurrentIndex++] });
                break;
            default:
                break;
        }

        CurrentBehavior = BehaviorPick.Count == 0 ? null : BehaviorPick[0];
        return BehaviorPick;
    }
}

/// <summary>
/// Figures out what state the controller should be in
/// </summary>
public abstract class StateManager : MonoBehaviour
{

    protected EnemyControllerComponent Controller;
    protected float UpdateInterval = 0.5f;

    protected EnemyState _State;
    protected EnemyState State
    {
        get => _State;
        set => Controller.State = _State;
    }

    public StateManager(EnemyControllerComponent Controller)
    {
        this.Controller = Controller;
        State = Controller.State;
    }

    public abstract void CheckState();
}

/// <summary>
/// Enemy controller
/// </summary>
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(DamageableComponent))]
[RequireComponent(typeof(Renderer))]
public class EnemyControllerComponent : MonoBehaviour
{
    [Tooltip("How often should update it's state")]
    public float CheckInterval = .5f;

    [Header("Behaviors")]
    [Tooltip("Behavior to run in the DORMANT state")]
    public BehaviorConfig DormantBehaviors;

    [Tooltip("Behavior to run in the ALERTED state")]
    public BehaviorConfig AlertedBehaviors;

    [Tooltip("Behavior to run in the ENGAGED state")]
    public BehaviorConfig EngagedBehaviors;

    [Tooltip("Behavior to run in the RETREATING state")]
    public BehaviorConfig RetreatingBehaviors;

    [Header("State")]
    [Tooltip("Which state manager to use")]
    public StateManager Manager;

    [Header("Weapon")]
    [Tooltip("Which weapon to use")]
    public WeaponComponent Weapon;

    protected EnemyState _State;
    public EnemyState State
    {
        get => _State;
        set
        {
            if (value != _State)
            {
                AbortBehaviors();
                _State = value;
            }
        }
    }

    public Renderer Renderer { get; protected set; }

    protected BehaviorConfig CurrentBehaviorSet;
    protected DeathComponent DeathComponent;
    protected Coroutine CheckRoutine;

    /// <summary>
    /// Awake
    /// </summary>
    protected void Awake()
    {
        State = EnemyState.Dormant;
        CurrentBehaviorSet = DormantBehaviors;

        DeathComponent = gameObject.GetComponent<DeathComponent>();
        if (DeathComponent != null)
        {
            DeathComponent.RaiseDied += Died;
        }

        DormantBehaviors.Cleanup();
        AlertedBehaviors.Cleanup();
        EngagedBehaviors.Cleanup();
        RetreatingBehaviors.Cleanup();

        Renderer = GetComponent<Renderer>();
    }

    /// <summary>
    /// Starts the check routine
    /// </summary>
    protected void OnEnable()
    {
        if (CheckRoutine != null)
        {
            StopCoroutine(CheckRoutine);
        }

        CheckRoutine = StartCoroutine(DeferedUpdate());
    }

    /// <summary>
    /// Stops updates and aborts all behaviors
    /// </summary>
    protected void OnDisable()
    {
        if (CheckRoutine != null)
        {
            StopCoroutine(CheckRoutine);
        }

        AbortBehaviors();
        State = EnemyState.Dormant;
    }

    /// <summary>
    /// Checks if a new behavior should be run then runs it
    /// </summary>
    protected virtual IEnumerator DeferedUpdate()
    {
        do
        {
            var DoGetNext = false;
            switch (CurrentBehaviorSet.Strategy)
            {
                case Strategy.ExecuteAll:
                    DoGetNext = true;
                    break;
                case Strategy.ExecuteRandom:
                case Strategy.ExecuteSequential:
                    DoGetNext = !CurrentBehaviorSet.CurrentBehavior?.IsRunningOrAborting() ?? true;
                    break;
                default:
                    break;
            }

            if (DoGetNext)
            {
                CurrentBehaviorSet.GetNext().ForEach(e => e.Run(gameObject));
            }

            yield return new WaitForSeconds(CheckInterval);
        } while (enabled);
    }

    /// <summary>
    /// Aborts all behaviors in the current behavior set
    /// </summary>
    protected void AbortBehaviors()
    {
        Array.ForEach(CurrentBehaviorSet.Behaviors, e => e.Abort());
    }

    /// <summary>
    /// Death logic
    /// </summary>
    protected void Died()
    {
        AbortBehaviors();

        // Lolded
        Destroy(gameObject);
    }
}
