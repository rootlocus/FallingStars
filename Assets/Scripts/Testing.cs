using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private GridSystem gridSystem;
    [SerializeField] private Transform debugPrefab;
    [SerializeField] private Transform orbPrefab;
    private List<GridObject> matchingLists;

    [SerializeField] private Transform launchPosition;
    private bool isLaunch;
    private Vector3 aimDir;



    private void Start() {
        matchingLists = new List<GridObject>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1))
        {
            LevelGrid.Instance.StartMoving();
        }
    }

    private void DebugMatch3()
    {
        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

        matchingLists.Clear();
        bool isMatched = LevelGrid.Instance.HasMatch3Link(gridPosition, ref matchingLists);

        Debug.Log(isMatched);
        if (isMatched) {
            foreach (GridObject gridObject in matchingLists)
            {
                Debug.Log(gridObject.ToString());
                gridObject.RemoveOrb();
            }
        }
    }

    private void DebugMousePosition()
    {
        Debug.Log("Mouse position" + MouseWorld.GetPosition());
        Debug.Log("Grid position" +  LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition()).ToString() );
    }

    private void DebugAdjacentGridObjects()
    {
        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

        List<GridObject> gridObjects = LevelGrid.Instance.GetAdjacentGridObjects(gridPosition);
        foreach (GridObject gridObject in gridObjects)
        {
            Debug.Log(gridObject.ToString());
        }
    }

    private void DebugGetOrbName()
    {
        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

        Debug.Log(LevelGrid.Instance.GetGridSystem().GetOrbSO(gridPosition).orbName);
    }

    private void DebugSpawnOrb()
    {
        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

        Instantiate(orbPrefab, LevelGrid.Instance.GetWorldPositionCenter(gridPosition) , Quaternion.identity);
    }

}
