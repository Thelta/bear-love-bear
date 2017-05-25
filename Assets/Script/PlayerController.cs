using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {

    Animator animator;
    Rigidbody2D rb;
    MateController mc;
    AudioSource boar;
    Transform stunStar;
    Vector2[] starPos = { new Vector2(0, 0.2f), new Vector2(0.2f, 0), new Vector2(0, -0.1f), new Vector2(-0.2f, 0) };
    Vector3 oldCamera;
    float winPower;
    GameObject rival;
    public Slider slider;


    [HideInInspector]
    public int loverNo;

    public int speed = 5;
    public float stunTime = 1.5f;
    [HideInInspector]
    public bool stun;
    public bool fight;
    public GameObject commObj;

    float passedStunTime = 0;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boar = GetComponent<AudioSource>();
        mc = GameObject.FindGameObjectWithTag("Mate").GetComponent<MateController>();
    }

    void Start () 
    {
        loverNo = mc.lovers;
        mc.lovers += 1;
        slider.gameObject.SetActive(false);
        mc.affection.Add(loverNo, 0);
        stun = false;
        stunStar = transform.GetChild(0);
        stunStar.gameObject.SetActive(false);
        oldCamera = Camera.main.transform.position;
        winPower = 5;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (!stun && !fight)
        {
            float moveHorizontal = Input.acceleration.x;
            float moveVertical = -Input.acceleration.y;

            moveHorizontal = Input.GetAxis("Horizontal");
            moveVertical = Input.GetAxis("Vertical");

            if (moveVertical != 0)
                animator.SetInteger("Direction", (moveVertical > 0) ? 0 : 2);
            else if (moveHorizontal != 0)
                animator.SetInteger("Direction", (moveHorizontal > 0) ? 1 : 3);

            rb.velocity = new Vector2(moveHorizontal, moveVertical).normalized * speed;
        }
        else
            rb.velocity = Vector3.zero;
	}

    void Update()
    {
        if(!stun & !fight)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.6f);
                boar.PlayOneShot(boar.clip);

                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].transform.tag == "Mate")
                    {
                        mc.hitPos = transform.position;

                        if (!mc.affection.ContainsKey(loverNo))
                            mc.affection.Add(loverNo, 1);
                        else
                        {
                            (mc.affection[loverNo])++;
                        }

                        mc.hitConfirmed = true;

                    }
                }
            }
        }
        else if(stun)
        {
            if(passedStunTime >= stunTime)
            {
                stunStar.gameObject.SetActive(false);
                passedStunTime = 0;
                stun = false;
            }
            else
            {
                if (!stunStar.gameObject.activeSelf)
                    stunStar.gameObject.SetActive(true);

                stunStar.localPosition = starPos[animator.GetInteger("Direction")];
                passedStunTime += Time.deltaTime;
            }
                
        }
        if(fight)
        {
            print(winPower);
            if (Mathf.RoundToInt(winPower) > 0 && Mathf.RoundToInt(winPower) < 55)
            {
                if (Input.GetButtonUp("Fire1") && !stun)
                    winPower += 5 * Mathf.Sqrt(Time.timeSinceLevelLoad);
                if(!rival.GetComponent<EnemyController>().stun)
                    winPower = Mathf.SmoothStep(winPower, 0f, Mathf.Sqrt(Mathf.Log10(Time.timeSinceLevelLoad) / 120f));
                slider.value = winPower;
            }
            else
            {
                StopAllCoroutines();
                slider.gameObject.SetActive(false);
                if (Mathf.RoundToInt(winPower) <= 0)
                {
                    GameObject comm = Instantiate(commObj);
                    string asdf = "Try Your Luck Next Time";
                    comm.GetComponent<Comm>().status = asdf;
                    Application.LoadLevel("gameover");
                }
                else
                {
                    Destroy(rival);
                    fight = false;
                    StartCoroutine(zoom(oldCamera, 3f));
                    StartCoroutine(killRival());
                }
            }

        }

      
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy") && !fight)
        {
            StopAllCoroutines();
            slider.gameObject.SetActive(true);
            winPower = 5;
            rival = other.gameObject;
            other.GetComponent<EnemyController>().fight = true;
            other.GetComponent<Animator>().SetInteger("Direction", (animator.GetInteger("Direction") + 2) % 4);
            fight = true;
            StartCoroutine(zoom(Vector3.Lerp(transform.position, rival.transform.position, 0.5f) + Vector3.back * 10, 0.5f));
            /*Camera.main.transform.position = Vector3.Lerp(transform.position, rival.transform.position, 0.5f) + Vector3.back * 10;
            Camera.main.orthographicSize = 0.5f;*/
        }
    }

    IEnumerator killRival()
    {
        if (rival != null)
        {
            print(rival);
            while (!Vector3.Equals(transform.position, rival.transform.position))
            {
                transform.position = Vector3.MoveTowards(transform.position, rival.transform.position, 0.03f);
                yield return null;
            }
        }
        
    }

    IEnumerator zoom(Vector3 newPos, float size)
    {
        print("girdim");
        while (!Vector3.Equals(Camera.main.transform, newPos) && !Mathf.Approximately(Camera.main.orthographicSize, size))
        {
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, newPos, 0.5f);
            Camera.main.orthographicSize = Mathf.SmoothStep(Camera.main.orthographicSize, size, 0.2f);
            yield return null;
        }
        print("çıktım");
    }

    
}
