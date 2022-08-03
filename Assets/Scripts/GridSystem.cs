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

    public GridSystem(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridObjectArray = new GridObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridPosition gridPosition = new GridPosition(x, y);
                gridObjectArray[x, y] = new GridObject(gridPosition);
                // Debug.DrawLine(GetWorldPosition(gridPosition), GetWorldPosition(new GridPosition(gridPosition.x, gridPosition.y + 1)), Color.white, 100f);
                // Debug.DrawLine(GetWorldPosition(gridPosition), GetWorldPosition(new GridPosition(gridPosition.x + 1, gridPosition.y)), Color.white, 100f);
            }
        }
                // Debug.DrawLine(GetWorldPosition(new GridPosition(0, height)), GetWorldPosition(new GridPosition(width, height)), Color.white, 100f);
                // Debug.DrawLine(GetWorldPosition(new GridPosition(width, 0)), GetWorldPosition(new GridPosition(width, height)), Color.white, 100f);

    }

    public void CreateDebugObject(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridPosition gridPosition = new GridPosition(x, y);

                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPositionCenter(gridPosition), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
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
        float x = gridPosition.x + offset;

        return (new Vector3(x, gridPosition.y) * cellSize);
    }

    public Vector3 GetWorldPositionCenter(GridPosition gridPosition)
    {
        return GetWorldPosition(gridPosition) + new Vector3(cellSize, cellSize) * 0.5f;
    }

    public GridPosition GetGridPosition(Vector2 worldPosition)
    {
        int posX = 0;
        int posY = Mathf.FloorToInt(worldPosition.y / cellSize);

        if (posY % 2 == 0) {
            float originX = (worldPosition.x / cellSize) - offSetSecondRow;
            posX = Mathf.FloorToInt(originX);
        } else {
            posX = Mathf.FloorToInt(worldPosition.x / cellSize);
        }

        return new GridPosition(
            posX,
            posY
        );
    }

}
