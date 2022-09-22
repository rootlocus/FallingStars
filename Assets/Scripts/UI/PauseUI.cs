using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public static event EventHandler OnPauseMenu;
    public static event EventHandler OnResumeMenu;

    [SerializeField] private GameObject pauseUI;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button optionButton;

    [Header("External Dependencies")]
    [SerializeField] private OptionsUI optionsUI;

    private bool isActivated;
    private bool isPaused;


    private void Awake()
    {
        quitButton.onClick.AddListener(() => {
            SceneHandler.Instance.QuitGame();
        });
        restartButton.onClick.AddListener(() => {
            SceneHandler.Instance.RestartLevel();
        }); 
        resumeButton.onClick.AddListener(() => {
            TogglePauseMode();
        });
        optionButton.onClick.AddListener(() => {
            isActivated = false;
            optionsUI.OpenWindow();
            optionsUI.onClose = (() => isActivated = true);
        });
    }

    private void Start() 
    {
        isActivated = false;
        isPaused = false;

        LevelState.Instance.OnStatePreStart += LevelState_OnStatePreStart;
        OptionsUI.OnCloseOptionUI += OptionsUI_OnCloseOptionUI;
    }

    private void Update() 
    {
        if (!isActivated) return;
        
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.O))
        {
            TogglePauseMode();
        }
    }

    private void LevelState_OnStatePreStart(object sender, EventArgs e)
    {
        isActivated = true;
    }

    private void OptionsUI_OnCloseOptionUI(object sender, EventArgs e)
    {
        isActivated = true;
    }

    public void HidePauseWindow()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowPauseWindow()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void TogglePauseMode()
    {
        if (isPaused)
        {
            OnResumeMenu?.Invoke(this, EventArgs.Empty);
            isPaused = false;
            HidePauseWindow();

            AudioManager.Instance.UnmuffleBGM();
        } else {
            OnPauseMenu?.Invoke(this, EventArgs.Empty);
            isPaused = true;
            ShowPauseWindow();
            
            AudioManager.Instance.MuffleBGM();
        }
    }
}
