using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    public static event EventHandler OnMaxComboTriggered;
    [SerializeField] private AudioClip[] comboClips;
    [SerializeField] private int maxComboTrigger;
    [SerializeField] private ComboMeterUI comboUI;

    private int currentCombo;


    private void Start() 
    {
        currentCombo = 0;
        comboUI.Setup(0, maxComboTrigger);
        comboUI.SetGauge(currentCombo);

        LevelGrid.Instance.OnSuccessfulMatch += LevelGrid_OnSuccessfulMatch;
        LevelGrid.Instance.OnUnsuccessfulMatch += LevelGrid_OnUnsuccessfulMatch;
        // Launcher.OnFireSpecial += Launcher_OnFireSpecial;
    }

    private void LevelGrid_OnUnsuccessfulMatch(object sender, EventArgs e)
    {
        ResetComboCount();
    }

    // private void Launcher_OnFireSpecial(object sender, EventArgs e)
    // {
    //     ResetComboCount();
    // }

    private void LevelGrid_OnSuccessfulMatch(object sender, LevelGrid.OnSuccessfulMatchArgs e)
    {
        AddCurrentCombo();
        if (maxComboTrigger == currentCombo)
        {
            ResetComboCount();
            OnMaxComboTriggered?.Invoke(this, EventArgs.Empty);
        }
    }

    private void AddCurrentCombo()
    {
        if (currentCombo < maxComboTrigger) {
            currentCombo++;
            if (currentCombo > 1)
            {
                AudioManager.Instance.PlaySFX(comboClips[currentCombo - 2]);
            }
        }

        comboUI.SetGauge(currentCombo);
    }

    private void ResetComboCount()
    {
        currentCombo = 0;

        comboUI.SetGauge(currentCombo);
    }
}
