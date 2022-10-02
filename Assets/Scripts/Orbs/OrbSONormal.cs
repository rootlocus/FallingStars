using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/OrbSO/Normal")]
public class OrbSONormal : OrbSO
{
    public override void Execute(GridPosition gridPosition, OrbSO orbType)
    {
        LevelGrid.Instance.AttachOrbToGrid(gridPosition, orbType);
    }
}
