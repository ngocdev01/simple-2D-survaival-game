using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat",menuName = "BaseStatList")]
public class BaseStatList : ScriptableObject
{
    public BaseStat[] stats;
    public  BaseStat GetStatValue(StatName statName)
    {
        var result = stats.Where(stat => stat.statName == statName).FirstOrDefault();
        if (result == null) throw new ArgumentNullException($"Stat name: {statName} does not exists ");
        return result;
    }
}

[Serializable]
public class BaseStat
{
    public StatName statName;
    public float value;
}

public enum StatName
{
    HP,
    ATK,
    ATKSPEED,
    SPEED,
    CRITRATE,
    CRITDMG,
    ACCURACY,
    BULLETSPEED,
    ATKRANGE,
}
