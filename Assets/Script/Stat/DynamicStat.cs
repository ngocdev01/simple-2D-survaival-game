using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DynamicStat 
{
    public Stat statBaseValue;
    private float statMinValue;
    private float value;
    public Action<DynamicStat> OnStatReachMinValue;
    public Action<DynamicStat> OnStatReachMaxValue;
    public Action<DynamicStat> OnValueChanged;
    public DynamicStat(Stat statBaseValue, float statMinValue = 0)
    {
        this.statBaseValue = statBaseValue;
        this.statMinValue = statMinValue;
        this.value = statBaseValue.Value;
    }

    

    public float Value
    {
        get { return value; }
        set
        {
           
            if(value <= statMinValue)
            {
                this.value = statMinValue;
                OnStatReachMinValue?.Invoke(this);
            }
            else if(value >= statBaseValue.Value)
            {
                this.value = statBaseValue.Value;
                OnStatReachMaxValue?.Invoke(this);
            }
            else
            {
                this.value = value;
            }
            OnValueChanged?.Invoke(this);
        }
    }
}
