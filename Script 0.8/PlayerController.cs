﻿using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {

    Animator animator;
    Rigidbody2D rb;
    Vector2 boxCol;
    MateController mc;
    AudioSource boar;

    [HideInInspector]
    public int loverNo;

    public int speed = 5;
    public float stunTime = 1.5f;
    [HideInInspector]
    public bool stun;

    float passedStunTime = 0;

    void Start () 
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCol = new Vector2(2.1f, 1.8f);
        mc = null;
        loverNo = -1;
        stun = false;
        boar = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (!stun)
        {
            float moveHorizontal = Input.acceleration.x;
            float moveVertical = -Input.acceleration.y;

            moveHorizontal = Input.GetAxis("Horizontal");
            moveVertical = Input.GetAxis("Vertical");

            if (moveVertical != 0)
                animator.SetInteger("Direction", (moveVertical > 0) ? 0 : 2);
            else if (moveHorizontal != 0)
                animator.SetInteger("Direction", (moveHorizontal > 0) ? 1 : 3);
            else
                animator.SetInteger("Direction", -1);

            //TODO : normalized çalışmıyor
            rb.velocity = new Vector2(moveHorizontal, moveVertical) * speed;
        }
        else
            rb.velocity = Vector3.zero;
	}

    void Update()
    {
        if(Input.GetButtonUp("Fire1") && !stun)
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, boxCol, 0, Vector2.zero);
            boar.PlayOneShot(boar.clip);

            for(int i = 0; i < hits.Length; i++)
            {
                if(hits[i].transform.tag == "Mate")
                {
                    if (mc == null)
                        mc = hits[i].collider.GetComponent<MateController>();

                    if(loverNo == -1)
                    {
                        loverNo = mc.lovers;
                        mc.lovers++;
                    }
                        
                    mc.hitPlace = transform.position;

                    if (!mc.affection.ContainsKey(loverNo))
                        mc.affection.Add(loverNo, 1);
                    else
                    {
                        print("here");
                        (mc.affection[loverNo])++;
                    }
                    
                    
                    mc.hitConfirmed = true;
                    
                }
                    
            }
        }
        else if(stun)
        {
            if(passedStunTime >= stunTime)
            {
                passedStunTime = 0;
                stun = false;
            }
            else
                passedStunTime += Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        print(other.tag);
        if(other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}
