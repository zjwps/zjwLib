using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zjw.Tools.IEnumeratorStep;

public class GameStart : MonoBehaviour
{
    private GameMain mGameMain;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Use this for initialization
    void Start()
    {
        mGameMain = new GameMain();
    }

    // Update is called once per frame
    void Update()
    {
        mGameMain.Update();
    }
}
public class GameMain
{
    public  Func<bool> Update;
    private GameTools GameTools;

    public GameMain()
    {
		GameTools =  new GameTools(this);
    }

}
public class GameTools
{
    private readonly GameMain main;
    private UpdateDriver mUpdateDriver;
    private StepBehaviour mStepBehaviour;

    public GameTools(GameMain main)
    {
        this.main = main;
        Init();

    }

    private void Init()
    {
        mUpdateDriver = new UpdateDriver();
		main.Update = mUpdateDriver.Update;
		mStepBehaviour = new StepBehaviour();
    }

    public void AddUpdate(Func<bool> updateAction)
    {

    }
	public IEnumerator<Step> StartStep(IEnumerator<Step> iEnumerator){
		return mStepBehaviour.StartIEnumerator(iEnumerator);
	}
	public void StopStep(){

	}
}
