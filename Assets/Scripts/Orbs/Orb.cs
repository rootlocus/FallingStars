using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Orb : MonoBehaviour
{
    public static event EventHandler OnRandomAbilityActivate;
    public bool isAbilityActivated = false;
    public OrbTypeSO myOrbType;

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private BoxCollider2D orbCollider;
    [SerializeField] private float duration;
    private Action<Orb> killAction;
    
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
        myOrbType = orbSO;
        sprite.sprite = orbSO.sprite;

        killAction = _killAction;
    }

    public OrbTypeSO GetOrbTypeSO() => myOrbType;

    public void Destroy()
    {
        orbCollider.enabled = false;

        if (isAbilityActivated)
        {
            OnRandomAbilityActivate?.Invoke(this, EventArgs.Empty);
        }
        DisableIsAbilityActivated();

        killAction(this);
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
}
