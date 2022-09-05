using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;
using Sirenix.OdinInspector;
using DG.Tweening;

public class Launcher : MonoBehaviour
{
    public static event EventHandler OnFireSpecial;
    public static event EventHandler<float> OnSpecialSwap;

    [Header("Setup")]
    [SerializeField] private Transform launchPosition;
    [SerializeField] private Transform projectilePrefab;
    [SerializeField] private Transform currentOrbTransform;
    [SerializeField] private Transform nextOrbTransform;
    [SerializeField] private GameObject highlightVignette;

    [Header("Mandatory Configurations")]
    [SerializeField] private List<OrbTypeSO> orbTypes;
    [SerializeField] private OrbTypeSO bombOrbType;

    [Header("Audio Configuration")]
    [SerializeField] private AudioClip launchSoundClip;
    [SerializeField] private AudioClip reloadSoundClip;
    [SerializeField] private AudioClip specialAmmoShowClip;

    [Header("Animation Configuration")]
    [SerializeField] private EntranceAnimation ropeAnimation;
    [SerializeField] private float swapDuration = 1.5f;

    private SpriteRenderer currentOrbSprite;
    private SpriteRenderer nextOrbSprite;
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
    private State onPausedState;
    private float stateTimer;
    private bool onSpecialMode;
    private bool isReloaded;


    private void Awake()
    {
        currentOrbSprite = currentOrbTransform.GetComponent<SpriteRenderer>();
        nextOrbSprite = nextOrbTransform.GetComponent<SpriteRenderer>();
    }

    private void Start() 
    {
        stateTimer = 0f;
        onSpecialMode = false;
        isReloaded = false;
        currentState = State.Pause;

        Projectile.OnProjectileStop += Projectile_OnProjectileStop;
        LevelState.Instance.OnStateLose += LevelState_OnStateLose;
        LevelState.Instance.OnStateStart += LevelState_OnStateStart;
        ComboSystem.OnMaxComboTriggered += ComboSystem_OnMaxComboTriggered;
        PauseUI.OnResumeMenu += PauseUI_OnResumeMenu;
        PauseUI.OnPauseMenu += PauseUI_OnPauseMenu;
    }

    private void Update() 
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer > 0f) {
            return;
        }

        switch (currentState)
        {
            case State.Initialize:
                nextOrbType = GetRandomOrb();
                currentOrbType = GetRandomOrb();
                currentOrbSprite.sprite = currentOrbType.sprite;
                nextOrbSprite.sprite = nextOrbType.sprite;

                ropeAnimation.MoveTransform();
                NextState();
                break;
            case State.Ready:
                if (Input.GetMouseButtonDown(0))
                {
                    PlayLauncherSound();

                    ThrowProjectile();
                    currentOrbSprite.enabled = false;
                    
                    NextState();
                    LevelGrid.Instance.StartMoving();
                }
                break;
            case State.Wait:
                //  // maybe only iterate after shot ?
                break;
            case State.Reload:
                if (isReloaded) return;

                AudioManager.Instance.PlaySFX(reloadSoundClip);

                if (onSpecialMode) 
                {
                    onSpecialMode = false;
                    SwapToSpecial();
                    NextState();
                } else {
                    isReloaded = true;
                    IterateNextOrb();
                }
                break;
            case State.Pause:
                break;
            default:
                break;
        }
    }

    private void ThrowProjectile()
    {
        Vector3 pointPosition = (Vector2) MouseWorld.GetPosition();
        pointPosition.y = Mathf.Clamp(pointPosition.y, 4f, 27.25f);

        Vector3 direction = (pointPosition - transform.position).normalized;
        
        Transform orbTransform = Instantiate(projectilePrefab, launchPosition.position, Quaternion.identity);
        currentProjectile = orbTransform.GetComponent<Projectile>();
        currentProjectile.Setup(direction, currentOrbType);
    }

    private void PlayLauncherSound()
    {
        AudioManager.Instance.PlaySFX(launchSoundClip);
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
        Vector3 previousPosition = nextOrbTransform.position;

        nextOrbTransform.DOMove(currentOrbTransform.position, 0.25f)
        .OnComplete(() => {
            //Shuffle orb and get new orb
            currentOrbType = nextOrbType;
            nextOrbType = GetRandomOrb();
            currentOrbSprite.sprite = currentOrbType.sprite;
            nextOrbSprite.sprite = nextOrbType.sprite;

            //Reset Position
            nextOrbTransform.position = previousPosition;
            currentOrbSprite.enabled = true;
            NextState();

            isReloaded = false;
        })
        .SetEase(Ease.Flash);
    }

    private OrbTypeSO GetRandomOrb()
    {
        return orbTypes[Random.Range(0, orbTypes.Count)];
    }

    [Button("Swap to Special")]
    private void SwapToSpecial()
    {
        currentOrbType = bombOrbType;
        currentOrbSprite.sprite = currentOrbType.sprite;
        currentOrbSprite.enabled = true;

        stateTimer = swapDuration;
        OnSpecialSwap?.Invoke(this, swapDuration);
        StartCoroutine(HighlightSpecialAmmo(swapDuration));

        AudioManager.Instance.PlaySFX(specialAmmoShowClip);
    }

    IEnumerator HighlightSpecialAmmo(float showDuration)
    {
        highlightVignette.SetActive(true);
        currentOrbSprite.material.EnableKeyword("FLICKER_ON");
        currentOrbSprite.material.EnableKeyword("HITEFFECT_ON");
        
        yield return new WaitForSeconds(showDuration);

        highlightVignette.SetActive(false);
        currentOrbSprite.material.DisableKeyword("FLICKER_ON");
        currentOrbSprite.material.DisableKeyword("HITEFFECT_ON");
    }

    private void Projectile_OnProjectileStop(object sender, EventArgs e)
    {
        currentState = State.Reload;
    }

    private void LevelState_OnStateLose(object sender, EventArgs e)
    {
        currentState = State.Pause;
    }
    
    private void ComboSystem_OnMaxComboTriggered(object sender, EventArgs e)
    {
        onSpecialMode = true;
        // OnFireSpecial?.Invoke(this, EventArgs.Empty);
    }

    private void LevelState_OnStateStart(object sender, EventArgs e)
    {
        currentState = State.Initialize;
    }
    
    private void PauseUI_OnPauseMenu(object sender, EventArgs e)
    {
        onPausedState = currentState;

        currentState = State.Pause;
    }

    private void PauseUI_OnResumeMenu(object sender, EventArgs e)
    {
        currentState = onPausedState;
    }
}
