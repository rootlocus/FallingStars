using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Orb : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    public bool isActivated = false;
    public OrbTypeSO myOrbType;
    private Action<Orb> killAction;
    

    public void Setup(OrbTypeSO orbSO, Action<Orb> _killAction)
    {
        myOrbType = orbSO;
        sprite.sprite = orbSO.sprite;

        killAction = _killAction;
    }

    public OrbTypeSO GetOrbTypeSO() => myOrbType;

    public void Destroy()
    {
        // Destroy(gameObject);

        killAction(this);
    }
}
