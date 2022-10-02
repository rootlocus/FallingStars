using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherVisualGuide : MonoBehaviour
{
    // [SerializeField] private LineRenderer lineRenderer;
    [Header("Components/GameObject Dependencies")]
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private Transform orbShadowTransform;
    [SerializeField] private SpriteRenderer orbShadowSprite;
    [SerializeField] private LineRenderer lineRendererPrefab;

    [Header("Config")]
    [SerializeField] private float leftWallBoundary;
    [SerializeField] private float rightWallBoundary;
    [SerializeField] private float tutorialAbilityDuration = 30f;

    private bool isActivated;
    private bool isFullGuideActivated;
    private bool onPause;
    private List<Vector2> reflectPoints;
    private List<LineRenderer> renderLists;
    private Launcher launcher;
    private float lowerBoundary;
    private float upperBoundary;
    private float orbRadiusSize = 0.8f;
    private float activateDuration = 0f;


    private void Awake() 
    {
        launcher = GetComponent<Launcher>();
    }

    private void Start() 
    {
        renderLists = new List<LineRenderer>();
        reflectPoints = new List<Vector2>();
        isActivated = false;
        onPause = false;
        lowerBoundary = launcher.GetLowerLauncherBoundary();
        upperBoundary = launcher.GetUpperLauncherBoundary();

        PauseUI.OnResumeMenu += PauseUI_OnResumeMenu;
        PauseUI.OnPauseMenu += PauseUI_OnPauseMenu;
        LevelState.Instance.OnStateStart += LevelState_OnStateStart;
        LevelState.Instance.OnStateLose += LevelState_OnStateLose;
        GuideBuff.OnGuideBuffActivate += GuideBuff_OnGuideBuffActivate;
    }

    private void Update()
    {
        if (!isActivated || onPause) return;

        if (isFullGuideActivated)
        {
            ActivateOrbShadow();

            DrawFullGuideLine();

            CheckDeactivateFullGuide();
        }
        else {
            DrawNormalLine();
        }
    }

    private void CheckDeactivateFullGuide()
    {
        activateDuration -= Time.deltaTime;
        if (activateDuration <= 0)
        {
            isFullGuideActivated = false;
            DeactivateOrbShadow();
        }
    }

    private void DrawNormalLine()
    {
        Vector3 mousePosition = (Vector2) MouseWorld.GetPosition();
        mousePosition.y = Mathf.Clamp(mousePosition.y, lowerBoundary, upperBoundary);

        Vector2 initialDirection = mousePosition - transform.position;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, orbRadiusSize, initialDirection, float.MaxValue, hitLayer);

        reflectPoints.Clear();
        reflectPoints.Add(transform.position);

        if (hit) {
            reflectPoints.Add(hit.point);
        } else {
            reflectPoints.Add(mousePosition);
        }

        DeactivateExtraLineRenderer();
        CreateUpdateLineRenderer();

    }

    private void DrawFullGuideLine()
    {
        Vector3 mousePosition = (Vector2) MouseWorld.GetPosition();
        mousePosition.y = Mathf.Clamp(mousePosition.y, lowerBoundary, upperBoundary);

        Vector2 initialDirection = mousePosition - transform.position;

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, orbRadiusSize, initialDirection, float.MaxValue, hitLayer);

        reflectPoints.Clear();
        reflectPoints.Add(transform.position);

        if (!hit) return;

        HandleCollisions(initialDirection, hit);
        DeactivateExtraLineRenderer();
        CreateUpdateLineRenderer();

        DrawShadow();
    }

    private void CreateUpdateLineRenderer()
    {
        for (int i = 0; i < reflectPoints.Count - 1; i++)
        {
            bool shouldSpawnRenderer = i > renderLists.Count - 1;

            if (shouldSpawnRenderer)
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
            reflectPoints.Add(hit.centroid);
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
        orbShadowSprite.sprite = launcher.GetCurrentSprite();

        Vector2 lastPosition = reflectPoints[reflectPoints.Count - 1];
        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(lastPosition);

        Vector3 predictivePosition = LevelGrid.Instance.GetWorldPositionCenter(gridPosition);
        orbShadowTransform.position = predictivePosition;
    }

    //TODO Add a fail safe probably
    private void ReflectArrowOnWall(Vector3 initialDir, Vector3 startPosition)
    {
        Vector3 reflectDirection = Vector3.Reflect(initialDir, Vector3.right);

        //Because colliding with the collided wall
        float centerXPosition = 10.5f;
        Vector3 readjustedPosition = startPosition;
        readjustedPosition.x += startPosition.x < centerXPosition ? orbRadiusSize : -orbRadiusSize;

        RaycastHit2D reflectHit = Physics2D.CircleCast(readjustedPosition, orbRadiusSize, reflectDirection, float.MaxValue, hitLayer);
        if (!reflectHit) return;

        HandleCollisions(reflectDirection, reflectHit);
    }

    private void ActivateOrbShadow() => orbShadowSprite.enabled = true;

    private void DeactivateOrbShadow() => orbShadowSprite.enabled = false;

#region Events
    private void PauseUI_OnPauseMenu(object sender, EventArgs e)
    {
        onPause = true;
    }

    private void PauseUI_OnResumeMenu(object sender, EventArgs e)
    {
        onPause = false;
    }
    
    private void LevelState_OnStateStart(object sender, EventArgs e)
    {
        isActivated = true;

        isFullGuideActivated = true;
        activateDuration = tutorialAbilityDuration;
        ActivateOrbShadow();
    }
    
    private void LevelState_OnStateLose(object sender, EventArgs e)
    {
        isActivated = false;
        //todo deactivate  visual
    }

    private void GuideBuff_OnGuideBuffActivate(object sender, float duration)
    {
        isFullGuideActivated = true;
        activateDuration = duration;
    }
#endregion

}
