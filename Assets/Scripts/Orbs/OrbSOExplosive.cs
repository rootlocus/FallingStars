using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/OrbSO/Explosive")]
public class OrbSOExplosive : OrbSO
{
    public static event EventHandler OnSpecialProjectileStop;
    public AudioClip soundClip;
    public float reverseSpeed = -10f;
    public float timeToHitMaxSpeed = 0.5f;
    private List<GridPosition> positions = new List<GridPosition> {
        new GridPosition(2, 0),
        new GridPosition(1, -1),
        new GridPosition(-1, - 1),
        new GridPosition(-2, 0),
        new GridPosition(-1, +1),
        new GridPosition(1, 1),
        
        // new GridPosition(2, -2),
        // new GridPosition(0, -2),
        // new GridPosition(-2, -2),
        // new GridPosition(-3, -1),
        // new GridPosition(-4, 0),
        // new GridPosition(-3, 1),
        // new GridPosition(-2, 2),
        // new GridPosition(0, 2),
        // new GridPosition(2, 2),
        // new GridPosition(3, 1),
        // new GridPosition(4, 0),
        // new GridPosition(3, -1),
    };


    public override void Execute(GridPosition gridPosition, OrbSO orbType)
    {
        OnSpecialProjectileStop?.Invoke(this, EventArgs.Empty); //TODO maybe remove

        DestroyOrbInArea(gridPosition);
        PushGridBack();
    }

    public void PushGridBack()
    {
        float defaultGridSpeed = LevelGrid.Instance.GetCurrentGridSpeed();
        float currentGridSpeed = LevelGrid.Instance.GetCurrentGridSpeed();

        DOVirtual.Float(currentGridSpeed, reverseSpeed, timeToHitMaxSpeed, v => {
            currentGridSpeed = v;
            LevelGrid.Instance.SetCurrentGridSpeed(currentGridSpeed);
            LevelGrid.Instance.MoveOrbRows();
        }).OnComplete(() => {
            LevelGrid.Instance.SetCurrentGridSpeed(defaultGridSpeed);
        });

        AudioManager.Instance.PlaySFX(soundClip);
    }

    public void DestroyOrbInArea(GridPosition gridPosition)
    {
        List<GridObject> adjacentGridObjects = LevelGrid.Instance.GetGridObjectsWithOrbOnSelectedPosition(gridPosition, positions);

        LevelGrid.Instance.DestroyAllOrbsInList(adjacentGridObjects);

        int fallenCount = LevelGrid.Instance.HandleIslandGrids();
    }
}
