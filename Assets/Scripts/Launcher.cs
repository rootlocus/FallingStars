using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] private Transform launchPosition;
    [SerializeField] private Transform projectilePrefab;
    [SerializeField] private SpriteRenderer currentOrbSprite;
    [SerializeField] private SpriteRenderer nextOrbSprite;
    [SerializeField] private List<OrbTypeSO> orbTypes;

    private OrbTypeSO currentOrbType;
    private OrbTypeSO nextOrbType;
    private Projectile currentProjectile;

    
    private void Start() 
    {
        nextOrbType = orbTypes[Random.Range(0, orbTypes.Count)];
        currentOrbType = orbTypes[Random.Range(0, orbTypes.Count)];
        currentOrbSprite.color = currentOrbType.color;
        nextOrbSprite.color = nextOrbType.color;
    }

    private void Update() 
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pointPosition = (Vector2) MouseWorld.GetPosition();
            Vector3 direction = (pointPosition - transform.position).normalized;
            //TODO clamp direction on left and right
            

            Transform orbTransform = Instantiate(projectilePrefab, launchPosition.position, Quaternion.identity);

            currentProjectile = orbTransform.GetComponent<Projectile>();
            currentProjectile.Setup(direction, currentOrbType);

            IterateNextOrb();
        }
    }

    private void IterateNextOrb()
    {
        currentOrbType = nextOrbType;
        nextOrbType = orbTypes[Random.Range(0, orbTypes.Count)];

        currentOrbSprite.color = currentOrbType.color;
        nextOrbSprite.color = nextOrbType.color;
    }


}
