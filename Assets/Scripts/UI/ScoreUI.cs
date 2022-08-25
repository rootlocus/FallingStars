using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private RectTransform scoreTransform;
    private float enlargeSize = 64f;

    public void SetScore(int newScore)
    {
        scoreText.fontSize = enlargeSize;
        scoreTransform.DOShakeAnchorPos(1f, 10f, 10, 90);
        DOTween.To(() => scoreText.fontSize, newFontSize => scoreText.fontSize = newFontSize, 36f, 0.5f).SetEase(Ease.InExpo);

        scoreText.SetText(newScore.ToString());
    }
}
