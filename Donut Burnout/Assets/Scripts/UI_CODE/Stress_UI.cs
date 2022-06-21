using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stress_UI : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetMaxStress(float stress)
    {
        slider.maxValue = stress;
        slider.value = stress;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetStress(float stress)
    {
        slider.value = stress;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

}
