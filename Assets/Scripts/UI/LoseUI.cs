using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoseUI : MonoBehaviour
{
    [SerializeField] private GameObject loseUI;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI timeText;


    private void Awake()
    {
        quitButton.onClick.AddListener(() => {
            SceneHandler.Instance.QuitGame();
        });
        restartButton.onClick.AddListener(() => {
            SceneHandler.Instance.RestartLevel();
        }); 
    }

    private void Start()
    {
        LevelState.Instance.OnStateLose += LevelState_OnStateLose;

        loseUI.SetActive(false);
    }

    private void LevelState_OnStateLose(object sender, EventArgs e)
    {
        SetPoints(999);
        SetTime(5, 32);

        loseUI.SetActive(true);
    }

    private void SetPoints(int points)
    {
        pointsText.SetText("Points: " + points);
    }

    private void SetTime(int minutes, int seconds)
    {
        timeText.SetText("Time: " + minutes + " minutes " + seconds + " seconds");
    }
}
