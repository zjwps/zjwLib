using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 猪脚控制器 : MonoBehaviour
{
	public float 速度=10;
    private Rigidbody m刚体;

    void Start()
	{
		m刚体 =  GetComponent<Rigidbody>();
	}
    void FixedUpdate()
    {
        var 横向 = Input.GetAxis("Horizontal");

        var 纵向 = Input.GetAxis("Vertical");

		var 移动 = new Vector3(横向,0,纵向);
		m刚体.AddForce(移动 * 速度);
    }
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("捡起")){
			other.gameObject.SetActive(false);
		}
	}
}
