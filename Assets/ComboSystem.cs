using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    public static event EventHandler OnMaxComboTriggered;
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
        Launcher.OnFireCombo += Launcher_OnFireCombo;
    }

    private void LevelGrid_OnUnsuccessfulMatch(object sender, EventArgs e)
    {
        ResetComboCount();
    }

    private void Launcher_OnFireCombo(object sender, EventArgs e)
    {
        ResetComboCount();
    }

    private void LevelGrid_OnSuccessfulMatch(object sender, int e)
    {
        AddCurrentCombo();

        if (maxComboTrigger == currentCombo)
        {
            OnMaxComboTriggered?.Invoke(this, EventArgs.Empty);
        }
    }

    private void AddCurrentCombo()
    {
        if (currentCombo < maxComboTrigger) {
            currentCombo++;
        }

        comboUI.SetGauge(currentCombo);
    }

    private void ResetComboCount()
    {
        currentCombo = 0;

        comboUI.SetGauge(currentCombo);
    }
}
