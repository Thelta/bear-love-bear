using UnityEngine;

public class Comm : MonoBehaviour {


    public string status = "asd";
	// Use this for initialization
	void Start () 
    {
        UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}
}
