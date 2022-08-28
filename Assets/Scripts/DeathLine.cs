using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathLine : MonoBehaviour
{
    public event EventHandler OnOrbEnter;
    public event EventHandler OnOrbExit;

    [SerializeField] private Vector2 startPosition;
    [SerializeField] private Vector2 endPosition;
    [SerializeField] private LayerMask orbLayer;

    [Header("Animations")]
    [SerializeField] private EntranceAnimation ropeAnimation;

    private bool isHitPrevious = false;


    private void Start() 
    {
        LevelState.Instance.OnStateStart += LevelState_OnStateStart;
    }

    private void Update()
    {
        RaycastHit2D orbHit = Physics2D.Linecast(startPosition, endPosition, orbLayer);

        if (orbHit) 
        {
            isHitPrevious = true;
            OnOrbEnter?.Invoke(this, EventArgs.Empty);
        }

        if (isHitPrevious == true && orbHit == false)
        {
            isHitPrevious = false;
            OnOrbExit?.Invoke(this, EventArgs.Empty);
        }
    }

    private void LevelState_OnStateStart(object sender, EventArgs e)
    {
        ropeAnimation.MoveTransform();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(startPosition, endPosition);
    }
}
