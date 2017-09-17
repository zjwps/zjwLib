using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
    public bool FaceLeft = true;
    public  Animator animator;
    public Rigidbody2D Rigidbody2D;
    public SpriteRenderer Star;
    public TextMesh TextMesh;

    private bool mFaceLeft = true;
    public float speed = 10;

    // Use this for initialization
    void Start () {
        var container = new GameObject();
        for (int i = 0; i < 12; i++)
        {
             var item = Instantiate(Star);
            item.transform.parent = container.transform;
            //item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
            item.transform.localScale = Vector3.one;

            item.transform.localPosition = new Vector3(Random.Range(-4,4),3);
            //item.transform.
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (FaceLeft != mFaceLeft)
        {
            mFaceLeft = FaceLeft;
            animator.SetBool("faceLeft",mFaceLeft);
        }
	}
    private void FixedUpdate()
    {
        var h = Input.GetAxis("Horizontal");
        var jump = Input.GetKeyDown(KeyCode.Space);
        if (h!=0)
        FaceLeft = h < 0;
        var y = Rigidbody2D.velocity.y;
        if (jump)
        {
            y= 5;
        }
        Rigidbody2D.velocity = new Vector2(speed * h,y);

    }
    int score = 0;
    void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("捡起"))
        //{
        //    other.gameObject.SetActive(false);
        //    score += 10;
        //    TextMesh.text = "积分:" + score;
        //}
    }
}
