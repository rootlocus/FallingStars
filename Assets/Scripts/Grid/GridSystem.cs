using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem
{
    private int width;
    private int height;
    private float cellSize;
    private List<List<GridObject>> gridObjectRows;
    private float offSetSecondRow = 0.5f;
    private List<GridPosition> doubleWidthDirection = new List<GridPosition> {
        new GridPosition(2, 0),
        new GridPosition(1, -1),
        new GridPosition(-1, - 1),
        new GridPosition(-2, 0),
        new GridPosition(-1, +1),
        new GridPosition(1, 1)
    };


    // using the double coordinate system, https://www.redblobgames.com/grids/hexagons/
    public GridSystem(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridObjectRows = new List<List<GridObject>>();
        for (int y = 0; y < this.height; y++)
        {
            int offSetX = (y % 2 == 0) ? 1 : 0;
            List<GridObject> row = new List<GridObject>();

            for (int x = 0; x < this.width; x++)
            {
                int doubleWidth = x * 2 + offSetX;
                GridPosition gridPosition = new GridPosition(doubleWidth, y);
                GridObject gridObject = new GridObject(gridPosition);
                row.Add(gridObject);
            }

            gridObjectRows.Add(row);
        }
    }

#region Accessors
	    public int GetWidth() => width;
	    
	    public int GetHeight() => height;
#endregion

#region Mutator
	    public void AddGridObjectRows(List<GridObject> row) => gridObjectRows.Add(row);
	
	    public void AddHeight(int i) => height = height + i;
	
	    public void CreateDebugObject(Transform debugPrefab, Transform parent)
	    {
	        for (int y = 0; y < height; y++)
	        {
	            int offSetX = (y % 2 == 0) ? 1 : 0;
	
	            for (int x = 0; x < width - offSetX; x++)
	            {
	                int doubleWidthX = x * 2 + offSetX;
	                GridPosition gridPosition = new GridPosition(doubleWidthX, y);
	
	                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPositionCenter(gridPosition), Quaternion.identity);
	                debugTransform.parent = parent;
	
	                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
	                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
	            }
	        }
	    }
#endregion

    public GridObject GetGridObject(GridPosition gridPosition)
    {
        int offSetX = (gridPosition.y % 2 == 0) ? 1 : 0;

        return gridObjectRows[gridPosition.y][(gridPosition.x - offSetX) / 2];
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        float offset = 0f;
        if (gridPosition.y % 2 == 0) {
            offset = offSetSecondRow;
        }
        float x = (gridPosition.x / 2) + offset;

        return (new Vector3(x, gridPosition.y) * cellSize);
    }

    public Vector3 GetWorldPositionCenter(GridPosition gridPosition)
    {
        return GetWorldPosition(gridPosition) + new Vector3(cellSize, cellSize) * 0.5f;
    }

    public OrbTypeSO GetOrbSO(GridPosition gridPosition) => GetGridObject(gridPosition).GetOrbSO();

    public GridPosition GetGridPosition(Vector2 worldPosition)
    {
        int doubleWidth = 2;
        int posX = 0;
        int posY = Mathf.FloorToInt(worldPosition.y / cellSize);

        if (posY % 2 == 0) {
            float originX = (worldPosition.x / cellSize) - offSetSecondRow;
            posX = Mathf.FloorToInt(originX) * doubleWidth + 1;
        } else {
            posX = Mathf.FloorToInt(worldPosition.x / cellSize) * doubleWidth;
        }

        return new GridPosition(
            posX,
            posY
        );
    }

    // Valid double coordinate system
    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        int x = gridPosition.x;
        int y = gridPosition.y;
        
        if (y >= 0 && y < height) {
            if (y % 2 == 0) {

                if (x > 0 && x < (width - 1) * 2) {
                    return true;
                }
            } else {
                if (x >= 0 && x < width * 2) 
                {
                    return true;
                }
            }
        }

        return false;
    }

    public List<GridObject> GetAdjacentGridObjects(GridPosition gridPosition, bool hasOrbsOnly=true)
    {
        List<GridObject> adjacentGridObjects = new List<GridObject>();

        foreach (GridPosition direction in doubleWidthDirection)
        {
            GridPosition adjacentPosition = gridPosition + direction;

            if (!IsValidGridPosition(adjacentPosition)) {
                continue;
            }

            GridObject adjacentGridObject = GetGridObject(adjacentPosition);
            if (hasOrbsOnly && !adjacentGridObject.HasOrb()) {
                continue;
            }
            
            adjacentGridObjects.Add(adjacentGridObject);
        }

        return adjacentGridObjects;
    }

    
    public bool IsGridPositionAttached(GridPosition gridPosition, out List<GridObject> attachedGridObjects)
    {
        attachedGridObjects = new List<GridObject>();
        
        FindAttachedGridObjects(gridPosition, ref attachedGridObjects);

        foreach (GridObject attachedGridObject in attachedGridObjects)
        {
            GridPosition attachedGridPosition = attachedGridObject.GetGridPosition();
            if (attachedGridPosition.y + 1 >= height) {
                return true;
            }
        }

        return false;
    }

    public List<GridObject> FindAttachedGridObjects(GridPosition gridPosition, ref List<GridObject> mainList)
    {
        mainList.Add(GetGridObject(gridPosition));

        List<GridObject> adjacentGridObjects = new List<GridObject>();
        adjacentGridObjects = GetAdjacentGridObjects(gridPosition);

        foreach (GridObject adjacentGridObject in adjacentGridObjects)
        {
            if (!adjacentGridObject.HasOrb()) {
                continue;
            }
            if (mainList.Contains(adjacentGridObject)) {
                continue;
            }
            FindAttachedGridObjects(adjacentGridObject.GetGridPosition(), ref mainList);
        }

        return adjacentGridObjects;
    }

    /*
    *    Get all matching colours of selected object
    *    check their adjacent neighbour not including the previous object or coordionate or position
    *    Add the matching colour GO to a list
    *    if size is > 3 its a match and return true and output the list
    *    else return false
    */
    public bool HasMatch3Link(GridPosition gridPosition, ref List<GridObject> matchedGridObjects)
    {
        OrbTypeSO selectedOrbType = GetOrbSO(gridPosition);
        List<GridObject> adjacentGridObjects = GetAdjacentGridObjects(gridPosition);

        matchedGridObjects.Add(GetGridObject(gridPosition));//first one

        GetMatchingGridObjectsByType(gridPosition, ref matchedGridObjects);

        int matchSize = 2;
        if (matchedGridObjects.Count > matchSize) {
            return true;
        }

        return false;
    }

    public void GetMatchingGridObjectsByType(GridPosition gridPosition, ref List<GridObject> matchedGridObjects)
    {
        OrbTypeSO selectedOrbType = GetOrbSO(gridPosition);
        List<GridObject> adjacentGridObjects = new List<GridObject>();
        adjacentGridObjects = GetAdjacentGridObjects(gridPosition);

        foreach (GridObject gridObject in adjacentGridObjects)
        {
            if (!gridObject.HasOrb()) {
                continue;
            }

            if (gridObject.GetOrbSO() == selectedOrbType)
            {
                if (matchedGridObjects.Contains(gridObject)) // avoid selecting same object
                {
                    continue;
                }

                matchedGridObjects.Add(gridObject);

                GetMatchingGridObjectsByType(gridObject.GetGridPosition(), ref matchedGridObjects);
            }
        }
    }

    public List<GridObject> GetAllGridObjectWithOrbs()
    {
        List<GridObject> gridObjectsWithOrbs = new List<GridObject>();

        for (int y = 0; y < this.height; y++)
        {
            int offSetX = (y % 2 == 0) ? 1 : 0;

            for (int x = 0; x < this.width - offSetX; x++)
            {
                int doubleWidthX = x * 2 + offSetX;
                GridObject gridObject = GetGridObject(new GridPosition(doubleWidthX, y));
                
                if (gridObject.HasOrb())
                {
                    gridObjectsWithOrbs.Add(gridObject);
                }
            }
        }

        return gridObjectsWithOrbs;
    }


    // private void PushBubblesDown()
    // {
    //     for (int x = 0; x < this.width; x+=2)
    //     {
    //         GridObject previousGridObject = null;
    //         GridObject currentGridObject = null;
    //         for (int y = 0; y < this.height; y++)
    //         {
    //             int offSetX = (y % 2 == 0) ? 1 : 0;

    //             previousGridObject = currentGridObject;
    //             currentGridObject = GetGridObject(new GridPosition(x + offSetX, y));

    //             if (!currentGridObject.HasOrb())
    //             {
    //                 continue;
    //             }

    //             if (y != 0)
    //             {
    //                 Orb orb = currentGridObject.GetOrb();
    //                 currentGridObject.UnsetOrb();
    //                 previousGridObject.SetOrb(orb);

    //                 orb.transform.position = GetWorldPositionCenter(previousGridObject.GetGridPosition());
    //             } else if (y == 0) //TODO if reached a line position maybe kill player instead
    //             {
    //                 currentGridObject.RemoveOrb();
    //             }
    //         }
    //     }
    // }
}
