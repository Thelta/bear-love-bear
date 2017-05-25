using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour {

    [HideInInspector]
    public int loverNo;
    
    Animator animator;

    float passedRoarTime;
    public AudioSource aSource;

    MateController mc;

    Vector2 targetPos;

    Destination destinator;
    bool haveTarget;

    public GameObject tree;
    Transform[] treesTransform;
    bool[] wentTree;
    int chosenTree;

    List<Collider2D> targets;
    Vector2[] directions = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

    Transform stunStar;
    Vector2[] starPos = { new Vector2(0, 0.2f), new Vector2(0.2f, 0), new Vector2(0, -0.1f), new Vector2(-0.2f, 0) };
    public float stunTime = 1.5f;
    [HideInInspector]
    public bool stun;
    float passedStunTime = 0;

    public bool fight;

    
    void Awake()
    {
        animator = GetComponent<Animator>();

        tree = GameObject.FindGameObjectWithTag("TreeTree");

        mc = GameObject.FindGameObjectWithTag("Mate").GetComponent<MateController>();

        stunStar = transform.GetChild(0);
        stunStar.gameObject.SetActive(false);

    }


    void Start () 
    {
        treesTransform = new Transform[tree.transform.childCount];
        for (int i = 0; i < treesTransform.Length; i++)
            treesTransform[i] = tree.transform.GetChild(i);

        wentTree = new bool[tree.transform.childCount];
        for (int i = 0; i < wentTree.Length; i++)
            wentTree[i] = false;

        loverNo = mc.lovers;
        mc.lovers += 1;

        mc.affection.Add(loverNo, 0);

        passedRoarTime = 0.9f;
        aSource = GetComponent<AudioSource>();
        targets = new List<Collider2D>();
        stun = false;
        haveTarget = false;
        fight = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!stun && !fight)
        {
            findTarget();

            if (destinator == Destination.None)
                findTree();

            if(!haveTarget)
            {
                if (destinator == Destination.Tree)
                {
                    targetPos = (Vector2)treesTransform[chosenTree].transform.position +
                       Random.insideUnitCircle * 1.9f;
                    haveTarget = true;
                }
                else if (destinator == Destination.Mate)
                {
                    targetPos = mc.transform.position;
                    haveTarget = true;
                }
                    
            }

            if (destinator == Destination.Rival)
                print("hell yeah");

            if(findAniAndMove(targetPos, 0.05f))
            {
                if(destinator == Destination.Tree)
                {
                    wentTree[chosenTree] = true;
                }
                destinator = Destination.None;
                haveTarget = false;
            }
        }
        else if(stun)
        {
            if (passedStunTime >= stunTime)
            {
                stunStar.gameObject.SetActive(false);
                stun = false;
                passedStunTime = 0;
            }
            else
            {
                if (!stunStar.gameObject.activeSelf)
                    stunStar.gameObject.SetActive(true);

                stunStar.localPosition = starPos[animator.GetInteger("Direction")];
                passedStunTime += Time.deltaTime;
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

        bool lol = Vector2.Equals(newPos, (Vector2)transform.position);

        transform.position = newPos;

        return lol;
    }

    void findTarget()
    {
        Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, 1.5f);
        int position = animator.GetInteger("Direction");
        for(int i = 0; i < found.Length; i++)
        {
            Vector2 dirToTarget = (found[i].transform.position - transform.position).normalized;

            if (Vector2.Angle(directions[position], dirToTarget) < 90)
                targets.Add(found[i]);
        }


        bool isMate = false;
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].CompareTag("Mate"))
            {
                destinator = Destination.Mate;
                isMate = true;

                if (passedRoarTime > 0.8f)
                {
                    mc.affection[loverNo]++;
                    mc.hitConfirmed = true;
                    mc.hitPos = transform.position;
                    passedRoarTime = 0;

                    aSource.PlayOneShot(aSource.clip);
                }
                else
                    passedRoarTime += Time.deltaTime;

            }

            if (((targets[i].CompareTag("Enemy") && !targets[i].GetComponent<EnemyController>().fight)
                || (targets[i].CompareTag("Player") && !targets[i].GetComponent<PlayerController>().fight)) && 
                ((destinator == Destination.Mate && Vector2.Distance(transform.position, mc.transform.position) < 
                Vector2.Distance(transform.position, targets[i].transform.position)) || destinator < Destination.Mate))
            {
                print("fuck yeah");
                destinator = Destination.Rival;
                targetPos = targets[i].transform.position;
                haveTarget = true;
            }

        }
        

        if (!isMate)
            passedRoarTime = 1;
            

        targets.Clear();
    }

    void findTree()
    {
        int noTree;
        for (noTree = 0; noTree < treesTransform.Length && wentTree[noTree]; noTree++) ;

        if (noTree == treesTransform.Length)
        {
            for (int i = 0; i < treesTransform.Length; i++)
            {
                if (i != chosenTree)
                    wentTree[i] = false;
            }
        }

        destinator = Destination.Tree;

        float distance = float.MaxValue;

        for (int i = 0; i < treesTransform.Length; i++)
        {
            if (distance > Vector2.Distance(transform.position, treesTransform[i].position) && !wentTree[i])
            {
                distance = Vector2.Distance(transform.position, treesTransform[i].position);
                chosenTree = i;
            }
        }
    }

    enum Destination
    {
        None,
        Tree,
        Mate, 
        Rival
    };

}
