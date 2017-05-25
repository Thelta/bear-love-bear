using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyReviver : MonoBehaviour {


    public GameObject enemyBear;

    Transform[] reviveBar;

    List<GameObject> bears;

    bool notDead;

    float fromReset;


	// Use this for initialization
	void Start () 
    {
        reviveBar = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            reviveBar[i] = transform.GetChild(i);

        bears = new List<GameObject>();
        fromReset = 0;
	}
	
	// Update is called once per frame
	void Update () 
    {
        notDead = false;
	    if(bears.Count > 0)
        {
            for(int i = 0; i < bears.Count; i++)
            {
                if (bears[i] != null)
                    notDead = true;
            }
            fromReset += Time.deltaTime;
        }

        if(bears.Count == 0 || !notDead ||(notDead && fromReset >= 1 && (int) fromReset % 10 == 9))
        {
            for (int i = 0; i < Time.time / 20 + 1; i++)
                bears.Add(Instantiate(enemyBear, reviveBar[Random.Range(0, transform.childCount)].position, Quaternion.identity) as GameObject);
        }
        fromReset = 0;
	}
}
