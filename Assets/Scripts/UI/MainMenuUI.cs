using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ModularMotion;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("GO Components Dependencies")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Image blackoutUI;
    [SerializeField] private UIMotion titleUiMotion;
    [SerializeField] private TextMeshProUGUI titleFont;
    [SerializeField] private RectTransform titleBackground;

    [Header("Configuration")]
    [SerializeField] private float timeToStart;

    [Header("External Dependencies")]
    [SerializeField] private OptionsUI optionsUI;
    
    private bool isInvoked;
    private float defaultSize;
    [SerializeField] private float bounceMultiplier = 0.1f;
    

    private void Start()
    {
        isInvoked = false;
        defaultSize = titleFont.fontSize;

        quitButton.onClick.AddListener(() => {
            SceneHandler.Instance.QuitGame();
        });
        startButton.onClick.AddListener(() => {
            StartGame();
        });
        optionButton.onClick.AddListener(() => {
            optionsUI.OpenWindow();
        });
    }

    private void Update() 
    {
        if (isInvoked) return;
        if (titleUiMotion.Frame == 4)
        {
            isInvoked = true;
            AudioManager.Instance.PlayMenuBGM();
        }
    }

    private void FixedUpdate() {
        if (isInvoked)
        {
            if (AudioManager.Instance.GetMiddlesFrequencyDisapason() == 0)
            {
                titleFont.fontSize = defaultSize;
            } else {
                titleFont.fontSize = defaultSize + (AudioManager.Instance.GetMiddlesFrequencyDisapason() *  bounceMultiplier);
                
            }
        }
    }

    private void StartGame()
    {
        blackoutUI.DOFade(1f, timeToStart).OnComplete(() => {
            SceneHandler.Instance.StartEndlessMode();
        });
    }




}
