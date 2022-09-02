using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class LoseUI : MonoBehaviour
{
    [SerializeField] private GameObject loseUI;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private AudioClip loseSound;
    

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

    [Button("Lose Window Test")]
    private void LevelState_OnStateLose(object sender, EventArgs e)
    {
        SetPoints(ScoreManager.Instance.GetCurrentScore());
        SetTime(5, 32);

        AudioManager.Instance.PauseBGM();
        AudioManager.Instance.PlayMenuSFX(loseSound);

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
