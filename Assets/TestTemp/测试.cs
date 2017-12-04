using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using 库;

public class 测试 : TestTemplate {

	// Use this for initialization
	IEnumerator Start () {
        yield return null;
        print(Time.time);
        //yield return new 异步任务(new 测试异步任务());
        print("done!");
        //StartCoroutine()
        yield return StartCoroutine(Test1());
	}
	IEnumerator Temp1;
	void Test2(){
		Temp1= 异步();
	}
	IEnumerator 异步(){
		if(Random.Range(1,99)==1){
			Debug.Log("Random.Range(1,4)==1");
			yield return 11;
		}
		// Debug.Log("异步1之前");
        // yield return 1;
		// Debug.Log("异步2之前");
        // yield return 2;
		// Debug.Log("异步2结束");

		Debug.Log("异步没有测试?");
    }
    void Test3(){
		if(Temp1==null) return;
        if (!Temp1.MoveNext())
        {
            Temp1 = null;
            Debug.Log("=====异步结束====");
            return;
        }
        //else
        //{
        //    Debug.Log("Temp1.MoveNext() true Current2: " + (Temp1.Current));
        //}
		Debug.Log("=====异步还有?====Temp1.Current==null " + (Temp1.Current==null));
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
            // Debug.Log("run");
            Test3();
        }
    }
  
}
