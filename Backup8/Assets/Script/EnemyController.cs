﻿using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour {

    [HideInInspector]
    public int loverNo;
    public LayerMask treeMask;

    Animator animator;
    public GameObject tree;
    Transform[] treesTransform;
    MateController mc;
    public AudioSource aSource;
    Vector2 boxCol;
    Vector2 lovedCol;
    Vector2 newDest;

    Transform rivalDest;

    bool[] wentTree;
    bool sawRival;
    bool goingTree;
    bool wClick;

    int chosenTree;

    float passedTime;

	void Start () 
    {
        animator = GetComponent<Animator>();

        tree = GameObject.FindGameObjectWithTag("TreeTree");

        treesTransform = new Transform[tree.transform.childCount];

        for (int i = 0; i < treesTransform.Length; i++)
            treesTransform[i] = tree.transform.GetChild(i);

        wentTree = new bool[tree.transform.childCount];

        for (int i = 0; i < wentTree.Length; i++)
            wentTree[i] = false;

        lovedCol = new Vector2(2.1f, 1.8f);

        boxCol = new Vector2(5f, 5f);
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCol, 0, Vector2.zero, 2.5f, treeMask);

        chosenTree = Int32.Parse(hit.collider.name);
        mc = null;
        loverNo = -1;
        passedTime = 0;
        aSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () 
    {

        print("choosenTree : " + chosenTree + " goingTree " + goingTree);

        if (!wentTree[chosenTree] && !goingTree)
        {
            newDest = Vector2.Lerp(transform.position, treesTransform[chosenTree].position,
              (Vector2.Distance(transform.position, treesTransform[chosenTree].position) - 1.08f) /
              Vector2.Distance(transform.position, treesTransform[chosenTree].position));
            goingTree = true;
        }

        if(!wentTree[chosenTree] && goingTree)
        {
            if(findAniAndMove(newDest, 0.03f))
            {
                wentTree[chosenTree] = true;
                goingTree = false;
            }
        }
           
        if(wentTree[chosenTree] && !goingTree)
        {
            RaycastHit2D[] hit = Physics2D.BoxCastAll(transform.position, lovedCol, 0, Vector2.zero);

            for(int i = 0; i < hit.Length; i++)
            {
                if(hit[i].collider.CompareTag("Mate"))
                {
                    if (mc == null)
                        mc = hit[i].collider.GetComponent<MateController>();

                    print("loverno " + loverNo);
                    if(loverNo < 0)
                    {
                        loverNo = mc.lovers;
                        mc.lovers++;
                    }
                        
                    mc.hitPlace = transform.position;

                    if (!mc.affection.ContainsKey(loverNo))
                    {
                        mc.affection.Add(loverNo, 1);
                        aSource.PlayOneShot(aSource.clip);
                    }
                    else if (passedTime > 0.8f)
                    {
                        mc.affection[loverNo] += 1;
                        passedTime = 0;
                        aSource.PlayOneShot(aSource.clip);
                    }
                    else
                        passedTime += Time.deltaTime;

                    mc.hitConfirmed = true;
                }

                if (hit[i].collider.CompareTag("Enemy") || hit[i].collider.CompareTag("Player"))
                {
                    sawRival = true;
                    rivalDest = hit[i].collider.transform;
                    goingTree = false;
                }

            }

            if(mc == null || !mc.hitConfirmed)
            {
                chosenTree = UnityEngine.Random.Range(0, 6);
                goingTree = false;
            }
        }
	}

    bool findAniAndMove(Vector2 dest, float t)
    {
        Vector2 newPos = Vector2.MoveTowards((Vector2)transform.position, dest, t);

        if (transform.position.y - newPos.y != 0)
            animator.SetInteger("Direction", (transform.position.y - newPos.y < 0) ? 0 : 2);
        else if (transform.position.x - newPos.x != 0)
            animator.SetInteger("Direction", (transform.position.x - newPos.x < 0) ? 1 : 3);
        else
            animator.SetInteger("Direction", -1);

        bool lol = Vector2.Equals(newPos, (Vector2)transform.position);

        transform.position = newPos;

        return lol;
    }

}