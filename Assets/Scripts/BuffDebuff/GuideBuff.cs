using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buff/GuideBuff")]
public class GuideBuff : BuffSO
{
    public override void Execute()
    {
        Debug.Log("ACTIVE Guide BUFF");
    }
}
