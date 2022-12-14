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

    public void AddOrb(Orb orb)
    {
        this.orb = orb;
    }

    public void RemoveOrb()
    {
        this.orb.Destroy();
        this.orb = null;
    }

    public void DropOrb()
    {
        this.orb.FallDown();
        this.orb = null;
    }
    public void DropOrbWithoutPoints()
    {
        this.orb.FallDownWithoutPoints();
        this.orb = null;
    }

    public void UnsetOrb() => this.orb = null;
    
    public void SetOrb(Orb orb) => this.orb = orb;

    public bool HasOrb() => orb != null;

    public Orb GetOrb() => orb;

    public OrbSO GetOrbSO() => orb.GetOrbSO();

    public void SetPosition(GridPosition gridPosition) => this.gridPosition = gridPosition;

    public GridPosition GetGridPosition() => gridPosition;
    

    public override string ToString()
    {   
        return gridPosition.ToString();
    }
}
