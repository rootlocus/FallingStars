using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class LoseUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
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

        HideLoseWindow();
    }

    [Button("Lose Window Test")]
    private void LevelState_OnStateLose(object sender, EventArgs e)
    {
        SetPoints(ScoreManager.Instance.GetCurrentScore());

        double time = LevelState.Instance.GetTotalPlayTime();
        string timeText = TimeSpan.FromSeconds(time).ToString(@"m\:ss");
        SetTime(timeText);


        AudioManager.Instance.PauseBGM();
        AudioManager.Instance.PlayMenuSFX(loseSound);

        ShowLoseWindow();
    }

    private void SetPoints(int points)
    {
        pointsText.SetText("Points: " + points);
    }

    private void SetTime(string time)
    {
        timeText.SetText("Time: " + time);
    }

    public void HideLoseWindow()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowLoseWindow()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
