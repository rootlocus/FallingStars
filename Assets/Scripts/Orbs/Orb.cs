using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Orb : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private BoxCollider2D orbCollider;
    public bool isActivated = false;
    public OrbTypeSO myOrbType;
    private Action<Orb> killAction;
    

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
        killAction(this);
    }

}
