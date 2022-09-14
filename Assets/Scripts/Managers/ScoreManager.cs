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
    private int multiplier = 1;
    private float duration = 0f;
    private bool isActivated = false;

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
        DoublePointsBuff.OnDoublePointsBuffActivate += DoublePointsBuff_OnDoublePointsBuffActivate;
    }

    private void Update() 
    {
        if (!isActivated) return;

        duration -= Time.deltaTime;

        if (duration <= 0) {
            multiplier = 1;
            isActivated = false;
        }
    }

    private void DoublePointsBuff_OnDoublePointsBuffActivate(object sender, float duration)
    {
        multiplier = 2;
        this.duration = duration;
        isActivated = true;
    }

    private void LevelGrid_OnSuccessfulMatch(object sender, LevelGrid.OnSuccessfulMatchArgs e)
    {
        currentScore += e.score;
        currentScore = currentScore * multiplier;
        scoreUI.SetScore(currentScore);
    }

    public PointPopup Create(Vector3 _position, int _score) 
    {
        Transform popupTransform = Instantiate(pointPrefab, _position, Quaternion.identity);

        PointPopup popup = popupTransform.GetComponent<PointPopup>();
        popup.Init(_score);

        return popup;
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }


}
