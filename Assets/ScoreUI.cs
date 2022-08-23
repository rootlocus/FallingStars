using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;


    public void SetScore(int newScore)
    {
        scoreText.SetText(newScore.ToString());
    }
}
