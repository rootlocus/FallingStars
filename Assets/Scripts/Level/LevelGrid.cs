using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random=UnityEngine.Random;


public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance;
    public event EventHandler OnStartLevel;
    public class OnSuccessfulMatchArgs : EventArgs
    {
        public int orbDestroyed;
        public int orbFallen;
        public int score;
    }
    public event EventHandler<OnSuccessfulMatchArgs> OnSuccessfulMatch;
    public event EventHandler OnUnsuccessfulMatch;
    

    [Header("Prefabs Transform")]
    [SerializeField] private Transform debugPrefab;
    [SerializeField] private Transform orbPrefab;
    [SerializeField] private Transform orbContainer;

    [Header("Configurations")]
    [SerializeField] private List<OrbTypeSO> orbTypes;
    [SerializeField] private LayerMask orbLayer;
    [SerializeField] private AudioClip matchSoundClip;
    [SerializeField] private AudioClip pushbackSoundClip;
    [SerializeField] private float defaultGridSpeed = 0.25f;
    [SerializeField] private Ease easeType;

    private GridSystem gridSystem;
    private int width = 10;
    private int height = 15;
    private float cellSize = 2f;

    private bool isMoving = false;
    private float nextLineSpawn; // spawn row of orb on this y position
    private float currentGridSpeed;


    private void Awake() 
    {
        if (Instance != null)
        {
            Debug.LogError("Theres more than one Level Grid " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        gridSystem = new GridSystem(width, height, cellSize);
        
        // gridSystem.CreateDebugObject(debugPrefab, orbContainer);
        //TODO: spawn the rope sprite

        nextLineSpawn = -1f;
        currentGridSpeed = defaultGridSpeed;

        OnStartLevel?.Invoke(this, EventArgs.Empty);
    }

    public void PushGridBack(GridPosition gridPosition, OrbTypeSO orbType)
    {
        float reverseSpeed = -10f;
        float timeToHitMaxSpeed = 0.5f;
        DOVirtual.Float(currentGridSpeed, reverseSpeed, timeToHitMaxSpeed, v => {
            currentGridSpeed = v;
        }).OnComplete(() => {
            currentGridSpeed = defaultGridSpeed;
        });

        AudioManager.Instance.PlaySFX(pushbackSoundClip);
    }

    public void MoveOrbRows()
    {
        if (!isMoving)
        {
            return;
        }
        
        orbContainer.position += Vector3.down * Time.deltaTime * currentGridSpeed;
    }

    public void CheckSpawnOrbRow()
    {
        if (orbContainer.position.y <= nextLineSpawn)
        {
            SpawnOrbRow();
            nextLineSpawn = nextLineSpawn - 2f;
        }
    }

    public void StartMoving()
    {
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    public GridSystem GetGridSystem() => gridSystem;

    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition - orbContainer.position);
    
    public Vector3 GetWorldPositionCenter(GridPosition gridPosition) => gridSystem.GetWorldPositionCenter(gridPosition);

    public List<GridObject> GetAdjacentGridObjects(GridPosition gridPosition) => gridSystem.GetAdjacentGridObjects(gridPosition);

    public bool HasMatch3Link(GridPosition gridPosition, ref List<GridObject> matchedGridObjects) => gridSystem.HasMatch3Link(gridPosition, ref matchedGridObjects);

    public List<OrbTypeSO> GetOrbTypes() => orbTypes;
    
    public LayerMask GetOrbMask() => orbLayer;

    public void AttachOrbToGrid(GridPosition gridPosition, OrbTypeSO orbType)
    {
        StopMoving();
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);

        Transform orb = Instantiate(orbPrefab, GetWorldPositionCenter(gridPosition) + orbContainer.position, Quaternion.identity);
        orb.parent = orbContainer;

        Orb attachedOrb = orb.GetComponent<Orb>();
        attachedOrb.Setup(orbType);
        gridObject.AddOrb(attachedOrb, orbType);

        List<GridObject> matchingLists = new List<GridObject>();
        TryMatchGridPosition(gridPosition, ref matchingLists);
        StartMoving();
    }

    private void TryMatchGridPosition(GridPosition gridPosition, ref List<GridObject> matchingLists)
    {
        matchingLists.Clear();
        bool isMatched = HasMatch3Link(gridPosition, ref matchingLists);

        if (isMatched) {
            int matchCount = matchingLists.Count;
            int totalScore = 0;

            foreach (GridObject gridObject in matchingLists)
            {
                gridObject.RemoveOrb();
                Vector2 gridObjectPosition = gridSystem.GetWorldPositionCenter(gridObject.GetGridPosition()) + orbContainer.position;
                ScoreManager.Instance.Create(gridObjectPosition, 100);

                totalScore += 100;
            }
            AudioManager.Instance.PlaySFX(matchSoundClip);
            int fallenCount = HandleIslandGrids();

            //match orb here
            OnSuccessfulMatch?.Invoke(this, new OnSuccessfulMatchArgs {
                orbDestroyed = matchCount,
                orbFallen = fallenCount,
                score = totalScore,
            });
        } else 
        {
            OnUnsuccessfulMatch?.Invoke(this, EventArgs.Empty);
        }
    }

    //TODO: able to be more efficient
    public int HandleIslandGrids()
    {
        int fallenCount = 0;
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
                
                fallenCount++;
                gridObject.RemoveOrb();
                removedGridObjects.Add(gridObject);
            }
        }

        return fallenCount;
    }

    private void SpawnOrbRow()
    {
        gridSystem.SpawnOrbRow(orbPrefab, orbContainer);
    }

    public void SpawnLevelOrbs(int size)
    {
        List<OrbTypeSO> orbTypes = LevelGrid.Instance.GetOrbTypes();
        float timeToMove = 2.5f;

        for (int y = gridSystem.GetHeight() - 1; y >= height - size; y--)
        {
            int offSetX = (y % 2 == 0) ? 1 : 0;
            timeToMove -= 0.5f;

            for (int x = 0; x < gridSystem.GetWidth() - offSetX; x++)
            {
                int doubleWidth = x * 2 + offSetX;
                GridPosition gridPosition = new GridPosition(doubleWidth, y);

                Vector2 endOrbPosition = GetWorldPositionCenter(gridPosition);
                Vector2 startOrbPosition = endOrbPosition;
                startOrbPosition.y = 30f;
                
                Transform orbTransform = GameObject.Instantiate(orbPrefab, startOrbPosition, Quaternion.identity);
                orbTransform.parent = orbContainer;
                orbTransform.DOMove(endOrbPosition, timeToMove).SetEase(easeType);

                //initialize orb
                OrbTypeSO typeSO = orbTypes[Random.Range(0, orbTypes.Count)];
                Orb orb = orbTransform.GetComponent<Orb>();

                gridSystem.GetGridObject(gridPosition).AddOrb(orb, typeSO);
            }
        }
    }

    public void GridChaseMode()
    {
        float maxSpeed = 1.5f;
        float timeToHitMaxSpeed = 5f;
        
        DOVirtual.Float(currentGridSpeed, maxSpeed, timeToHitMaxSpeed, v => {
            currentGridSpeed = v;
        });
    }

    public void SetGridSpeedNormal()
    {
        currentGridSpeed = defaultGridSpeed;
    }

    // public void SpawnAndShiftOrbRow(Transform orbPrefab) => gridSystem.SpawnGridRow(orbPrefab);

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
