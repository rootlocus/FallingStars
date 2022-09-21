using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class Orb : MonoBehaviour
{
    public bool isAbilityActivated = false;
    public OrbTypeSO myOrbType;

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private BoxCollider2D orbCollider;
    [SerializeField] private float duration;
    [SerializeField] private Animator animator;
    private float delayBeforeKill = 1f;
    private Action<Orb> killAction;
    private const string IDLE = "Idle";
    private const string BURST = "Burst";
    

    private void Update() 
    {
        if (!isAbilityActivated) return;
        duration -= Time.deltaTime;

        if (duration <= 0)
        {
            DisableIsAbilityActivated();
        }
    }

    public void Setup(OrbTypeSO orbSO, Action<Orb> _killAction)
    {
        orbCollider.enabled = true;
        sprite.sprite = orbSO.sprite;
        animator.runtimeAnimatorController = orbSO.orbAnimation;
        
        myOrbType = orbSO;
        ChangeAnimationState(IDLE);

        killAction = _killAction;
    }

    public OrbTypeSO GetOrbTypeSO() => myOrbType;

    public void Destroy()
    {
        ChangeAnimationState(BURST);

        orbCollider.enabled = false;

        if (isAbilityActivated)
        {
            BuffDebuffManager.Instance.ActivateRandomBuff();
        }
        DisableIsAbilityActivated();

        StartCoroutine(CoDestroySelf(delayBeforeKill));
    }

    IEnumerator CoDestroySelf(float duration)
    {
        yield return new WaitForSeconds(duration);

        killAction(this);
    }

    public void FallDown()
    {
        int fallPoints = 50;
        orbCollider.enabled = false;

        this.transform.DOMoveY(-2f, 0.5f)
            .SetEase(Ease.OutFlash)
            .OnComplete(() => {
                Destroy();
                ScoreManager.Instance.Create(this.transform.position + new Vector3(0f, 4f), fallPoints);
            });
    }

    public void FallDownWithoutPoints()
    {
        orbCollider.enabled = false;

        this.transform.DOMoveY(-2f, 0.5f)
            .SetEase(Ease.OutFlash)
            .OnComplete(() => {
                Destroy();
            });
    }

    public void DisableIsAbilityActivated()
    {
        isAbilityActivated = false;
        sprite.material.DisableKeyword("SHAKEUV_ON");
    }

    public void EnableIsAbilityActivated(float duration)
    {
        this.duration = duration; 
        isAbilityActivated = true;
        sprite.material.EnableKeyword("SHAKEUV_ON");
    }

    private void ChangeAnimationState(string animationState)
    {
        animator.Play(animationState);
    }
}
