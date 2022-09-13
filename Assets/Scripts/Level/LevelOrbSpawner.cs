using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using DG.Tweening;
using Sirenix.OdinInspector;


public class LevelOrbSpawner : MonoBehaviour
{
    public static LevelOrbSpawner Instance;

    [Header("Pool Configuration")]
    [SerializeField] private Orb orbPrefab;
    [SerializeField] private int defaultCapacity = 300;
    [SerializeField] private int maxCapacity = 400;

    [SerializeField] private Transform orbContainer;
    [SerializeField] private List<OrbTypeSO> orbTypes;
    [SerializeField] private List<OrbTypeSO> currentOrbPool;
    [SerializeField] private int linesToLevel;
    private int currentLinesSpawned;
    private float nextLineSpawn; // spawn row of orb on this y position
    private ObjectPool<Orb> pool;
    

    private void Awake() 
    {
        if (Instance != null)
        {
            Debug.LogError("Theres more than one LevelOrbSpawner " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() 
    {
        InitializeNewPool();

        nextLineSpawn = -1f;
        currentLinesSpawned = 0;

        AddNewOrbColor();
        AddNewOrbColor();
        AddNewOrbColor();
    }

    private void Update() 
    {
        if (currentLinesSpawned >= linesToLevel)
        {
            currentLinesSpawned = 0;
            AddNewOrbColor();
        }
    }

#region Pool
    private void InitializeNewPool()
    {
        pool = new ObjectPool<Orb>(() => {
            return Instantiate(orbPrefab);
        }, orb => {
            orb.gameObject.SetActive(true);
        }, orb => {
            orb.gameObject.SetActive(false);
        }, orb => {
            Destroy(orb.gameObject);
        }, false, defaultCapacity, maxCapacity);
    }

    public Transform SpawnOrb(Vector3 spawnPosition, OrbTypeSO orbType, GridObject gridObject)
    {
        Orb orb = pool.Get();
        Transform orbTransform = orb.transform;
        orbTransform.position = spawnPosition;
        orbTransform.parent = orbContainer;

        orb.Setup(orbType, DestroyPrefab);

        gridObject.AddOrb(orb);

        return orb.transform;
    }

    private void DestroyPrefab(Orb orb)
    {
        pool.Release(orb);
    }
#endregion
    
    public List<OrbTypeSO> GetCurrentOrbPool() => currentOrbPool;

[Button("SPAWN ROW")]
    private void SpawnOrbRow()
    {
        LevelGrid.Instance.AddHeight(1);

        int y = LevelGrid.Instance.GetHeight() - 1;
        int offSetX = (y % 2 == 0) ? 1 : 0;
        List<GridObject> row = new List<GridObject>();

        for (int x = 0; x < LevelGrid.Instance.GetWidth() - offSetX; x++)
        {
            //create new grid object
            int doubleWidth = x * 2 + offSetX;
            GridPosition gridPosition = new GridPosition(doubleWidth, y);
            GridObject gridObject = new GridObject(gridPosition);
            row.Add(gridObject);

            //Spawn Orb
            Vector3 orbPosition = LevelGrid.Instance.GetWorldPositionCenter(gridPosition);
            SpawnOrb(orbPosition, GetRandomOrbFromPool(), gridObject);
        }

        LevelGrid.Instance.AddGridObjectRows(row);

        currentLinesSpawned++;
    }

    public void InitializeLevelOrbs(int size)
    {
        float timeToMove = 2.5f;
        int height = LevelGrid.Instance.GetHeight();
        int width = LevelGrid.Instance.GetWidth();

        for (int y = height - 1; y >= height - size; y--)
        {
            int offSetX = (y % 2 == 0) ? 1 : 0;
            timeToMove -= 0.5f;

            for (int x = 0; x < width - offSetX; x++)
            {
                int doubleWidth = x * 2 + offSetX;
                GridPosition gridPosition = new GridPosition(doubleWidth, y);

                Vector2 endOrbPosition = LevelGrid.Instance.GetWorldPositionCenter(gridPosition);
                Vector2 startOrbPosition = endOrbPosition;
                startOrbPosition.y = 30f;
                
                GridObject gridObject = LevelGrid.Instance.GetGridObject(gridPosition);

                Transform orbTransform = SpawnOrb(startOrbPosition, GetRandomOrbFromPool(), gridObject);

                orbTransform.DOMove(endOrbPosition, timeToMove).SetEase(Ease.Linear);
            }
        }
    }


    public void CheckSpawnOrbRow()
    {
        if (orbContainer.position.y <= nextLineSpawn)
        {
            SpawnOrbRow();
            nextLineSpawn = nextLineSpawn - 2f;
        }
    }

    private OrbTypeSO GetRandomOrbFromPool()
    {
        return currentOrbPool[Random.Range(0, currentOrbPool.Count)];
    }

    [Button("ADD ORB COLOR")]
    private void AddNewOrbColor()
    {
        if (orbTypes.Count > 0)
        {
            currentOrbPool.Add(orbTypes[0]);
            orbTypes.RemoveAt(0);
        }
    }
}
