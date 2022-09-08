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

    [Header("Props GO")]
    [SerializeField] private DeathLine deathline;
    [SerializeField] private RushLine rushLine;
    [Header("Animation Config")]
    [SerializeField] private float preStartDuration;
    [SerializeField] private float startDuration;
    [Header("Game Config")]
    [SerializeField] private int chaseKillCount;
    [SerializeField] private float countdownDuration;
    [SerializeField] private AudioClip beepSound;


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
    private State onPausedState;
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
        PauseUI.OnResumeMenu += PauseUI_OnResumeMenu;
        PauseUI.OnPauseMenu += PauseUI_OnPauseMenu;
        Projectile.OnSpecialProjectileStop += Projectile_OnSpecialProjectileStop;
        Launcher.OnSpecialSwap += Launcher_OnSpecialSwap;

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
                LevelOrbSpawner.Instance.InitializeLevelOrbs(5);
                AudioManager.Instance.PlayLevelBGM();

                NextState();
                break;
            case State.Running:
                LevelGrid.Instance.MoveOrbRows();
                LevelOrbSpawner.Instance.CheckSpawnOrbRow();
                break;
            case State.Lose:
                CancelInvoke("CountDownBeep");
                OnStateLose?.Invoke(this, EventArgs.Empty); // maybe pass score and time here ?
                isRunning = false;
                break;
            case State.Pause:
                break;
            case State.Countdown:
                InvokeRepeating("CountDownBeep", 0f, 1f);
                NextState();
                break;
            default:
                break;
        }
    }

    private void CountDownBeep()
    {
        AudioManager.Instance.PlaySFX(beepSound);
    }

    private void NextState()
    {
        switch (currentState)
        {
            case State.PreStart:
                currentState = State.Start;
                stateTimer = preStartDuration;
                break;
            case State.Start:
                currentState = State.Running;
                stateTimer = startDuration;
                break;
            case State.Countdown:
                currentState = State.Lose;
                stateTimer = countdownDuration;
                break;
            default:
                break;
        }
    }

    private void ResetKillCountDifficulty()
    {
        currentKillCount = 0;
        LevelGrid.Instance.SetGridSpeedNormal();
    }

    private void PauseGrid(float pauseDuration)
    {
        if (currentState == State.Lose)
        {
            stateTimer = pauseDuration + stateTimer;
        } else {
            stateTimer = pauseDuration;
        }
    }

#region Events
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
            AudioManager.Instance.MuffleBGM();
        }
    }

    private void DeathLine_OnOrbExit(object sender, EventArgs e)
    {
        currentState = State.Running;
        stateTimer = 0f;

        CancelInvoke("CountDownBeep");
        AudioManager.Instance.UnmuffleBGM();
    }

    private void RushLine_OnOrbEnter(object sender, EventArgs e)
    {
        ResetKillCountDifficulty();
    }

    private void Projectile_OnSpecialProjectileStop(object sender, EventArgs e)
    {
        ResetKillCountDifficulty();
    }

    private void PauseUI_OnPauseMenu(object sender, EventArgs e)
    {
        onPausedState = currentState;

        currentState = State.Pause;
        Time.timeScale = 0;
    }

    private void PauseUI_OnResumeMenu(object sender, EventArgs e)
    {
        currentState = onPausedState;
        Time.timeScale = 1;
    }
    
    private void Launcher_OnSpecialSwap(object sender, float pauseDuration)
    {
        PauseGrid(pauseDuration);
    }
#endregion

}
