using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Stat
{
    public StatName statName;

    public float baseValue;
    public Action<Stat> OnStatChanged;
    private float value;

    private List<StatModifier> statModifiers;

    public Stat(BaseStat baseStat)
    {
        statModifiers = new List<StatModifier>();
        this.statName = baseStat.statName;
        this.BaseValue = baseStat.value;
        
    }

    public float BaseValue
    {
        get
        {
            return baseValue;
        }
        set
        {
            baseValue = value;
            RecaculateValue();
        }
    }
    public float Value
    {
        get { return value; }
    }

    public void AddModifier(StatModifier statModifier)
    {
        statModifiers.Add(statModifier);
        RecaculateValue();
    }
    public void RemoveModifier(StatModifier statModifier)
    {
        statModifiers.Remove(statModifier);
        RecaculateValue();
    }
    private void RecaculateValue()
    {
        
        float percent = 0;
        float value = baseValue;
        foreach(var statModifier in statModifiers)
        {
            if (statModifier.isPercent) percent += statModifier.value;
            else value += statModifier.value;
        }

        this.value = value + (value / 100 * percent);

        OnStatChanged?.Invoke(this);
    }




}

