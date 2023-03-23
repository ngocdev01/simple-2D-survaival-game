using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent (typeof(Collider2D))]
public class ProjectTile : MonoBehaviour
{
    private float velocity;
    private LayerMask targetLayer;
    private Vector2 startPos;
    private float range;
    private Collider2D col;
    private Rigidbody2D rb2d;

    public Action<ProjectTile,GameObject,ObjectPool<GameObject>> OnHit;
    public Action<ProjectTile, ObjectPool<GameObject>> OnOutOfRange;
    public  ObjectPool<GameObject>  pool;
   

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        rb2d.gravityScale = 0;
        col = rb2d.GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    public void Init(Vector2 pos,Quaternion rotate,float velocity,float range,LayerMask hitLayerMask)
    {
        transform.position = pos;
        transform.rotation = rotate;

        this.startPos = pos;
        this.velocity = velocity;
     
        this.range = range;
        this.targetLayer = hitLayerMask;
    
        
    }

    private void FixedUpdate()
    {
        if(((Vector2)transform.position-startPos).sqrMagnitude>range*range)
        {
            OnOutOfRange?.Invoke(this,pool);
        }
        rb2d.MovePosition(transform.position + transform.right * velocity * Time.deltaTime);
    }
    private void OnDisable()
    {
        OnHit = null;
        OnOutOfRange = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if ((1 << collision.gameObject.layer & targetLayer) != 0)
        {         
             OnHit?.Invoke(this, collision.gameObject,pool);
        }
    }
   
}
