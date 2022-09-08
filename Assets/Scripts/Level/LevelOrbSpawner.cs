using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class LevelOrbSpawner : MonoBehaviour
{
    public static LevelOrbSpawner Instance;

    [SerializeField] private Transform orbContainer;
    [SerializeField] private Transform orbPrefab;
    [SerializeField] private List<OrbTypeSO> orbTypes;
    [SerializeField] private List<OrbTypeSO> currentOrbPool;
    [SerializeField] private int linesToLevel;
    private int currentLinesSpawned;
    private float nextLineSpawn; // spawn row of orb on this y position
    
    
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

    public List<OrbTypeSO> GetCurrentOrbPool() => currentOrbPool;

    public Transform SpawnOrb(Vector3 spawnPosition)
    {
        Transform orbTransform = Instantiate(orbPrefab, spawnPosition, Quaternion.identity);
        orbTransform.parent = orbContainer;

        return orbTransform;
    }

    private void SpawnOrbRow(Transform orbPrefab, Transform orbContainer)
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

            //Add orbs
            Transform orbTransform = SpawnOrb(LevelGrid.Instance.GetWorldPositionCenter(gridPosition) + orbContainer.position);

            Orb orb = orbTransform.GetComponent<Orb>();
            gridObject.AddOrb(orb, GetRandomOrbFromPool());
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
                
                Transform orbTransform = SpawnOrb(startOrbPosition);
                orbTransform.DOMove(endOrbPosition, timeToMove).SetEase(Ease.Linear);

                //initialize orb
                Orb orb = orbTransform.GetComponent<Orb>();

                // LevelGrid.Instance.
                LevelGrid.Instance.GetGridObject(gridPosition).AddOrb(orb, GetRandomOrbFromPool());
            }
        }
    }


    public void CheckSpawnOrbRow()
    {
        if (orbContainer.position.y <= nextLineSpawn)
        {
            SpawnOrbRow(orbPrefab, orbContainer);
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
