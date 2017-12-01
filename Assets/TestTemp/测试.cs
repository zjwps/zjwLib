using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using 库;

public class 测试 : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
        yield return null;
        print(Time.time);
        yield return new 异步任务(new 测试异步任务());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public class 测试异步任务 : 任务
    {
        public 测试异步任务()
        {
            开始 = () => true;
        }
    }
}
