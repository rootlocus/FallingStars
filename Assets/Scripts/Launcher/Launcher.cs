using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

public class Launcher : MonoBehaviour
{
    public static event EventHandler OnFireCombo;

    [SerializeField] private Transform launchPosition;
    [SerializeField] private Transform projectilePrefab;
    [SerializeField] private SpriteRenderer currentOrbSprite;
    [SerializeField] private SpriteRenderer nextOrbSprite;
    [SerializeField] private List<OrbTypeSO> orbTypes;
    [SerializeField] private AudioClip launchSoundClip;
    [SerializeField] private AudioClip reloadSoundClip;

    private OrbTypeSO currentOrbType;
    private OrbTypeSO nextOrbType;
    private Projectile currentProjectile;
    private enum State
    {
        Initialize,
        Ready,
        Wait,
        Pause,
        Reload,
    }
    private State currentState;
    private State nextState;
    private float stateTimer;
    

    private void Start() 
    {
        Projectile.OnProjectileStop += Projectile_OnProjectileStop;
        LevelState.Instance.OnStateLose += LevelState_OnStateLose;
        ComboSystem.OnMaxComboTriggered += ComboSystem_OnMaxComboTriggered;
        
        nextOrbType = GetRandomOrb();
        currentOrbType = GetRandomOrb();
        currentOrbSprite.sprite = currentOrbType.sprite;
        nextOrbSprite.sprite = nextOrbType.sprite;
    }

    private void ComboSystem_OnMaxComboTriggered(object sender, EventArgs e)
    {
        //TODO swap single bullet type
        // eventually move this into special projectile
        OnFireCombo?.Invoke(this, EventArgs.Empty);
    }

    private void Update() 
    {
        switch (currentState)
        {
            case State.Initialize:
                NextState();
                break;
            case State.Ready:
                if (Input.GetMouseButtonDown(0))
                {
                    AudioManager.Instance.PlaySFX(launchSoundClip);

                    Vector3 pointPosition = (Vector2) MouseWorld.GetPosition();
                    Vector3 direction = (pointPosition - transform.position).normalized;
                    //TODO clamp direction on left and right
                    
                    Transform orbTransform = Instantiate(projectilePrefab, launchPosition.position, Quaternion.identity);
                    currentProjectile = orbTransform.GetComponent<Projectile>();
                    currentProjectile.Setup(direction, currentOrbType);

                    currentOrbSprite.enabled = false;

                    NextState();
                    
                    LevelGrid.Instance.StartMoving();
                }
                break;
            case State.Wait:
                //  // maybe only iterate after shot ?
                break;
            case State.Reload:
                AudioManager.Instance.PlaySFX(reloadSoundClip);
                IterateNextOrb();
                currentOrbSprite.enabled = true;
                NextState();
                break;
            case State.Pause:
                break;
            default:
                break;
        }
    }

    private void NextState()
    {
        switch (currentState)
        {
            case State.Initialize:
                currentState = State.Ready;
                break;
            case State.Ready:
                currentState = State.Wait;
                break;
            case State.Reload:
                currentState = State.Ready;
                break;
            default:
                break;
        }
    }

    private void IterateNextOrb()
    {
        currentOrbType = nextOrbType;
        nextOrbType = GetRandomOrb();

        currentOrbSprite.sprite = currentOrbType.sprite;
        nextOrbSprite.sprite = nextOrbType.sprite;
    }

    private OrbTypeSO GetRandomOrb()
    {
        return orbTypes[Random.Range(0, orbTypes.Count)];
    }

    private void Projectile_OnProjectileStop(object sender, EventArgs e)
    {
        currentState = State.Reload;
    }

    private void LevelState_OnStateLose(object sender, EventArgs e)
    {
        currentState = State.Pause;
    }
}
