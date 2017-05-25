using UnityEngine;
using System.Collections;


[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {

    Animator animator;
    Rigidbody2D rb;

    public int speed = 5;

    void Start () 
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
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


        rb.velocity = new Vector2(moveHorizontal, moveVertical) * speed;
	}
}
