using System;
using System.Collections.Generic;
using UnityEngine;
namespace zjw.Tools.WaitStep
{
    public class StepPool
    {
        private static readonly StepPool mInstance = new StepPool();
        private readonly List<Step> mSteps = new List<Step>();
        private StepPool()
        {
        }
        public static Step NewStep()
        {
            return mInstance.GetNewStep();
            //new Step();
        }
        #region 几个等待step

        public static Step NewWaitTime(float needWaitTime, bool realTime = true)
        {
            var step = NewStep();
            Func<float> GetTime = () =>
            {
                if (realTime) return Time.realtimeSinceStartup;
                return Time.time;
            };
            float startTime = 0;
            step.SetOnStart(() =>
            {
                startTime = GetTime();
                return false;
            });
            step.SetOnUpdate(() =>
            {
                return (GetTime() - startTime) > needWaitTime;
            });
            return step;
        }
        public static Step NewWaitFrame(int frame = 1)
        {
            var step = NewStep();
            step.SetOnUpdate(() =>
            {
                frame--;
                return frame <= 0;
            });
            return step;
        }
        #endregion
        public static void RecoveryStep(Step step)
        {
            step.Clear();
            mInstance.mSteps.Add(step);
        }
        public Step GetNewStep()
        {
            if (mSteps.Count == 0) return new Step();
            var item = mSteps[0];
            mSteps.RemoveAt(0);
            return item;
            //new Step();
        }
    }
    public sealed class Step
    {
        private bool mIsCompleted;
        private bool mIsCompletedBefore;
        public bool IsCompleted { get { return mIsCompleted; } }

        private bool mIsNeedUpdate;
        public bool IsNeedUpdate { get { return mIsNeedUpdate; } }
        private Func<bool> OnStartFunc;
        private Func<bool> OnUpdateFunc;
        private Action OnCompletedAction;
        public bool Start()
        {
            mIsCompletedBefore = mIsCompleted = false;
            if (OnStartFunc != null)
            {
                mIsCompleted = OnStartFunc() || mIsCompleted;
            }
            CheckComplete();
            return mIsCompleted;
        }


        #region 对外接口
        public void SetOnStart(Func<bool> onStartFunc)
        {
            OnStartFunc = onStartFunc;
        }
        public void SetOnUpdate(Func<bool> onUpdateFunc)
        {
            mIsNeedUpdate = true;
            OnUpdateFunc = onUpdateFunc;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="onComplete"></param>
        public void AddOnComplete(Action onComplete)
        {
            OnCompletedAction += onComplete;
        }
        #endregion

        public void Update()
        {
            if (mIsCompleted) return;
            if (OnUpdateFunc != null)
            {
                mIsCompleted = OnUpdateFunc();
            }
        }
        public void Clear()
        {
            mIsCompleted = false;
            OnUpdateFunc = null;
            OnStartFunc = null;
            mIsNeedUpdate = false;
            OnCompletedAction = null;
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
        public void Complete()
        {
            mIsCompleted = true;
            CheckComplete();
        }
        private void OnComplete()
        {
            if (OnCompletedAction != null) OnCompletedAction();
            Clear();
        }
    }
    public class StepProcess
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
        public IEnumerator<Step> StartStep(IEnumerator<Step> iEnumerator)
        {
            if (iEnumerator == null) return iEnumerator;
            if (!iEnumerator.MoveNext()) return iEnumerator;
            StartOneStep(iEnumerator);
            return iEnumerator;
        }
        public void StopStep(IEnumerator<Step> iEnumerator)
        {
            if (iEnumerator == null) return;
            if (iEnumerator.Current == null) return;
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
                iEnumerator.Current.AddOnComplete(() =>
                {
                    TryStepNext(iEnumerator);
                });
            }

        }
        private void TryStepNext(IEnumerator<Step> iEnumerator)
        {
            var step = iEnumerator.Current;

            if (iEnumerator.MoveNext())
            {
                StartOneStep(iEnumerator);
            }
            else
            {
                //没有下一步了完了.
            }
            if (step != null)
            {
                StepPool.RecoveryStep(step);
            }
        }
    }
}