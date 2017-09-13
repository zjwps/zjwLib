using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zjw.Tools.IEnumeratorStep
{

    public class Step
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
                mIsCompleted = OnStartAction() || mIsCompleted;
            }
            CheckComplete();
            return mIsCompleted;
        }
        public void SetUpdate(Func<bool> onUpdateAction)
        {
            mIsNeedUpdate = true;
            OnUpdateAction = onUpdateAction;
        }
        public void SetOnStart(Func<bool> onStartAction)
        {
            OnStartAction = onStartAction;
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
            OnCompletedAction = null;
        }

    }
    public static class IEnumeratorStepEx
    {

    }
    public sealed class StepFactory
    {
        private static StepFactory mInstance = new StepFactory();
        //缓存step
        private readonly List<Step> mSteps = new List<Step>();
        private StepFactory()
        {

        }
        public static void RecoveryStep(Step step){
            Debug.LogError("todo");
        }
        public static Step NewWaitFrame(int frame){
            var step = NewWaitStep();
            step.SetUpdate(()=>{
                
                frame--;
                return frame<=0;
            });
            return step;
        }
        public static Step NewWaitTime(float time, bool realTime = true)
        {
            var step = NewWaitStep();
            var mStartTime = 0f;
            Func<float> GetTime = () =>
            {
                if (realTime) return Time.realtimeSinceStartup;
                return Time.time;
            };
            step.SetOnStart(() => { mStartTime = GetTime(); return false; });
            step.SetUpdate(() =>
            {
                return (GetTime() - mStartTime) >= time;
            });
            return step;
        }
        public static Step NewWaitStep()
        {
            if (mInstance.mSteps.Count > 0)
            {
                var item = mInstance.mSteps[0];
                mInstance.mSteps.RemoveAt(0);
                return item;
            }
            return new Step();
        }
    }
    public class StepBehaviour
    {
        
        private List<IEnumerator<Step>> mNeedUpdateSteps;
        int i;
        IEnumerator<Step> mIEnumerator;
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
        public IEnumerator<Step> StartIEnumerator(IEnumerator<Step> iEnumerator)
        {
            if (iEnumerator == null) return iEnumerator;
            if (!iEnumerator.MoveNext()) return iEnumerator;
            StartOneStep(iEnumerator);
            return iEnumerator;
        }
        public void StopIEnumerator(IEnumerator<Step> iEnumerator)
        {
            if (iEnumerator == null) return;
            if (iEnumerator.Current.IsNeedUpdate)
            {
                mNeedUpdateSteps.Remove(iEnumerator);
            }
            iEnumerator.Dispose();
            //if(iEnumerator)
        }
        private void StartOneStep(IEnumerator<Step> iEnumerator)
        {
            if (iEnumerator.Current.Start())
            {
                TryStepNext(iEnumerator);
                return;
            }
            if (iEnumerator.Current.IsNeedUpdate)
            {
                if (mNeedUpdateSteps == null) mNeedUpdateSteps = new List<IEnumerator<Step>>();
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
        private void TryStepNext(IEnumerator<Step> iEnumerator)
        {
            if (iEnumerator.MoveNext())
            {
                StartOneStep(iEnumerator);
            }
        }
    }
}
