using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {

    GameObject comm;

    public Text text;
    
	// Use this for initialization
	void Start () 
    {
        comm = GameObject.FindGameObjectWithTag("Comm");
        print(comm.GetComponent<Comm>().status);
        text.text = comm.GetComponent<Comm>().status;
    }

    public void restart()
    {
        Destroy(comm);
        Application.LoadLevel("game");
        
    }
}
