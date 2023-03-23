using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(Collider2D))]
public class CombatController : MonoBehaviour, IDamageable
{
    private Collider2D hitBox;
    private LayerMask targetLayer;
 
    private ObjectPool<GameObject> pool;
    public Stat ATK { get; set; }
    public Stat ATKSpeed { get; set; }
    public Stat Accuracy { get; set; }
    public Stat BulletSpeed { get; set; }

    public Stat ATKRange { get; set; }

    public Stat AttacVel { get; set; }
    public DynamicStat HP { get; set; }

  

    private bool isPerormingAttack;
    private float lastTimeAttack;
    private int attackNumber;
    public Action OnHit;
    public Action OnPerformAttack;
    public Transform attackPoint;
    public Vector2 attackDir;

    public void InitController(Stat ATK, Stat ATKSpeed, Stat Accuracy, Stat ATKRange,Stat BulletSpeed, DynamicStat HP, LayerMask layerMask)
    {
        this.ATK = ATK;
        this.ATKSpeed = ATKSpeed;
        this.Accuracy = Accuracy;
        this.ATKRange = ATKRange;
        this.HP = HP;
        this.targetLayer = layerMask;
        this.BulletSpeed = BulletSpeed;
    }

    public void GetProjectilePool(string GUID, GameObject prefab)
    {
        pool = GameObjectPool.Instance.TryGetPool(GUID, prefab);
    }
    public void ReleaseProjectilePool(string GUID)
    {
        GameObjectPool.Instance.RequestDisposePool(GUID);
    }

    public void OnAttackStart(AttackBurst attackBurst, Vector2 attackDir, float acc)
    {
        this.attackDir = attackDir;
        if (Time.time - lastTimeAttack < 1/ATKSpeed.Value|| isPerormingAttack) return;
        attackNumber = attackBurst.bustTime;

        isPerormingAttack = true;
        StartCoroutine(burstCoroutine(attackBurst, new WaitForSeconds(1 / 10f), acc));

    }

    IEnumerator burstCoroutine(AttackBurst attackBurst, WaitForSeconds delay, float acc)
    {
        while (attackNumber > 0)
        {
            OnAttackPerform(attackBurst, attackDir, acc);
            attackNumber--;
            yield return delay;
        }
        OnAttackEnd();
    }

    
    void OnAttackPerform(AttackBurst attackBurst, Vector2 dir, float acc)
    {
        GetAttack(attackBurst, dir, acc);
        OnPerformAttack?.Invoke();

    }

    void OnAttackEnd()
    {
        lastTimeAttack = Time.time;
        attackNumber = 0;
        isPerormingAttack = false;
    }

    void OnAttackHit(ProjectTile projectTile, GameObject gameObject,ObjectPool<GameObject> pool)
    {

        if (gameObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.GetDamage(ATK.Value);
        }
        pool.Release(projectTile.gameObject);

    }

    void OnOutOfRange(ProjectTile projectTile, ObjectPool<GameObject> pool)
    {
        pool.Release(projectTile.gameObject);
    }

    void GetAttack(AttackBurst attackBurst, Vector2 attackDir,float acc)
    {
        float[] burstAngle = attackBurst.GetBustAngle(acc);
        foreach(var angle in burstAngle)
        {
            var obj = pool.Get().GetComponent<ProjectTile>();
            var rotate =Mathf.Rad2Deg * Mathf.Atan2(attackDir.y, attackDir.x) + angle;
            obj.Init(attackPoint.transform.position, Quaternion.Euler(0, 0, rotate),
               BulletSpeed.Value, ATKRange.Value, targetLayer);
            obj.pool =  pool;
            obj.OnHit +=  OnAttackHit;
            obj.OnOutOfRange+= OnOutOfRange;
        }
    }

    public void GetDamage(float amout)
    {
        HP.Value -= amout;
        OnHit?.Invoke();
    }

}
