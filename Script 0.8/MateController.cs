using UnityEngine;
using System.Collections.Generic;


[RequireComponent(typeof(Animator))]
public class MateController : MonoBehaviour {


    public GameObject tree;
    public GameObject commObj;

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
    public LayerMask loverMask;
    public int affectionToLove;

    Transform[] treesTransform;
    Animator animator;

    bool inTree;
    bool changeTree = true;
    bool bearLoveBear;
    int chosenTree;
    Vector2 boxCol;

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
        boxCol = new Vector2(1.05f, 0.9f);
        bearLoveBear = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if(!inTree)
        {
            Vector2 newPos = Vector2.Lerp((Vector2)transform.position, (Vector2)treesTransform[chosenTree].position, 
                0.1f);

            if (Vector2.Equals(newPos, (Vector2)transform.position))
                inTree = true;

            findAniAndMove(treesTransform[chosenTree].position, 
                0.1f );
        }

        //print("inTree: " + inTree + " changeTree: " + changeTree);            
	}

    void Update ()
    {
        RaycastHit2D[] hit = Physics2D.BoxCastAll(transform.position, boxCol, 0, Vector2.zero);
        if((inTree && lovers > 0))
        {
            for (int i = 0; i < hit.Length && !bearLoveBear; i++)
            {
                if (hit[i].collider.CompareTag("Player") && !bearLoveBear)
                {
                    PlayerController pc = hit[i].collider.GetComponent<PlayerController>();
                    if (affection[pc.loverNo] < affectionToLove)
                    {
                        List<int> lol = new List<int>();
                        lol.Add(chosenTree);
                        for (int j = 1; j < 3; j++)
                        {
                            if (j + chosenTree < tree.transform.childCount)
                                lol.Add(j + chosenTree);
                            if (chosenTree - j >= 0)
                                lol.Add(chosenTree - j);
                        }

                        int random = Random.Range(0, tree.transform.childCount);
                        while (lol.Contains(random))
                            random = Random.Range(0, tree.transform.childCount);
                        chosenTree = random;
                        inTree = false;
                    }
                    else
                        bearLoveBear = true;
                }
                if(hit[i].collider.CompareTag("Enemy") && !bearLoveBear)
                {
                    EnemyController pc = hit[i].collider.GetComponent<EnemyController>();
                    if (affection[pc.loverNo] < affectionToLove)
                    {
                        List<int> lol = new List<int>();
                        lol.Add(chosenTree);
                        for (int j = 1; j < 3; j++)
                        {
                            if (j + chosenTree < tree.transform.childCount)
                                lol.Add(j + chosenTree);
                            if (chosenTree - j >= 0)
                                lol.Add(chosenTree - j);
                        }

                        int random = Random.Range(0, tree.transform.childCount);
                        while (lol.Contains(random))
                            random = Random.Range(0, tree.transform.childCount);
                        chosenTree = random;
                        inTree = false;
                    }
                    else
                        bearLoveBear = true;
                }
            }
        }


            if (hitConfirmed)
            {
                findAniAndMove(hitPlace, affection[hitMan] / 500f);

                hitConfirmed = false;
            }

    }

    void findAniAndMove(Vector2 dest, float t)
    {
        Vector2 newPos = Vector2.MoveTowards((Vector2)transform.position, dest, t);

        if (transform.position.y - newPos.y != 0)
            animator.SetInteger("Direction", (transform.position.y - newPos.y < 0) ? 0 : 2);
        else if (transform.position.x - newPos.x != 0)
            animator.SetInteger("Direction", (transform.position.x - newPos.x < 0) ? 1 : 3);
        else
            animator.SetInteger("Direction", -1);

        transform.position = newPos;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        int ln = -1;

        if (other.CompareTag("Enemy"))
            ln = other.GetComponent<EnemyController>().loverNo;
        else if (other.CompareTag("Player"))
            ln = other.GetComponent<PlayerController>().loverNo;

        
        if(ln > -1)
        {
            print(ln);
            if (other.CompareTag("Enemy") && affection[ln] > affectionToLove)
            {
                GameObject comm = Instantiate(commObj);
                string asdf = "Try Your Luck Next Time";
                comm.GetComponent<Comm>().status = asdf;
                Application.LoadLevel("gameover");
            }

            if (other.CompareTag("Player") && affection[ln] > affectionToLove)
            {
                GameObject comm = Instantiate(commObj);
                string asdf = "Congrulation on Your Happy Ending";
                comm.GetComponent<Comm>().status = asdf;
                Application.LoadLevel("gameover");
            }

            if (other.CompareTag("Player") && affection[ln] < affectionToLove)
            {
                other.GetComponent<PlayerController>().stun = true;
            }

            if (other.CompareTag("Enemy") && affection[ln] < affectionToLove)
            {
                other.GetComponent<EnemyController>().stun = true;
            }
        }


        

    }

}
