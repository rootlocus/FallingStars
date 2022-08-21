using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelState : MonoBehaviour
{
    public static LevelState Instance;
    public event EventHandler OnStatePreStart;
    public event EventHandler OnStateStart;

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
    [SerializeField] private DeathLine deathline;
    

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
        LevelGrid.Instance.OnStartLevel += LevelGrid_OnStartLevel;
    }

    private void Update() 
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer > 0f) {
            return;
        }

        switch (currentState)
        {
            case State.PreStart:
                //1) game start sign
                OnStatePreStart?.Invoke(this, EventArgs.Empty);
                NextState();
                break;
            case State.Start:
                //2) spawn orbs
                //3 spawn launcher move in
                OnStateStart?.Invoke(this, EventArgs.Empty);
                LevelGrid.Instance.SpawnLevelOrbs(3);
                NextState();
                break;
            case State.Running:
                LevelGrid.Instance.MoveOrbRows();
                LevelGrid.Instance.CheckSpawnOrbRow();
                break;
            case State.Lose:
                Debug.Log("LOSE");
                break;
            case State.Pause:
                break;
            case State.Countdown:
                Debug.Log("countdown start");
                NextState();
                //start countdown
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
                break;
            case State.Start:
                currentState = State.Running;
                stateTimer = 2f;
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
}
