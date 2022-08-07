using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Transform orbPrefab;
    [SerializeField] private SpriteRenderer sprite;

    private float moveSpeed = 50f;
    private bool isMove;
    private Vector3 direction;
    private OrbTypeSO orbType;
    

    public void Setup(Vector3 direction, OrbTypeSO orbType)
    {
        this.direction = direction;
        this.orbType = orbType;
        sprite.color = orbType.color;

        isMove = true;
    }

    private void Update() 
    {
        if (isMove) {
            Vector3 distanceTravel = direction * moveSpeed * Time.deltaTime;

            RaycastHit2D predictionHit = Physics2D.CircleCast(transform.position, 0.5f, direction, Vector3.Distance(transform.position + distanceTravel, transform.position), LevelGrid.Instance.GetOrbMask());
            
            if (predictionHit.collider != null) {
                isMove = false;
                GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

                LevelGrid.Instance.SpawnOrbObjectOnGridPosition(gridPosition, orbType);
                Destroy(gameObject);
            } else {
                transform.position += distanceTravel;
            }
        }
    }

    private void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        string collidedTag = other.gameObject.tag;
        if (collidedTag == "Wall") {
            direction = Vector3.Reflect(direction, Vector3.right);
        }

    }

    private void OnDrawGizmos() {
        if (!Application.isPlaying) return;

        Vector3 distanceTravel = direction * moveSpeed * Time.deltaTime;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + distanceTravel, 0.5f);
    }
}
