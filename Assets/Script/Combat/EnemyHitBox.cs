using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour,IDamageable
{
    private Enemy enemy;
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }
    public void GetDamage(float amout)
    {
       
        enemy.GetDamage(amout);
    }
}
