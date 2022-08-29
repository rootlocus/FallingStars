using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static event EventHandler OnPauseMenu;
    public static event EventHandler OnResumeMenu;

    [SerializeField] private PauseUI pauseUI;
    private bool isActivated;
    private bool isPaused;


    private void Start() 
    {
        isActivated = false;
        isPaused = false;

        LevelState.Instance.OnStatePreStart += LevelState_OnStatePreStart;
    }

    private void LevelState_OnStatePreStart(object sender, EventArgs e)
    {
        isActivated = true;
    }

    private void Update() 
    {
        if (!isActivated) return;
        
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.O))
        {
            TogglePauseMode();
        }
    }

    public void TogglePauseMode()
    {
        if (isPaused)
        {
            OnResumeMenu?.Invoke(this, EventArgs.Empty);
            isPaused = false;
            pauseUI.ResumeGame();
        } else {
            OnPauseMenu?.Invoke(this, EventArgs.Empty);
            isPaused = true;
            pauseUI.PauseGame();
        }
    }
}
