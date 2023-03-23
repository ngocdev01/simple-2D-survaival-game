using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(CharacterController2D))]
public class Enemy : MonoBehaviour
{
    protected Animator animator;
    protected CharacterController2D characterController;
    protected SpriteRenderer spriteRenderer;
    [SerializeField]
    protected GameObject projectTile;
    public CombatController combatController;



    protected GameObject PLayer { get => GameManager.Instance.player; }
    [SerializeField]
    [Range(0f, 10f)]
    protected float detectRange;

    [SerializeField]
    [Range(0f, 10f)]
    protected float shootAngle;

    [SerializeField]
    [Range(0f, 10f)]
    protected float attackRange;
    protected bool hasTriggered;
    [SerializeField]
    protected BaseStatList baseStat;
    protected bool isFlipped;
    public Transform attackPoint;
    private string projectTileGUID;

    [SerializeField]
    public bool baseSpriteIsLeft;
    private bool isDead;

    public LayerMask targetLayer;

    protected Action onInRange;
    protected Action onAttackRange;
    public Action<Enemy> onDead;

    private Stat ATK;
    private Stat HPMax;
    private Stat Speed;
    private Stat ATKSpeed;
    private Stat Accuracy;
    private Stat ATKRange;
    private Stat BulletSpeed;
    private DynamicStat HP;


    public AttackBurst attackBurst;
    protected void Awake()
    {
        isFlipped = baseSpriteIsLeft;
        characterController = GetComponent<CharacterController2D>();
        combatController = GetComponentInChildren<CombatController>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        onInRange += Chase;
        onAttackRange += OnAttackStart;
        onDead += OnDead;
        projectTileGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(projectTile));
        combatController.GetProjectilePool(projectTileGUID, projectTile);

        Init();
    }

    private void Init()
    {
        HPMax = new Stat(baseStat.GetStatValue(StatName.HP));
        HP = new DynamicStat(HPMax);
        ATK = new Stat(baseStat.GetStatValue(StatName.ATK));
        ATKSpeed = new Stat(baseStat.GetStatValue(StatName.ATKSPEED));
        Speed = new Stat(baseStat.GetStatValue(StatName.SPEED));
        BulletSpeed = new Stat(baseStat.GetStatValue(StatName.BULLETSPEED));
        ATKRange = new Stat(baseStat.GetStatValue(StatName.ATKRANGE));
        HP.OnStatReachMinValue += OnZeroHP;
        combatController.OnHit += OnGetHit;
        combatController.InitController(ATK, ATKSpeed, Accuracy, ATKRange,BulletSpeed, HP, targetLayer);
      
    }

    public void Buff()
    {

    }

    void OnAttackStart()
    {
        combatController.OnAttackStart(attackBurst, (PLayer.transform.position - attackPoint.position).normalized, 40f);
    }
    void OnGetHit()
    {
        animator.SetTrigger("Hit");
    }
    void OnZeroHP(DynamicStat Hp)
    {
        if (isDead) return;
        onDead?.Invoke(this);
    }

    private void OnDead(Enemy enemy)
    {
        isDead = true;
        animator.SetBool("Dead", true);
        characterController.Move(Vector2.zero, Speed.Value);
        Destroy(this.gameObject, 2);
    }

   


    protected void Update()
    {
        if(isDead)
        {
            
            return;
        }
        UpdateDirection((PLayer.transform.position - transform.position).normalized);
        UpdateDetect();
        OnUpdate();

    }

    

    void UpdateDetect()
    {
        if ((InRange || hasTriggered) && !InAttackRange)
        {
            onInRange?.Invoke();
        }
        else if (InAttackRange)
        {
            onAttackRange?.Invoke();
        } else
        {
            characterController.Move(Vector2.zero, Speed.Value);
        }

    }
    protected void OnUpdate()
    {
        
    }

    public void GetDamage(float amout)
    {
        animator.SetTrigger("Hit");
        HP.Value -= amout;
    }
    protected virtual void Chase()
    {
        Vector2 moveDir = (PLayer.transform.position - transform.position).normalized;
        characterController.Move(moveDir,Speed.Value);
    }

    protected bool InRange
    {
        get => (PLayer.transform.position - transform.position).sqrMagnitude < detectRange * detectRange;
    }

    protected bool InAttackRange
    {

        get => (PLayer.transform.position - transform.position).sqrMagnitude < attackRange * attackRange;
    }

   

    private void UpdateDirection(Vector2 moveDir)
    {
        if (InRange)
        {
            bool isFlip = baseSpriteIsLeft ? (moveDir.x > 0) : (moveDir.x < 0); ;
            if (isFlip != isFlipped)
            {
                isFlipped = isFlip;
                spriteRenderer.flipX = isFlip;
            }
           
        }
    }

    
    

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endif

}
