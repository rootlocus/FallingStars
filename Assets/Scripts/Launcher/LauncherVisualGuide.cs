using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherVisualGuide : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask hitLayer;
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
        mousePosition.y = Mathf.Clamp(mousePosition.y, 4f, 27.25f);

        Vector2 dir = mousePosition - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, float.MaxValue, hitLayer);
        
        reflectPoints.Clear();
        reflectPoints.Add(transform.position);

        if (!hit) return;
        
        if (hit.transform.tag == "Wall")
        {
            Vector2 collidePoint = hit.point;
            CollideOnWall(collidePoint);

            ReflectArrow(dir, collidePoint);
            // Bounce
            // Vector2 reflectDirection = Vector3.Reflect(dir, Vector3.right);
            // RaycastHit2D reflectHit = Physics2D.Raycast(collidePoint, reflectDirection, float.MaxValue, hitLayer);
            // if (!reflectHit) return;

            // Vector2 newColliderPoint = reflectHit.point - new Vector2(0, 1f);
            // reflectPoints.Add(newColliderPoint);

        } else if (hit.transform.tag == "Orb")
        {
            Vector2 collidePoint = hit.point - new Vector2(0, 1f);

            // Debug.Log("TOUC1");

            reflectPoints.Add(collidePoint);
        }

        DrawLine();
    }

    private void DrawLine()
    {
        int i = 0;
        lineRenderer.positionCount = reflectPoints.Count;
        foreach (Vector2 point in reflectPoints)
        {
            lineRenderer.SetPosition(i, point);
            i++;
        }
    }

    private void ReflectArrow(Vector3 initialDir, Vector2 startPosition)
    {
        // Bounce
        Vector2 reflectDirection = Vector3.Reflect(initialDir, Vector3.right);
        RaycastHit2D reflectHit = Physics2D.Raycast(startPosition, reflectDirection, float.MaxValue, hitLayer);

        if (!reflectHit) return;
        Debug.DrawLine(startPosition, reflectHit.point, Color.green, 0.5f);

        Vector2 reflectPoint = reflectHit.point;

        if (reflectHit.transform.tag == "Wall")
        {
            // Debug.Log("REFLECT");
            reflectPoint.x = Mathf.Clamp(reflectPoint.x, -0.25f, 24.1f);

            reflectPoints.Add(reflectPoint);
            // ReflectArrow(reflectDirection, reflectHit.point);
        } else if (reflectHit.transform.tag == "Orb")
        {
            // Debug.Log("TOUCH2");
            reflectPoints.Add(reflectPoint);
        }
        // Vector2 newColliderPoint = reflectHit.point - new Vector2(0, 1f);
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
        collidePoint.x = Mathf.Clamp(collidePoint.x, -0.25f, 24.1f);
        reflectPoints.Add(collidePoint);
    }
}
