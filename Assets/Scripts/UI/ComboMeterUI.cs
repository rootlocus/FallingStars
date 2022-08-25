using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ComboMeterUI : MonoBehaviour
{
    [SerializeField] private Slider meterGauge;
    [SerializeField] private Transform meterLabel;


    public void Setup(int minValue, int maxValue)
    {
        meterGauge.minValue = minValue;
        meterGauge.maxValue = maxValue;
    }

    public void SetGauge(float amount)
    {
        meterGauge.value = amount;
    }

    // private void Update() 
    // {
    //     float meterValue = meterGauge.value;

    //     if (meterValue == 0)
    //     {
    //         return;
    //     }

    // }
}
