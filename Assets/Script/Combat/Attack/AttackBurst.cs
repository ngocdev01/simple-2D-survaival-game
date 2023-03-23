using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class AttackBurst
{

    public int bustTime;

    public int burstNumber;
    

    public float[] GetBustAngle(float angle)
    {
       
        angle /= 2;
        float[] bustAngle = new float[burstNumber];
        if (burstNumber == 1)
        {
            bustAngle[0] = 0;
            return bustAngle;
        }

        for(int i = 0; i < burstNumber; i++)
        {
            bustAngle[i] = Mathf.Lerp(-angle,angle,(float)i/burstNumber);   
        }
        return bustAngle;
    }
}
