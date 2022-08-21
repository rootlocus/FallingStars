using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private TextMeshPro startLabel;
    private CanvasGroup ui;

    private void Awake() 
    {
        ui = GetComponent<CanvasGroup>();    
    }

    private void Start() 
    {
        LevelState.Instance.OnStatePreStart += LevelState_OnStatePreStart;
    }

    private void LevelState_OnStatePreStart(object sender, EventArgs e)
    {
        DisplayStartLabel();
    }

    private void DisplayStartLabel()
    {
        
        //do animation with tween
        ui.alpha = 1;
        //tween to hide
    }

}
