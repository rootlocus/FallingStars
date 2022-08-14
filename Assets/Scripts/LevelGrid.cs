using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance;
    private enum State
    {
        PreStart,
        Start,
        Running,
        Win,
        Lose,
        Pause,
    }
    
    [SerializeField] private Transform debugPrefab;
    [SerializeField] private Transform orbPrefab;
    [SerializeField] private List<OrbTypeSO> orbTypes;
    [SerializeField] private LayerMask orbLayer;

    private State state;
    private State nextState;
    private float stateTimer;

    private GridSystem gridSystem;
    private int width = 10;
    private int height = 15;
    private float cellSize = 2f;

    // for moving entire grid orbs
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private Transform orbContainer;
    private bool isMoving = false;


    private void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("Theres more than one Level Grid " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gridSystem = new GridSystem(width, height, cellSize);
        
        gridSystem.CreateDebugObject(debugPrefab);

        state = State.PreStart;
        // SpawnLevelOrbs(orbPrefab, 3);
    }

    private void Update() 
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer > 0f) {
            return;
        }
        switch (state)
        {
            case State.PreStart:
                NextState();
                break;
            case State.Start:
                //1) game start sign
                //2) spawn orbs
                //3 spawn launcher move in
                SpawnLevelOrbs(orbPrefab, 3);
                NextState();
                break;
            case State.Running:
                SpawnAndShiftOrbRow(orbPrefab);
                stateTimer = 4f;
                break;
            case State.Win:
                break;
            case State.Lose:
                break;
            case State.Pause:
                break;
            default:
                break;
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.PreStart:
                state = State.Start;
                break;
            case State.Start:
                state = State.Running;
                stateTimer = 2f;
                break;
            default:
                break;
        }
    }

    private void MoveOrbRows()
    {
        if (!isMoving)
        {
            return;
        }
        orbContainer.position += Vector3.down * Time.deltaTime * speed;
    }

    public void StartMoving()
    {
        isMoving = true;
    }

    public GridSystem GetGridSystem() => gridSystem;

    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition - orbContainer.position);
    
    public Vector3 GetWorldPositionCenter(GridPosition gridPosition) => gridSystem.GetWorldPositionCenter(gridPosition);

    public List<GridObject> GetAdjacentGridObjects(GridPosition gridPosition) => gridSystem.GetAdjacentGridObjects(gridPosition);

    public bool HasMatch3Link(GridPosition gridPosition, ref List<GridObject> matchedGridObjects) => gridSystem.HasMatch3Link(gridPosition, ref matchedGridObjects);

    public List<OrbTypeSO> GetOrbTypes() => orbTypes;
    
    public LayerMask GetOrbMask() => orbLayer;

    public void SpawnOrbOnGridPosition(GridPosition gridPosition, OrbTypeSO orbType)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);

        Transform orb = Instantiate(orbPrefab, GetWorldPositionCenter(gridPosition) + orbContainer.position, Quaternion.identity);
        orb.parent = orbContainer;

        Orb attachedOrb = orb.GetComponent<Orb>();
        attachedOrb.Setup(orbType);
        gridObject.AddOrb(attachedOrb, orbType);

        List<GridObject> matchingLists = new List<GridObject>();
        MatchGridPosition(gridPosition, ref matchingLists);
    }

    private void MatchGridPosition(GridPosition gridPosition, ref List<GridObject> matchingLists)
    {
        matchingLists.Clear();
        bool isMatched = LevelGrid.Instance.HasMatch3Link(gridPosition, ref matchingLists);

        if (isMatched) {
            foreach (GridObject gridObject in matchingLists)
            {
                gridObject.RemoveOrb();
            }
        }
        HandleIslandGrids();
    }

    //TODO: able to be more efficient
    public void HandleIslandGrids()
    {
        List<GridObject> gridObjectWithOrbs = new List<GridObject>();
        gridObjectWithOrbs = gridSystem.GetAllGridObjectWithOrbs();

        List<GridObject> removedGridObjects = new List<GridObject>();

        foreach (GridObject gridObjectWithOrb in gridObjectWithOrbs) 
        {
            List<GridObject> attachedGridObject = new List<GridObject>();
            bool isAttached = gridSystem.IsGridPositionAttached(gridObjectWithOrb.GetGridPosition(), out attachedGridObject);

            foreach(GridObject gridObject in attachedGridObject)
            {
                if (removedGridObjects.Contains(gridObject)) {
                    continue;
                }
                if (isAttached) {
                    continue;
                }

                gridObject.RemoveOrb();
                removedGridObjects.Add(gridObject);
            }
        }
    }

    public void SpawnLevelOrbs(Transform orbPrefab, int size)
    {
        List<OrbTypeSO> orbTypes = LevelGrid.Instance.GetOrbTypes();

        for (int x = 0; x < gridSystem.GetWidth(); x+=2)
        {
            for (int y = gridSystem.GetHeight() - 1; y >= height - size; y--)
            {
                int offSetX = (y % 2 == 0) ? 1 : 0;

                GridPosition gridPosition = new GridPosition(x + offSetX, y);
                Transform orbTransform = GameObject.Instantiate(orbPrefab, GetWorldPositionCenter(gridPosition), Quaternion.identity);

                orbTransform.parent = orbContainer;
                //initialize orb
                Orb orb = orbTransform.GetComponent<Orb>();
                OrbTypeSO typeSO = orbTypes[Random.Range(0, orbTypes.Count)];
                gridSystem.GetGridObject(gridPosition).AddOrb(orb, typeSO);
            }
        }
    }

    public void SpawnAndShiftOrbRow(Transform orbPrefab) => gridSystem.SpawnGridRow(orbPrefab);

    // private void OnDrawGizmos() {
    //     if (!Application.isPlaying || gridSystem == null) return;
    //     Gizmos.color = Color.yellow;

    //     for (int x = 0; x < width; x++)
    //     {
    //         for (int y = 0; y < height; y++)
    //         {
    //             GridPosition gridPosition = new GridPosition(x, y);
    //             Vector2 center = gridSystem.GetWorldPosition(gridPosition);
    //             Gizmos.matrix = transform.localToWorldMatrix;
    //             Gizmos.DrawWireCube(center + new Vector2(cellSize / 2, cellSize / 2), new Vector2(cellSize, cellSize) );
    //         }
    //     }
    // }
}
