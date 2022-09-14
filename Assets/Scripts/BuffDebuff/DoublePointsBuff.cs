using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Buff/DoublePointsBuff")]
public class DoublePointsBuff : BuffSO
{
    public static EventHandler<float> OnDoublePointsBuffActivate;

    public override void Execute()
    {
        OnDoublePointsBuffActivate?.Invoke(this, duration);
    }
}
