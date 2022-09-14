using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Buff/GuideBuff")]
public class GuideBuff : BuffSO
{
    public static EventHandler<float> OnGuideBuffActivate;

    public override void Execute()
    {
        OnGuideBuffActivate?.Invoke(this, duration);
    }
}
