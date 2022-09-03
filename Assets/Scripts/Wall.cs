using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Wall : MonoBehaviour
{
    [Header("Animations")]
    [SerializeField] private EntranceAnimation wallLeftAnimation;
    [SerializeField] private EntranceAnimation wallRightAnimation;

    private void Start() 
    {
        // LevelState.Instance.OnStatePreStart += LevelState_OnStatePreStart;
    }

    // private void LevelState_OnStatePreStart(object sender, EventArgs e)
    // {
    //     wallLeftAnimation.MoveTransform();
    //     wallRightAnimation.MoveTransform();
    // }
}
