using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    [SerializeField] private Transform pointPrefab;
    [SerializeField] private ScoreUI scoreUI;

    private int currentScore;


    private void Awake() 
    {
        if (Instance != null)
        {
            Debug.LogError("Theres more than one ScoreManager " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() 
    {
        currentScore = 0;
        scoreUI.SetScore(currentScore);
        
        LevelGrid.Instance.OnSuccessfulMatch += LevelGrid_OnSuccessfulMatch;
    }

    private void LevelGrid_OnSuccessfulMatch(object sender, int score)
    {
        currentScore += score;
        scoreUI.SetScore(currentScore);
    }

    public PointPopup Create(Vector3 _position, int _score) 
    {
        Transform popupTransform = Instantiate(pointPrefab, _position, Quaternion.identity);

        PointPopup popup = popupTransform.GetComponent<PointPopup>();
        popup.Init(_score);

        return popup;
    }

}
