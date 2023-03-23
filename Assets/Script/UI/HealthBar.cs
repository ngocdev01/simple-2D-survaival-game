using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;
    private void Awake()
    {
        slider = GetComponent<Slider>();
    }
    public void SetMaxHelth(float helth)
    {
       
        slider.maxValue = helth;
    }

    public void SetHelth(float helth)
    {
       
        slider.value = helth;
    }
}
