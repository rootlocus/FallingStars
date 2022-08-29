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

    private void Start() 
    {
        isActivated = false;

        PauseController.OnResumeMenu += PauseController_OnResumeMenu;
        PauseController.OnPauseMenu += PauseController_OnPauseMenu;
        LevelState.Instance.OnStateStart += LevelState_OnStateStart;
    }

    private void Update() 
    {
        if (!isActivated) return;

        Vector3 mousePosition = (Vector2) MouseWorld.GetPosition();
        mousePosition.y = Mathf.Clamp(mousePosition.y, 4f, 27.25f);

        Vector2 dir = mousePosition - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, float.MaxValue, hitLayer);
        
        if (!hit) return;
        
        if (hit.transform.tag == "Wall")
        {
            // lineRenderer.positionCount = 3;

            //collide wall
            Vector2 collidePoint = hit.point;
            collidePoint.x = Mathf.Clamp(collidePoint.x, 1.5f, 19.5f);
            lineRenderer.SetPosition(1, collidePoint);

            //bounce
            // Vector2 reflectDirection = Vector3.Reflect(dir, Vector3.right);
            // RaycastHit2D hit2 = Physics2D.Raycast(collidePoint, reflectDirection, float.MaxValue, hitLayer);
            // if (!hit2) return;

            // Vector2 newColliderPoint = hit2.point - new Vector2(0, 1f);

            lineRenderer.SetPosition(1, collidePoint);
            // lineRenderer.SetPosition(2, newColliderPoint);
        }
        if (hit.transform.tag == "Orb")
        {
            Vector2 collidePoint = hit.point - new Vector2(0, 1f);
            lineRenderer.SetPosition(1, collidePoint);
        }
    }

    private void PauseController_OnPauseMenu(object sender, EventArgs e)
    {
        isActivated = false;
    }

    private void PauseController_OnResumeMenu(object sender, EventArgs e)
    {
        isActivated = true;
    }
    
    private void LevelState_OnStateStart(object sender, EventArgs e)
    {
        isActivated = true;
    }
}
