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
    private Vector2 endLine;
    private bool isActivated;
    private List<Vector2> reflectPoints;


    private void Start() 
    {
        isActivated = false;
        reflectPoints = new List<Vector2>();

        PauseUI.OnResumeMenu += PauseUI_OnResumeMenu;
        PauseUI.OnPauseMenu += PauseUI_OnPauseMenu;
        LevelState.Instance.OnStateStart += LevelState_OnStateStart;
    }

    private void Update() 
    {
        if (!isActivated) return;

        Vector3 mousePosition = (Vector2) MouseWorld.GetPosition();
        mousePosition.y = Mathf.Clamp(mousePosition.y, 5.5f, 27.25f);

        Vector2 dir = mousePosition - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, float.MaxValue, hitLayer);
        
        reflectPoints.Clear();
        reflectPoints.Add(transform.position);

        if (!hit) return;
        
        if (hit.transform.tag == "Wall")
        {
            Vector2 collidePoint = hit.point;
            collidePoint.x = Mathf.Clamp(collidePoint.x, leftWallBoundary, rightWallBoundary);
            CollideOnWall(collidePoint);

            ReflectArrowOnWall(dir, collidePoint);
        } else if (hit.transform.tag == "Orb")
        {
            Vector2 collidePoint = hit.point - new Vector2(0, 1f);

            reflectPoints.Add(collidePoint);
        }

        DrawShadow();
        DrawLine();
    }

    private void DrawLine()
    {
        // number of lines renderer = count - 1

        int i = 0;
        float centerXPosition = 10.5f;

        lineRenderer.positionCount = reflectPoints.Count;
        foreach (Vector2 point in reflectPoints)
        {
            Vector2 newPoint = point;
            if (i != 0 && i != reflectPoints.Count - 1) {
                newPoint.x = (point.x < centerXPosition) ? -3f : 27f;
            }

            lineRenderer.SetPosition(i, newPoint);
            i++;
        }
    }

    private void AddLineRenderer(Vector3 startPoint, Vector3 endPoint)
    {
        LineRenderer renderer = Instantiate(lineRendererPrefab);
        renderer.SetPosition(0, startPoint);
        renderer.SetPosition(1, endPoint);
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
        // Bounce
        Vector3 reflectDirection = Vector3.Reflect(initialDir, Vector3.right);
        float centerXPosition = 10.5f;
        if (startPosition.x < centerXPosition) {
            startPosition.x = startPosition.x + 0.01f;
        } else {
            startPosition.x = startPosition.x - 0.01f;
        }
        RaycastHit2D reflectHit = Physics2D.Raycast(startPosition, reflectDirection, float.MaxValue, hitLayer);

        if (!reflectHit) return;

        Vector3 reflectPoint = reflectHit.point;

        if (reflectHit.transform.tag == "Wall")
        {
            reflectPoint.x = Mathf.Clamp(reflectPoint.x, leftWallBoundary, rightWallBoundary);

            reflectPoints.Add(reflectPoint);
            ReflectArrowOnWall(reflectDirection, reflectHit.point);
        } else if (reflectHit.transform.tag == "Orb")
        {
            reflectPoint = reflectPoint - new Vector3(0, 1f);

            reflectPoints.Add(reflectPoint);
        }
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

    private void CollideOnWall(Vector2 collidePoint)
    {
        //collide wall
        reflectPoints.Add(collidePoint);
    }

}
