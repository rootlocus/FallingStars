using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance;

    [SerializeField] private Transform debugPrefab;
    [SerializeField] private Transform orbPrefab;
    [SerializeField] private List<OrbTypeSO> orbTypes;
    [SerializeField] private LayerMask orbLayer;

    private GridSystem gridSystem;
    private int width = 10;
    private int height = 20;
    private float cellSize = 2f;
    private List<GridObject> matchingLists;


    private void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("Theres more than one Level Grid " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        matchingLists = new List<GridObject>();

        gridSystem = new GridSystem(width, height, cellSize);
        
        gridSystem.CreateDebugObject(debugPrefab);

        gridSystem.PopulateOrbObjects(orbPrefab, 3);
    }

    public GridSystem GetGridSystem() => gridSystem;

    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
    
    public Vector3 GetWorldPositionCenter(GridPosition gridPosition) => gridSystem.GetWorldPositionCenter(gridPosition);

    public List<GridObject> GetAdjacentGridObjects(GridPosition gridPosition) => gridSystem.GetAdjacentGridObjects(gridPosition);

    public bool HasMatch3Link(GridPosition gridPosition, ref List<GridObject> matchedGridObjects) => gridSystem.HasMatch3Link(gridPosition, ref matchedGridObjects);

    public List<OrbTypeSO> GetOrbTypes() => orbTypes;
    
    public LayerMask GetOrbMask() => orbLayer;

    public void SpawnOrbObjectOnGridPosition(GridPosition gridPosition, OrbTypeSO orbType)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);

        Transform orb = Instantiate(orbPrefab, LevelGrid.Instance.GetWorldPositionCenter(gridPosition), Quaternion.identity);

        Orb attachedOrb = orb.GetComponent<Orb>();
        attachedOrb.Setup(orbType);
        gridObject.AddOrb(attachedOrb, orbType);

        MatchGridPosition(gridPosition);
    }

    private void MatchGridPosition(GridPosition gridPosition)
    {
        matchingLists.Clear();
        bool isMatched = LevelGrid.Instance.HasMatch3Link(gridPosition, ref matchingLists);

        if (isMatched) {
            foreach (GridObject gridObject in matchingLists)
            {
                gridObject.RemoveOrb();
            }
        }
    }

    private void OnDrawGizmos() {
        if (!Application.isPlaying || gridSystem == null) return;
        Gizmos.color = Color.yellow;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridPosition gridPosition = new GridPosition(x, y);
                Vector2 center = gridSystem.GetWorldPosition(gridPosition);
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(center + new Vector2(cellSize / 2, cellSize / 2), new Vector2(cellSize, cellSize) );
            }
        }
    }
}
