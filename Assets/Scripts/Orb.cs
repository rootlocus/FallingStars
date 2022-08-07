using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    public bool isActivated = false;
    public OrbTypeSO myOrbType;
    

    public void Setup(OrbTypeSO orbSO)
    {
        myOrbType = orbSO;
        sprite.color = orbSO.color;
    }

    public OrbTypeSO GetOrbTypeSO() => myOrbType;

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
