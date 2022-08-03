using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPosition
{
    public int x;
    public int y;

    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return "x: " + x + "; y: " + y + ";";
    }
}
