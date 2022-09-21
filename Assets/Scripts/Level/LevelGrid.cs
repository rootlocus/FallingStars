using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


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
    [SerializeField] private Transform orbContainer;

    [Header("Configurations")]
    [SerializeField] private LevelOrbSpawner spawner;
    [SerializeField] private LayerMask orbLayer;
    [SerializeField] private AudioClip matchSoundClip;
    [SerializeField] private AudioClip pushbackSoundClip;
    [SerializeField] private float defaultGridSpeed = 0.25f;
    [SerializeField] private Ease easeType;
    
    private GridSystem gridSystem;
    private int width = 8;
    private int height = 15;
    private float cellSize = 2f;

    private bool isMoving = false;
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

        currentGridSpeed = defaultGridSpeed;

        OnStartLevel?.Invoke(this, EventArgs.Empty);
    }

    public void PushGridBack(GridPosition gridPosition, OrbTypeSO orbType)
    {
        float reverseSpeed = -10f;
        float timeToHitMaxSpeed = 0.5f;
        DOVirtual.Float(currentGridSpeed, reverseSpeed, timeToHitMaxSpeed, v => {
            currentGridSpeed = v;
            MoveOrbRows();
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

    public void StartMoving()
    {
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }

#region Getters
	    public GridSystem GetGridSystem() => gridSystem;
	
	    public int GetHeight() => gridSystem.GetHeight();
	
	    public int GetWidth() => gridSystem.GetWidth();
	
	    public void AddHeight(int i) => gridSystem.AddHeight(i);
	
	    public GridObject GetGridObject(GridPosition gridPosition) => gridSystem.GetGridObject(gridPosition);

        public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition - orbContainer.position);
        
        public Vector3 GetWorldPositionCenter(GridPosition gridPosition) => gridSystem.GetWorldPositionCenter(gridPosition) + orbContainer.position;

        public List<GridObject> GetAdjacentGridObjects(GridPosition gridPosition) => gridSystem.GetAdjacentGridObjects(gridPosition);

        public LayerMask GetOrbMask() => orbLayer;
#endregion

    public void AddGridObjectRows(List<GridObject> row) => gridSystem.AddGridObjectRows(row);

    public bool HasMatch3Link(GridPosition gridPosition, ref List<GridObject> matchedGridObjects) => gridSystem.HasMatch3Link(gridPosition, ref matchedGridObjects);

    public void AttachOrbToGrid(GridPosition gridPosition, OrbTypeSO orbType)
    {
        StopMoving();
        GridObject gridObject = GetGridObject(gridPosition);

        LevelOrbSpawner.Instance.SpawnOrb(GetWorldPositionCenter(gridPosition), orbType, gridObject);

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
                Vector2 gridObjectPosition = GetWorldPositionCenter(gridObject.GetGridPosition());
                ScoreManager.Instance.Create(gridObjectPosition, 100);

                totalScore += 100;
            }
            AudioManager.Instance.PlaySFX(matchSoundClip);

            int fallenCount = HandleIslandGrids();
            totalScore = (fallenCount * 50) + totalScore;

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
                gridObject.DropOrb();
                removedGridObjects.Add(gridObject);
            }
        }

        return fallenCount;
    }

    public void GridChaseMode()
    {
        float maxSpeed = 1.5f;
        float timeToHitMaxSpeed = 5f;
        
        DOVirtual.Float(currentGridSpeed, maxSpeed, timeToHitMaxSpeed, v => {
            currentGridSpeed = v;
        });
    }

    public void SetGridSpeedNormal() => currentGridSpeed = defaultGridSpeed;

    public List<GridObject> GetAllGridObjectWithOrbs() => gridSystem.GetAllGridObjectWithOrbs();

    public void DropAllOrbs()
    {
        List<GridObject> gridObjects = GetAllGridObjectWithOrbs();

        foreach(GridObject gridObject in gridObjects)
        {
            gridObject.DropOrbWithoutPoints();
        }
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
