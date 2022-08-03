using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private GridSystem gridSystem;
    [SerializeField] private Transform debugPrefab;
    [SerializeField] private Transform orbPrefab;


    void Start()
    {

    }

    private void Update() {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse position" + MouseWorld.GetPosition());
            Debug.Log("end position" +  LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition()).ToString() );
            GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            Instantiate(orbPrefab, LevelGrid.Instance.GetWorldPositionCenter(gridPosition) , Quaternion.identity);
        }
    }

}
