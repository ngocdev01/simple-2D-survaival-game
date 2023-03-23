using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatSystem : MonoBehaviour
{
    [SerializeField]
    private BaseStatList baseStat;
    private Stat[] Stats;

    private void Awake()
    {
        List<Stat> stats = new List<Stat>();
        foreach(var stat in baseStat.stats)
        {
            stats.Add(new Stat(stat));
        }
        Stats = stats.ToArray();
    }

    public Stat GetStat(StatName statName)
    {
        var stat =  Stats.Where(stat => stat.statName == statName).FirstOrDefault();
        return stat != null ? stat : 
            throw new ArgumentNullException($"Stat {statName} does not exist in {gameObject.name}");
    }


}


