using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RushLine : MonoBehaviour
{
    public event EventHandler OnOrbEnter;
    // public event EventHandler OnOrbExit;

    [SerializeField] private Vector2 startPosition;
    [SerializeField] private Vector2 endPosition;
    [SerializeField] private LayerMask orbLayer;
    // private bool isHitPrevious = false;


    private void Update()
    {
        RaycastHit2D orbHit = Physics2D.Linecast(startPosition, endPosition, orbLayer);

        if (orbHit) 
        {
            // isHitPrevious = true;
            OnOrbEnter?.Invoke(this, EventArgs.Empty);
        }

        // if (isHitPrevious == true && orbHit == false)
        // {
        //     isHitPrevious = false;
        //     OnOrbExit?.Invoke(this, EventArgs.Empty);
        // }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(startPosition, endPosition);
    }
}
