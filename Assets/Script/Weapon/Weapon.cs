using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;


public class Weapon : MonoBehaviour 
{ 
    protected BaseStatList baseStat;
    protected SpriteRenderer weaponSprite;

    public StatModifier ATK;
    public StatModifier ATKSpeed;
    public StatModifier Accuracy ;                
    public StatModifier ATKRange ;     
    public StatModifier AttacVel ;


 


    public Transform shootPoint;
    [SerializeField]
    protected GameObject projectTile;
    [SerializeField]
    public AttackBurst attackBurst;
    [HideInInspector]
    public AudioSource audioSource;
    public AudioClip attackSound;
   



    protected string projectTileGUID;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = attackSound;
        projectTileGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(projectTile));
        weaponSprite = GetComponentInChildren<SpriteRenderer>();
      
    }

    public void FlipCheck(Vector2 dir)
    {
        transform.rotation = Quaternion.Euler(dir.x < 0 ? 180 : 0, transform.rotation.y, transform.rotation.z);
        
    }
    public void OnEquip(CombatController combatController,Transform weaponHolder)
    {
        
        GetComponent<Collider2D>().enabled = false;
        combatController.GetProjectilePool(projectTileGUID, projectTile);
        combatController.OnPerformAttack += audioSource.Play;
        combatController.attackPoint = shootPoint;



        combatController.ATK.AddModifier(ATK);
        combatController.ATKSpeed.AddModifier(ATKSpeed);
        transform.parent = weaponHolder;
        transform.localPosition = Vector2.zero;
        transform.localRotation = Quaternion.identity;
       
    }

    public void OnUnEquip(CombatController combatController)
    {
        combatController.ATK.RemoveModifier(ATK);
        combatController.ATKSpeed.RemoveModifier(ATKSpeed);
        GetComponent<Collider2D>().enabled = true;
        combatController.ATKSpeed.RemoveModifier(ATKSpeed);
        combatController.ReleaseProjectilePool(projectTileGUID);
        combatController.OnPerformAttack -= audioSource.Play;
        transform.parent = null;
    }

    public void SpriteControl(bool flip)
    {
        weaponSprite.flipX = flip;
    }
    
    
}
