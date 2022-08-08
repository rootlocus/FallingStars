using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem
{
    private int width;
    private int height;
    private GridObject[,] gridObjectArray;
    private float cellSize;
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
        this.width = width * 2;
        this.height = height;
        this.cellSize = cellSize;

        gridObjectArray = new GridObject[this.width, this.height];

        for (int x = 0; x < this.width; x+=2)
        {
            for (int y = 0; y < this.height; y++)
            {
                int offSetX = (y % 2 == 0) ? 1 : 0;

                GridPosition gridPosition = new GridPosition(x + offSetX, y);
                gridObjectArray[x + offSetX, y] = new GridObject(gridPosition);
            }
        }
    }

    public void CreateDebugObject(Transform debugPrefab)
    {
        for (int x = 0; x < width; x+=2)
        {
            for (int y = 0; y < height; y++)
            {
                int offSetX = (y % 2 == 0) ? 1 : 0;

                GridPosition gridPosition = new GridPosition(x + offSetX, y);

                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPositionCenter(gridPosition), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }

    //TEMP
    public void PopulateOrbObjects(Transform orbPrefab, int size)
    {
        List<OrbTypeSO> orbTypes = LevelGrid.Instance.GetOrbTypes();

        for (int x = 0; x < width; x+=2)
        {
            for (int y = height - 1; y >= height - size; y--)
            {
                int offSetX = (y % 2 == 0) ? 1 : 0;

                GridPosition gridPosition = new GridPosition(x + offSetX, y);
                Transform debugTransform = GameObject.Instantiate(orbPrefab, GetWorldPositionCenter(gridPosition), Quaternion.identity);

                //initialize orb
                Orb orb = debugTransform.GetComponent<Orb>();
                OrbTypeSO typeSO = orbTypes[Random.Range(0, orbTypes.Count)];
                GetGridObject(gridPosition).AddOrb(orb, typeSO);
            }
        }
    }

    public GridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.y];
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
                if (x > 0 && x < width) {
                    return true;
                }
            } else {
                if (x >= 0 && x < width) 
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
        // foreach(GridObject gridObject in gridObjectArray)
        // {
        //     Debug.Log(gridObject);
        //     if (gridObject.HasOrb())
        //     {
        //         gridObjectsWithOrbs.Add(gridObject);
        //     }
        // }

        for (int x = 0; x < this.width; x+=2)
        {
            for (int y = 0; y < this.height; y++)
            {
                int offSetX = (y % 2 == 0) ? 1 : 0;

                GridObject gridObject = gridObjectArray[x + offSetX, y];
                if (gridObject.HasOrb())
                {
                    gridObjectsWithOrbs.Add(gridObject);
                }
            }
        }

        return gridObjectsWithOrbs;
    }


}
