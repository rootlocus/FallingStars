using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static event EventHandler OnCloseOptionUI;

    [SerializeField] private GameObject optionsWindow;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button returnButton;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    private float maxValue;
    private bool isActivated;


    private void Start()
    {
        isActivated = false;
        maxValue = bgmSlider.maxValue;

        returnButton.onClick.AddListener(() => { CloseWindow(); });
        bgmSlider.onValueChanged.AddListener(val => AudioManager.Instance.SetBgmVolume(val / maxValue));
        sfxSlider.onValueChanged.AddListener(val => AudioManager.Instance.SetSfxVolume(val / maxValue));
        masterSlider.onValueChanged.AddListener(val => AudioManager.Instance.AdjustMasterVolume(val / maxValue));
    }

    private void Update() 
    {
        if (!isActivated) return;
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseWindow();
        }    
    }

    public void CloseWindow()
    {
        isActivated = false;
        
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        OnCloseOptionUI?.Invoke(this, EventArgs.Empty);
    }

    public void OpenWindow()
    {
        isActivated = true;

        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        bgmSlider.value = AudioManager.Instance.GetBGMVolume() * maxValue;
        sfxSlider.value = AudioManager.Instance.GetSFXVolume() * maxValue;
        masterSlider.value = AudioManager.Instance.GetMasterVolume() * maxValue;
    }
}
