using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static event EventHandler OnProjectileStop;
    public static event EventHandler OnSpecialProjectileStop;
    [SerializeField] private Transform orbPrefab;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private AudioClip reflectSoundClip;

    private float moveSpeed = 50f;
    private float projectileExitHeight = 28.5f;
    private bool isMove;
    private Vector3 direction;
    private OrbTypeSO orbType;
    

    private void Start() 
    {
        PauseUI.OnResumeMenu += PauseUI_OnResumeMenu;
        PauseUI.OnPauseMenu += PauseUI_OnPauseMenu;    
    }

    private void OnDestroy() 
    {
        PauseUI.OnResumeMenu -= PauseUI_OnResumeMenu;
        PauseUI.OnPauseMenu -= PauseUI_OnPauseMenu;
    }

    public void Setup(Vector3 direction, OrbTypeSO orbType)
    {
        this.direction = direction;
        this.orbType = orbType;
        sprite.sprite = orbType.sprite;

        isMove = true;
    }

    private void Update() 
    {
        if (isMove) {
            Vector3 distanceTravel = direction * moveSpeed * Time.deltaTime;

            RaycastHit2D predictionHit = Physics2D.CircleCast(transform.position, 0.5f, direction, Vector3.Distance(transform.position + distanceTravel, transform.position), LevelGrid.Instance.GetOrbMask());
            
            if (predictionHit.collider != null) 
            {
                isMove = false;
                GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
                if (orbType.type == OrbTypeSO.OrbType.Normal) // in future can put this in execute and scriptable object
                {
                    LevelGrid.Instance.AttachOrbToGrid(gridPosition, orbType);
                } else 
                {
                    OnSpecialProjectileStop?.Invoke(this, EventArgs.Empty);
                    LevelGrid.Instance.PushGridBack(gridPosition, orbType);
                }

                OnProjectileStop?.Invoke(this, EventArgs.Empty);

                Destroy(gameObject); // change this probably
            } else {
                transform.position += distanceTravel;

                if (transform.position.y > projectileExitHeight)
                {
                    OnProjectileStop?.Invoke(this, EventArgs.Empty);
                    Destroy(gameObject);
                }
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
            AudioManager.Instance.PlaySFX(reflectSoundClip);
            direction = Vector3.Reflect(direction, Vector3.right);
        }

    }

    private void PauseUI_OnPauseMenu(object sender, EventArgs e)
    {
        isMove = false;
    }

    private void PauseUI_OnResumeMenu(object sender, EventArgs e)
    {
        isMove = true;
    }

    private void OnDrawGizmos() {
        if (!Application.isPlaying) return;

        Vector3 distanceTravel = direction * moveSpeed * Time.deltaTime;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + distanceTravel, 0.5f);
    }
}
