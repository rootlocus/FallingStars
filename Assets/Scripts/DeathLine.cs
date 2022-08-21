using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathLine : MonoBehaviour
{
    public event EventHandler OnOrbEnter;
    public event EventHandler OnOrbExit;

    private Vector2 startPosition;
    private Vector2 endPosition;
    [SerializeField] private LayerMask orbLayer;
    private bool isHitPrevious = false;


    private void Start()
    {
        startPosition = new Vector2(0.5f, 5.5f);
        endPosition = new Vector2(20.5f, 5.5f);
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
}
