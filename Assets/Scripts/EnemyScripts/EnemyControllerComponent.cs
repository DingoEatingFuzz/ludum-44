using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Flags]
public enum EnemyState
{
    Dormant = 0,
    Alerted = 1,
    Engaged = 2,
    Retreat = 3
}

public enum Strategy
{
    ExecuteRandom,
    ExecuteAll,
    ExecuteSequential
}

public class EnemyData
{
    public EnemyControllerComponent Controller;
    public GameObject Player;
    public Renderer Renderer;
    public EnemyWeaponComponent Weapon;
    public GameObject Self;
    public EnemyState State
    {
        get => Controller.State;
        set => Controller.State = value;
    }
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

        List<Behavior> CanRun = Behaviors.Where(e => e.ShouldRun).ToList();

        switch (MultiStrategy)
        {
            case Strategy.ExecuteAll:
                BehaviorPick = CanRun.ToList();
                break;
            case Strategy.ExecuteRandom:
                if (CanRun.Count > 0)
                {
                    BehaviorPick.Add(CanRun.ElementAt(Mathf.FloorToInt(Random.Range(0f, CanRun.Count)))); 
                }
                break;
            case Strategy.ExecuteSequential:
                BehaviorPick.Add(CanRun.First(b => Behaviors.IndexOf(b) > CurrentIndex));
                break;
            default:
                break;
        }

        if (MultiStrategy != Strategy.ExecuteAll && BehaviorPick.Count > 0)
        {
            CurrentBehavior = BehaviorPick.First();
            CurrentIndex = Behaviors.IndexOf(CurrentBehavior);
        } else
        {
            CurrentBehavior = null;
            CurrentIndex = 0;
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
    public EnemyWeaponComponent Weapon { get; protected set; }

    public BehaviorConfig DormantBehaviorConfig;// = new BehaviorConfig();
    public BehaviorConfig AlertedBehaviorConfig;// = new BehaviorConfig();
    public BehaviorConfig EngagedBehaviorConfig;// = new BehaviorConfig();
    public BehaviorConfig RetreatBehaviorConfig;// = new BehaviorConfig();
    protected BehaviorConfig CurrentBehaviorSet;

    protected StateManager Manager;
    protected EnemyData Data = new EnemyData();

    protected EnemyState _State;
    public EnemyState State
    {
        get => _State;
        set
        {
            if (value != _State)
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
        InitEnemyData();
        GetBehaviors();

        Manager = GetComponent<StateManager>();
        DeathComponent = GetComponent<DeathComponent>();

        if (DeathComponent != null)
        {
            DeathComponent.RaiseDied += Died;
        }

        State = EnemyState.Dormant;
        Manager.Data = Data;
    }

    /// <summary>
    /// Creates enemydata and sets its members
    /// </summary>
    protected void InitEnemyData()
    {
        Data = new EnemyData()
        {
            Controller = this,
            Player = GameObject.FindGameObjectWithTag("Player"),
            Weapon = GetComponent<EnemyWeaponComponent>(),
            Renderer = GetComponentInChildren<Renderer>(),
            Self = gameObject
        };
    }

    /// <summary>
    /// Assigns all the behaviors to their appropriate containers based on type
    /// </summary>
    protected void GetBehaviors()
    {
        var Behaviors = GetComponentsInChildren<Behavior>().ToList();
        Behaviors.ForEach(b => b.Data = Data);
        DormantBehaviorConfig.Behaviors.AddRange(Behaviors.Where(b => b.StatesToRun.Any(s => s == EnemyState.Dormant)));
        AlertedBehaviorConfig.Behaviors.AddRange(Behaviors.Where(b => b.StatesToRun.Any(s => s == EnemyState.Alerted)));
        EngagedBehaviorConfig.Behaviors.AddRange(Behaviors.Where(b => b.StatesToRun.Any(s => s == EnemyState.Engaged)));
        RetreatBehaviorConfig.Behaviors.AddRange(Behaviors.Where(b => b.StatesToRun.Any(s => s == EnemyState.Retreat)));
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
                    DoGetNext = CurrentBehaviorSet.CurrentBehavior?.ShouldRun ?? true;
                    break;
                default:
                    break;
            }

            if (DoGetNext)
            {
                CurrentBehaviorSet.GetNext().ForEach(e => e.Run(gameObject, this));
            }

            yield return new WaitForSeconds(CheckInterval);
        } while (enabled);
    }

    /// <summary>
    /// Aborts all behaviors in the current behavior set
    /// </summary>
    protected void AbortBehaviors()
    {
        CurrentBehaviorSet?.Behaviors.Where(b => b.ShouldAbort).ToList().ForEach(b => b.Abort());
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
