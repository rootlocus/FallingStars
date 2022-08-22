using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI startLabel;
    [SerializeField] private Ease easeType;
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

    [Button("Display Start Label")]
    private void DisplayStartLabel()
    {
        ui.alpha = 1;
        startLabel.fontSize = 300f;
        RectTransform labelTransform = startLabel.rectTransform;
        labelTransform.rect.Set(0,0,0,0);

        Sequence shrinkAndExit = DOTween.Sequence();

        labelTransform.DOShakeAnchorPos(5f, 10f, 10, 90);

        shrinkAndExit
            .Append( DOTween.To(() => startLabel.fontSize, newFontSize => startLabel.fontSize = newFontSize, 84f, 1f).SetEase(easeType) )
            .AppendInterval(1f)
            .OnComplete(() => {
                labelTransform.DOAnchorPosY(-800f, 0.5f).SetEase(Ease.OutExpo)
                    .OnComplete(()=> ui.alpha = 0);
            });
    }

}
