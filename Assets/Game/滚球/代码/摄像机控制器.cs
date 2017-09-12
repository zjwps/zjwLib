using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 摄像机控制器 : MonoBehaviour {
	public GameObject 跟随的目标;
	public Vector3 偏移;
	// Use this for initialization
	void Start () {
		//偏移 = transform.position - 跟随的目标.transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position =  跟随的目标.transform.position + 偏移;
	}
}
