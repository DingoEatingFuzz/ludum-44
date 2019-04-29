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
    Retreat
}

public enum Strategy
{
    ExecuteRandom,
    ExecuteAll,
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
    [HideInInspector]
    public List<Behavior> Behaviors = new List<Behavior>();

    [Tooltip("How should multiple behaviors be handled")]
    public Strategy MultiStrategy;

    [HideInInspector]
    public Behavior CurrentBehavior;

    public EnemyData Data { get; protected set; } = new EnemyData();

    protected int _currentIndex = 0;
    // Used by the sequential strategy
    public int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            if (value != _currentIndex && (value < 0 || value >= Behaviors.Count))
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
        CurrentBehavior = null;

        if (Behaviors.Count == 0)
        {
            return BehaviorPick;
        }

        switch (MultiStrategy)
        {
            case Strategy.ExecuteAll:
                BehaviorPick.AddRange(Behaviors.Where(e => !e.IsRunningOrAborting()));
                break;
            case Strategy.ExecuteRandom:
                BehaviorPick.Add(Behaviors.Where(e => !e.IsRunningOrAborting()).ElementAt(Mathf.FloorToInt(Random.Range(0, Behaviors.Count))));
                break;
            case Strategy.ExecuteSequential:
                BehaviorPick.Add(Behaviors[CurrentIndex++]); 
                break;
            default:
                break;
        }

        if (MultiStrategy != Strategy.ExecuteAll)
        {
            CurrentBehavior = BehaviorPick.First();
        }

        //Debug.Log(CurrentBehavior);

        return BehaviorPick;
    }
}

/// <summary>
/// Enemy controller
/// </summary>
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(DamageableComponent))]
[RequireComponent(typeof(StateManager))]
public class EnemyControllerComponent : MonoBehaviour
{
    [Tooltip("How often should update it's state")]
    public float CheckInterval = .5f;

    //[Tooltip("Which weapon to use")]
    public WeaponComponent Weapon { get; protected set; }

    public BehaviorConfig DormantBehaviorConfig;// = new BehaviorConfig();
    public BehaviorConfig AlertedBehaviorConfig;// = new BehaviorConfig();
    public BehaviorConfig EngagedBehaviorConfig;// = new BehaviorConfig();
    public BehaviorConfig RetreatBehaviorConfig;// = new BehaviorConfig();
    protected BehaviorConfig CurrentBehaviorSet;

    protected StateManager Manager;

    [HideInInspector]
    public Renderer Renderer { get; protected set; }

    protected EnemyState _State;
    public EnemyState State
    {
        get => _State;
        set
        {
             AbortBehaviors();
            switch (value)
            {
                case EnemyState.Dormant:
                    CurrentBehaviorSet = DormantBehaviorConfig;
                    break;
                case EnemyState.Alerted:
                    CurrentBehaviorSet = AlertedBehaviorConfig;
                    break;
                case EnemyState.Engaged:
                    CurrentBehaviorSet = EngagedBehaviorConfig;
                    break;
                case EnemyState.Retreat:
                    CurrentBehaviorSet = RetreatBehaviorConfig;
                    break;
                default:
                    break;
            }
            _State = value;
        }
    }

    protected DeathComponent DeathComponent;
    protected Coroutine CheckRoutine;

    /// <summary>
    /// Awake
    /// </summary>
    protected void Awake()
    {
        GetComponents();

        if (DeathComponent != null)
        {
            DeathComponent.RaiseDied += Died;
        }

        State = EnemyState.Dormant;
        Manager.Controller = this;
    }

    /// <summary>
    /// Assigns all the behaviors to their appropriate containers based on type
    /// </summary>
    protected void GetComponents()
    {
        var Behaviors = GetComponents<Behavior>().ToList();
        DormantBehaviorConfig.Behaviors.AddRange(Behaviors.Where(b => b.Type == EnemyState.Dormant));
        AlertedBehaviorConfig.Behaviors.AddRange(Behaviors.Where(b => b.Type == EnemyState.Alerted));
        EngagedBehaviorConfig.Behaviors.AddRange(Behaviors.Where(b => b.Type == EnemyState.Engaged));
        RetreatBehaviorConfig.Behaviors.AddRange(Behaviors.Where(b => b.Type == EnemyState.Retreat));

        Renderer = GetComponentInChildren<Renderer>();
        Manager = GetComponent<StateManager>();

        var Weapons = GetComponentsInChildren<WeaponComponent>(true).ToList();
        Weapon = Weapons.First();

        DeathComponent = GetComponent<DeathComponent>();
        
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
            Manager.CheckState();

            var DoGetNext = false;
            switch (CurrentBehaviorSet.MultiStrategy)
            {
                case Strategy.ExecuteAll:
                    DoGetNext = true;
                    break;
                case Strategy.ExecuteRandom:
                case Strategy.ExecuteSequential:
                    DoGetNext = !(CurrentBehaviorSet.CurrentBehavior?.IsRunningOrAborting() ?? false);
                    break;
                default:
                    break;
            }

            if (DoGetNext)
            {
                CurrentBehaviorSet.GetNext()?.ForEach(e => e.Run(gameObject));
            }

            yield return new WaitForSeconds(CheckInterval);
        } while (enabled);
    }

    /// <summary>
    /// Aborts all behaviors in the current behavior set
    /// </summary>
    protected void AbortBehaviors()
    {
        CurrentBehaviorSet?.Behaviors.ForEach(b => b.Abort());
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
