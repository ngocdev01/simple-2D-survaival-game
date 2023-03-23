
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputController))]
[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private CharacterController2D characterController;
    private CombatController combatController;
    private InputController inputController;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    [HideInInspector]
    public GameObject pickUp;

    private bool fire;
    private bool isDead;
    private HealthBar healthBar;

    [SerializeField]
    private BaseStatList baseStat;

   
    private bool isFlipped;
    private Vector2 inputVector;
    private Vector2 mousePos;
    public Transform weaponHolder;

    public Weapon weapon;


    public Stat ATK;
    public Stat HPMax;
    public Stat Speed;
    public Stat ATKSpeed;
    public Stat Accuracy;
    public Stat ATKRange;
    public Stat BulletSpeed;

    public DynamicStat HP;

    public LayerMask targetLayerMask;


  
    private void Awake()
    {
        characterController = GetComponent<CharacterController2D>();
        inputController = GetComponent<InputController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        combatController = GetComponentInChildren<CombatController>();
        animator = GetComponent<Animator>();
        healthBar = GameManager.Instance.healthBar;
    }

    private void Start()
    {
        HPMax =new Stat( baseStat.GetStatValue(StatName.HP));
        HP = new DynamicStat(HPMax);
        ATK = new Stat(baseStat.GetStatValue(StatName.ATK));
        ATKSpeed = new Stat(baseStat.GetStatValue(StatName.ATKSPEED));
        Speed = new Stat(baseStat.GetStatValue(StatName.SPEED));
        BulletSpeed = new Stat(baseStat.GetStatValue(StatName.BULLETSPEED));
        ATKRange = new Stat(baseStat.GetStatValue(StatName.ATKRANGE));
        GameManager.Instance.healthBar.SetMaxHelth(HPMax.Value);
        GameManager.Instance.healthBar.SetHelth(HP.Value);

        HPMax.OnStatChanged += OnMaxHpChange;
        HP.OnStatReachMinValue += OnZeroHP;
        HP.OnValueChanged += OnHPChange;
        combatController.InitController(ATK, ATKSpeed, Accuracy, ATKRange,BulletSpeed, HP, targetLayerMask);
        weapon.OnEquip(combatController, weaponHolder);


    }
    void OnHPChange(DynamicStat hp)
    {
        GameManager.Instance.healthBar.SetHelth(hp.Value);
    }

    void OnMaxHpChange(Stat stat)
    {
        GameManager.Instance.healthBar.SetMaxHelth(stat.Value);
    }
    void  OnZeroHP(DynamicStat hp)
    {
        OnDead();
    }

    private void Update()
    {
        if (!isDead)
        {
            GartherInput();
            UpdateAnim();
            UpdateFire();
            UpdateInteract();
        }
        
        UpdateAim();


    }
    void UpdateInteract()
    {
        if(inputController.pickUpPresses && pickUp!=null)
        {
            if(pickUp.TryGetComponent(out Weapon weapon))
            {
                if (GameManager.Instance.clearList.Contains(weapon.gameObject))
                    GameManager.Instance.clearList.Remove(weapon.gameObject);
                this.weapon.OnUnEquip(combatController);
                GameManager.Instance.clearList.Add(this.weapon.gameObject);
                this.weapon = weapon;
                this.weapon.OnEquip(combatController,weaponHolder);           
            }
            else if (pickUp.TryGetComponent(out Potion potion))
            {
                this.HP.Value += potion.healValue;
                Destroy(potion.gameObject);
            }
        }
    }
    private void OnDead()
    {
        isDead = true;
       
        animator.SetBool("Dead", true);

        GameManager.Instance.GameOver();

    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            Movement();
        }
    }

    void UpdateFire()
    {
        if(fire)
        {
            combatController.OnAttackStart(weapon.attackBurst, 
                (mousePos - (Vector2)transform.position).normalized,10f);
        }
    }

   
    void UpdateAim()
    {
        Vector2 aimVec;
        bool needFlip;
        if(fire)
        {
            aimVec = mousePos - (Vector2)transform.position;        
        }
        else
        {
            aimVec = inputVector;          
        }
        aimVec.Normalize();
        needFlip = aimVec.x < 0;
        UpdateDirection(aimVec, needFlip);
        UpdateAimRotate(aimVec);
       
    }
    void UpdateAimRotate(Vector2 aimVec)
    {
        
        if(aimVec==Vector2.zero) aimVec = isFlipped?Vector2.left:Vector2.right;
        float rotateX = aimVec.x < 0 ? 180 : 0;
        float rotateZ = Mathf.Rad2Deg * Mathf.Atan2(aimVec.y, aimVec.x);
        rotateZ = aimVec.x < 0 ? -rotateZ : rotateZ;
        weaponHolder.transform.rotation = Quaternion.Euler(rotateX,0,rotateZ);
    }
    private void UpdateDirection(Vector2 aimVec, bool needFlip)
    {
        if (needFlip != isFlipped)
        {
            isFlipped = needFlip;
            spriteRenderer.flipX = needFlip;
        }
    }
        
    

    private void UpdateAnim()
    {

       
        if (inputVector.sqrMagnitude > 0)
        {
            animator.SetBool("Run", true);
            animator.SetBool("Idle", false);
        }
        else
        {
            animator.SetBool("Run", false);
            animator.SetBool("Idle", true);
        }
      
    }
   
 
   
    private void GartherInput()
    {
        mousePos = inputController.mousePosition;
        inputVector = inputController.controlVector;
        fire = inputController.fireButton;
    }

    private void Movement()
    {
        characterController.Move(inputVector, Speed.Value);
    }
    
}
