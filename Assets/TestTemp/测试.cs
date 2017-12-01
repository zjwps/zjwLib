using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using 库;

public class 测试 : TestTemplate {

	// Use this for initialization
	IEnumerator Start () {
        yield return null;
        print(Time.time);
        yield return new 异步任务(new 测试异步任务());
        print("done!");
        //StartCoroutine()
        yield return StartCoroutine(Test1());
	}
	IEnumerator Temp1;
	void Test2(){
		Temp1= 异步();
	}
	IEnumerator 异步(){
		Debug.Log("异步1");
		yield return 11;
        Debug.Log("异步2");
        yield return 22;
        Debug.Log("异步3");
    }
    void Test3(){
		if(Temp1==null) return;
		Debug.Log("Current1: "+Temp1.Current);
        if (!Temp1.MoveNext())
        {
            Temp1 = null;
            Debug.Log("end?");
        }
        else
        {
            Debug.Log("Temp1.MoveNext() true Current2: " + Temp1.Current);
        }
    }
    
	IEnumerator Test1()
    {
        yield return 0;
    }
    public bool TT1;
    private int i;
    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (TT1)
        {
            TT1 = false;
            if (i == 0)
            {
                Test2();
                i++;
                return;
            }
            Debug.Log("run");
            Test3();
        }
    }
    public class 测试异步任务 : 任务
    {
        public 测试异步任务()
        {
            开始 = () => false;
        }
    }
}
