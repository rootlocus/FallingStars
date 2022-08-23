using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboMeterUI : MonoBehaviour
{
    [SerializeField] private Slider meterGauge;

    public void Setup(int minValue, int maxValue)
    {
        meterGauge.minValue = minValue;
        meterGauge.maxValue = maxValue;
    }

    public void SetGauge(float amount)
    {
        meterGauge.value = amount;
    }
}
