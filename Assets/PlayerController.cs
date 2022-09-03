using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public event EventHandler OnEscapePressed;

    private void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("Theres more than one PlayerController " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

}
