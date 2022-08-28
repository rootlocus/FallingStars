using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelState : MonoBehaviour
{
    public static LevelState Instance;
    public event EventHandler OnStatePreStart;
    public event EventHandler OnStateStart;
    public event EventHandler OnStateLose;

    [SerializeField] private DeathLine deathline;
    [SerializeField] private RushLine rushLine;
    [SerializeField] private int chaseKillCount;

    private enum State
    {
        PreStart,
        Start,
        Running,
        Win,
        Lose,
        Pause,
        Countdown,
    }
    private State currentState;
    private float stateTimer;
    private bool isRunning;
    private int currentKillCount;
    

    private void Awake() 
    {
        if (Instance != null)
        {
            Debug.LogError("Theres more than one LevelState " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() 
    {
        deathline.OnOrbEnter += DeathLine_OnOrbEnter;
        deathline.OnOrbExit += DeathLine_OnOrbExit;
        rushLine.OnOrbEnter += RushLine_OnOrbEnter;
        LevelGrid.Instance.OnStartLevel += LevelGrid_OnStartLevel;
        LevelGrid.Instance.OnSuccessfulMatch += LevelGrid_OnSuccessfulMatch;
        
        isRunning = true;
        currentKillCount = 0;
    }

    private void Update() 
    {
        if (!isRunning) return;

        stateTimer -= Time.deltaTime;
        if (stateTimer > 0f) {
            return;
        }

        switch (currentState)
        {
            case State.PreStart:
                OnStatePreStart?.Invoke(this, EventArgs.Empty);
                NextState();
                break;
            case State.Start:
                OnStateStart?.Invoke(this, EventArgs.Empty);
                LevelGrid.Instance.SpawnLevelOrbs(5);
                NextState();
                break;
            case State.Running:
                LevelGrid.Instance.MoveOrbRows();
                LevelGrid.Instance.CheckSpawnOrbRow();
                break;
            case State.Lose:
                OnStateLose?.Invoke(this, EventArgs.Empty); // maybe pass score and time here ?
                isRunning = false;
                break;
            case State.Pause:
                break;
            case State.Countdown:
                NextState();
                //TODO start countdown
                //change audio
                //if countdown reach then set state to lose
                break;
            default:
                break;
        }
    }

    private void NextState()
    {
        switch (currentState)
        {
            case State.PreStart:
                currentState = State.Start;
                stateTimer = 2f;
                break;
            case State.Start:
                currentState = State.Running;
                stateTimer = 3f;
                break;
            case State.Countdown:
                currentState = State.Lose;
                stateTimer = 5f;
                break;
            default:
                break;
        }
    }

    private void LevelGrid_OnStartLevel(object sender, EventArgs e)
    {
        currentState = State.PreStart;
    }

    private void LevelGrid_OnSuccessfulMatch(object sender, LevelGrid.OnSuccessfulMatchArgs e)
    {
        int previousCount = currentKillCount;
        currentKillCount += e.orbDestroyed;
        currentKillCount += e.orbFallen;

        if (previousCount < chaseKillCount && currentKillCount >= chaseKillCount)
        {
            LevelGrid.Instance.GridChaseMode();
        }
    }
    
    private void DeathLine_OnOrbEnter(object sender, EventArgs e)
    {
        if (currentState != State.Lose && currentState != State.Countdown)
        {
            currentState = State.Countdown;
        }
    }

    private void DeathLine_OnOrbExit(object sender, EventArgs e)
    {
        currentState = State.Running;
        stateTimer = 0f;
    }

    private void RushLine_OnOrbEnter(object sender, EventArgs e)
    {
        currentKillCount = 0;
        LevelGrid.Instance.SetGridSpeedNormal();
    }

}
