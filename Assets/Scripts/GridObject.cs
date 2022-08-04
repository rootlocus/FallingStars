using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private GridPosition gridPosition;
    private Orb orb;


    public GridObject (GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public void AddOrb(Orb orb, OrbTypeSO typeSO)
    {
        this.orb = orb;
        orb.Setup(typeSO);
    }

    public void RemoveOrb()
    {
        this.orb = null;
    }

    public OrbTypeSO GetOrbSO() => orb.GetOrbTypeSO();

    public override string ToString()
    {   string isActivated = "null";

        if (orb) {
            isActivated = orb.isActivated.ToString();
        }
        return gridPosition.ToString() + "\n" + isActivated;
    }
}
