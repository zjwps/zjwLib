using System;
using System.Collections.Generic;
using UnityEngine;
public class iEnumeratorTime : IEnumeratorStep
{
    private float mWaitTime;
    private float mStartTime;
    private readonly bool realTime;
    public iEnumeratorTime(float time, bool realTime = true)
    {
        this.realTime = realTime;
        this.mWaitTime = time;
        mIsNeedUpdate = true;

    }
    protected override bool OnStart()
    {
         Debug.Log("iEnumeratorTime OnStart =====" + GetTime());
        mStartTime = GetTime();
        return mWaitTime <= 0;
    }
    private float GetTime()
    {
        if (realTime)return Time.realtimeSinceStartup;
        return Time.time;
    }
    protected override bool OnUpdate()
    {
        // Debug.Log("onUpdate time"+GetTime());
        // Debug.Log("mStartTime "+mStartTime);
       return  (GetTime()-mStartTime) >= mWaitTime;
    }
}
public class iEnumeratorFrame : IEnumeratorStep
{
    private int frame;
    public iEnumeratorFrame(int frame)
    {
        this.frame = frame;
        mIsNeedUpdate = true;

    }
    protected override bool OnStart()
    {
        if (frame <= 0) return true;
        return false;
    }
    protected override bool OnUpdate()
    {
        frame--;
        return frame <= 0;
    }
}
public class IEnumeratorStep
{
    private bool mIsCompleted;
    private bool mIsCompletedBefore;
    public bool IsCompleted { get { return mIsCompleted; } }

    protected bool mIsNeedUpdate;
    public bool IsNeedUpdate { get { return mIsNeedUpdate; } }
    public Func<bool> OnStartAction;
    public Func<bool> OnUpdateAction;
    public Action OnCompletedAction;
    public bool Start()
    {
        //Debug.Log("start111");
        mIsCompletedBefore = mIsCompleted = false;
        if (OnStartAction != null)
        {
            mIsCompleted = OnStartAction();
        }
        mIsCompleted = OnStart();
        CheckComplete();
        return mIsCompleted;
    }
    public void Update()
    {
        if (mIsCompleted) return;
        if (OnUpdateAction != null)
        {
            mIsCompleted = OnUpdateAction();
        }
        if (mIsCompleted) return;
        mIsCompleted = OnUpdate();
    }
    private void CheckComplete()
    {
        if (mIsCompleted)
        {
            if (mIsCompletedBefore == false)
            {
                mIsCompletedBefore = true;
                OnComplete();
            }
        }
    }
    protected void Complete()
    {
        mIsCompleted = true;
        CheckComplete();
    }
    protected void OnComplete()
    {
        //Debug.Log("OnComplete1 OnComplete OnComplete");
        if (OnCompletedAction != null) OnCompletedAction();
        OnCompletedAction=null;
    }

    protected virtual bool OnUpdate()
    {
        return false;
    }
    protected virtual bool OnStart()
    {
        return false;
    }

}
public class IEnumeratorProcess : IUpdate
{
    private List<IEnumerator<IEnumeratorStep>> mNeedUpdateSteps;

    int i;
    IEnumerator<IEnumeratorStep> mIEnumerator;
    public bool IsCompleted = false;
    public bool Update()
    {
        if (mNeedUpdateSteps == null) return IsCompleted;
        for (i = 0; i < mNeedUpdateSteps.Count; i++)
        {
            mIEnumerator = mNeedUpdateSteps[i];
            if (mIEnumerator.Current.IsCompleted)
            {
                mNeedUpdateSteps.Remove(mIEnumerator);
                i--;
                TryStepNext(mIEnumerator);
                continue;
            }
            mIEnumerator.Current.Update();
        }
        return IsCompleted;
    }
    public void StartIEnumerator(IEnumerator<IEnumeratorStep> iEnumerator)
    {
        if(iEnumerator==null)return;
        if(!iEnumerator.MoveNext())return;
        StartOneStep(iEnumerator);
    }
    public void StopIEnumerator(IEnumerator<IEnumeratorStep> iEnumerator)
    {
        if (iEnumerator == null) return;
        if(iEnumerator.Current.IsNeedUpdate){
            mNeedUpdateSteps.Remove(iEnumerator);
        }
        iEnumerator.Dispose();
        //if(iEnumerator)
    }
    private void StartOneStep(IEnumerator<IEnumeratorStep> iEnumerator)
    {
        if (iEnumerator.Current.Start())
        {
            TryStepNext(iEnumerator);
            return;
        }
        if (iEnumerator.Current.IsNeedUpdate)
        {
            if (mNeedUpdateSteps == null) mNeedUpdateSteps = new List<IEnumerator<IEnumeratorStep>>();
            mNeedUpdateSteps.Add(iEnumerator);
        }
        else
        {
            iEnumerator.Current.OnCompletedAction = () =>
            {
                TryStepNext(iEnumerator);
            };
        }
        
    }
    private void TryStepNext(IEnumerator<IEnumeratorStep> iEnumerator)
    {
        if (iEnumerator.MoveNext())
        {
            StartOneStep(iEnumerator);
        }
    }
}