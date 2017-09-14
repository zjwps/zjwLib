using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using zjw.Tools.WaitStep;

public class Test1 : MonoBehaviour {
    private UpdateDriver mUpdateDriver;

    // Use this for initialization
    void Start () {
		 mUpdateDriver = new UpdateDriver();
		 var stepTask = new StepProcess();
		 mUpdateDriver.AddUpdate(stepTask.Update);
		 
		 stepTask.StartStep(Fn1());
		 
	}

    private IEnumerator<Step> Fn1()
    {
		Debug.Log("startTime:"+Time.time);
		yield return StepPool.NewWaitTime(0.5f);
		Debug.Log("Time1:"+Time.time);
		yield return StepPool.NewWaitFrame(2);
		Debug.Log("Time1:"+Time.time);
    }

    // Update is called once per frame
    void Update () {
		mUpdateDriver.Update();
	}
}
