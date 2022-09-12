using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherVisualGuide : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private float leftWallBoundary;
    [SerializeField] private float rightWallBoundary;
    [SerializeField] private Transform orbShadowTransform;
    [SerializeField] private SpriteRenderer orbShadowSprite;
    [SerializeField] private LineRenderer lineRendererPrefab;
    private List<LineRenderer> renderLists;

    private Vector2 endLine;
    private bool isActivated;
    private List<Vector2> reflectPoints;


    private void Start() 
    {
        renderLists = new List<LineRenderer>();
        isActivated = false;
        reflectPoints = new List<Vector2>();

        PauseUI.OnResumeMenu += PauseUI_OnResumeMenu;
        PauseUI.OnPauseMenu += PauseUI_OnPauseMenu;
        LevelState.Instance.OnStateStart += LevelState_OnStateStart;
    }

    private void Update()
    {
        if (!isActivated) return;

        Vector3 mousePosition = (Vector2)MouseWorld.GetPosition();
        mousePosition.y = Mathf.Clamp(mousePosition.y, 5.5f, 27.25f);

        Vector2 initialDirection = mousePosition - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, initialDirection, float.MaxValue, hitLayer);

        reflectPoints.Clear();
        reflectPoints.Add(transform.position);

        if (!hit) return;

        HandleCollisions(initialDirection, hit);
        DeactivateExtraLineRenderer();
        CreateUpdateLineRenderer();

        DrawShadow();
        // DrawLine();
    }

    private void CreateUpdateLineRenderer()
    {
        for (int i = 0; i < reflectPoints.Count - 1; i++)
        {
            if (i > renderLists.Count - 1)  // if not enough line renderer. spawn another one
            {
                AddLineRenderer(reflectPoints[i], reflectPoints[i + 1]); // TODO: change to pool
            }
            else
            {
                renderLists[i].gameObject.SetActive(true);
                renderLists[i].SetPosition(0, reflectPoints[i]);
                renderLists[i].SetPosition(1, reflectPoints[i + 1]);
            }
        }
    }

    private void DeactivateExtraLineRenderer()
    {
        bool shouldDisableExtraRenderer = renderLists.Count > reflectPoints.Count - 1;
        
        if (shouldDisableExtraRenderer)
        {
            for (int i = reflectPoints.Count - 1; i < renderLists.Count; i++)
            {
                renderLists[i].gameObject.SetActive(false);
            }
        }
    }

    private void HandleCollisions(Vector2 direction, RaycastHit2D hit)
    {
        Vector2 collidePoint = hit.point;

        if (hit.transform.tag == "Wall")
        {
            collidePoint.x = Mathf.Clamp(collidePoint.x, leftWallBoundary, rightWallBoundary);
            reflectPoints.Add(collidePoint);

            ReflectArrowOnWall(direction, collidePoint);
        }
        else if (hit.transform.tag == "Orb")
        {
            reflectPoints.Add(collidePoint - new Vector2(0, 1f));
        }
    }

    private void AddLineRenderer(Vector3 startPoint, Vector3 endPoint)
    {
        LineRenderer renderer = Instantiate(lineRendererPrefab);
        renderer.transform.parent = this.transform;

        renderer.SetPosition(0, startPoint);
        renderer.SetPosition(1, endPoint);

        renderLists.Add(renderer);
    }

    private void DrawShadow()
    {
        Vector2 lastPosition = reflectPoints[reflectPoints.Count - 1];
        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(lastPosition);

        Vector3 predictivePosition = LevelGrid.Instance.GetWorldPositionCenter(gridPosition);
        orbShadowTransform.position = predictivePosition;
    }

    private void ReflectArrowOnWall(Vector3 initialDir, Vector3 startPosition)
    {
        Vector3 reflectDirection = Vector3.Reflect(initialDir, Vector3.right);

        //Because colliding with the collided wall
        float centerXPosition = 10.5f;
        Vector3 readjustedPosition = startPosition;
        readjustedPosition.x += startPosition.x < centerXPosition ? 0.01f : -0.01f;

        RaycastHit2D reflectHit = Physics2D.Raycast(readjustedPosition, reflectDirection, float.MaxValue, hitLayer);
        if (!reflectHit) return;

        HandleCollisions(reflectDirection, reflectHit);
    }

    private void PauseUI_OnPauseMenu(object sender, EventArgs e)
    {
        isActivated = false;
    }

    private void PauseUI_OnResumeMenu(object sender, EventArgs e)
    {
        isActivated = true;
    }
    
    private void LevelState_OnStateStart(object sender, EventArgs e)
    {
        isActivated = true;
    }

}
