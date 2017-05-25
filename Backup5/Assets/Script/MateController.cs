using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(Animator))]
public class MateController : MonoBehaviour {


    public GameObject tree;

    [HideInInspector]
    public Dictionary<int, int> affection;
    [HideInInspector]
    public bool hitConfirmed;
    [HideInInspector]
    public Vector2 hitPlace;
    [HideInInspector]
    public int hitMan;
    [HideInInspector]
    public int lovers;

    Transform[] treesTransform;
    Animator animator;

    bool inTree;
    bool changeTree = true;
    int chosenTree;

	void Start () 
    {
        treesTransform = new Transform[tree.transform.childCount];

        for(int i = 0; i < treesTransform.Length; i++)
            treesTransform[i] = tree.transform.GetChild(i);

        chosenTree = Random.Range(0, treesTransform.Length);

        animator = GetComponent<Animator>();

        affection = new Dictionary<int, int>();
        hitConfirmed = false;
        lovers = 0;
        inTree = false;
        changeTree = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if(!inTree)
        {
            Vector2 newPos = Vector2.Lerp((Vector2)transform.position, (Vector2)treesTransform[chosenTree].position, 
                0.1f / Vector2.Distance(transform.position, treesTransform[chosenTree].position));

            if (Vector2.Equals(newPos, (Vector2)transform.position))
            {
                inTree = true;
                print(newPos + " " + transform.position);

            }

            findAniAndMove(treesTransform[chosenTree].position, 
                0.1f / Vector2.Distance(transform.position, treesTransform[chosenTree].position));
        }

        print("inTree: " + inTree + " changeTree: " + changeTree);
            


        
	}

    void Update ()
    {
        if (hitConfirmed)
        {
            findAniAndMove(hitPlace, affection[hitMan] / 200f);

            print("changed");
            print(affection[0]);

            hitConfirmed = false;
        }
    }

    void findAniAndMove(Vector2 dest, float t)
    {
        Vector2 newPos = Vector2.Lerp((Vector2)transform.position, dest, t);

        if (transform.position.y - newPos.y != 0)
            animator.SetInteger("Direction", (transform.position.y - newPos.y < 0) ? 0 : 2);
        else if (transform.position.x - newPos.x != 0)
            animator.SetInteger("Direction", (transform.position.x - newPos.x < 0) ? 1 : 3);
        else
            animator.SetInteger("Direction", -1);

        transform.position = newPos;
    }
}
