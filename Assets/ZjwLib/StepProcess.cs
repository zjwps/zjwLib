using System;
using System.Collections.Generic;
using UnityEngine;

namespace zjw.Tools.WaitStep
{
    public class TestStepProcess : StepProcess
    {
        private bool wait;

        public void Test1()
        {
            StartStep(Fn1());
            //StartStep(TestWait());
            StartStep(Steps());
            //StartStep(WaitTest1());

        }
        private IEnumerator<Step> WaitTest1()
        {
            var needWait = true;
            yield return this.NewWaitStep(() =>
            {
                Debug.Log("needWait---");
                return !needWait;
            });
            Debug.Log("WaitTest1 succ:" + Time.time);

        }
        private IEnumerator<Step> Steps()
        {
            wait = true;
            yield return this.NewWaitFrame(60);
            //Debug.Log("NewWaitTime end:" + Time.time);
            Debug.Log("NewWaitFrame end:" + Time.time);
            yield return this.NewStartStep(TestWait());
        }
        private IEnumerator<Step> TestWait()
        {
            Debug.Log("TestWait start:" + Time.time);
            yield return this.NewWaitStep(() =>
            {
                Debug.Log("wait---");
                return !wait;
            });
            Debug.Log("TestWait end:" + Time.time);

        }
        private IEnumerator<Step> Fn1()
        {
            yield return this.NewWaitFrame(1);
            Debug.Log("Fn1 1:" + Time.time);
            yield return this.NewStartStep(Fn2());
            Debug.Log("Fn1 2:" + Time.time);
            yield return this.NewWaitFrame(1);
            Debug.Log("Fn1End:" + Time.time);

            wait = false;
            Debug.Log(" wait=false");

        }
        private IEnumerator<Step> Fn2()
        {
            yield return this.NewWaitTime(.2f);
            Debug.Log("Fn2: end" + Time.time);
        }
    }
    public static class StepProcessEx
    {
        public static Step NewStep(this StepProcess stepProcess)
        {
            return StepPool.NewStep();
        }
        public static Step NewWaitTime(this StepProcess stepProcess, float needWaitTime, bool realTime = true)
        {
            return StepPool.NewWaitTime(needWaitTime, realTime);
        }
        public static Step NewWaitFrame(this StepProcess stepProcess, int frame)
        {
            return StepPool.NewWaitFrame(frame);
        }
        public static Step NewWaitStep(this StepProcess stepProcess, Func<bool> waitFn)
        {
            return StepPool.NewWaitStep(waitFn);

        }
        public static Step NewStartStep(this StepProcess stepProcess, IEnumerator<Step> iEnumerator)
        {
            return StepPool.NewStartStep(iEnumerator);
        }
    }
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
        /// <summary>
        /// /// <summary>
        /// TODO 为嵌套NewStartStep的缓存下 StepProcess
        /// </summary>
        /// </summary>
        /// <param name="iEnumerator"></param>
        /// <returns></returns>
        public static Step NewStartStep(IEnumerator<Step> iEnumerator)
        {
            var step = NewStep();
            StepProcess stepProcess = null;
            step.SetOnStart(() =>
            {
                stepProcess = new StepProcess();
                stepProcess.StartStep(iEnumerator);
                return false;
            });
            step.SetOnUpdate(() =>
            {
                if (stepProcess == null) return true;
                if (stepProcess.Update())
                {
                    stepProcess = null;
                    return true;
                }
                return false;
            });
            return step;
        }

        public static Step NewWaitStep(Func<bool> waitFn)
        {
            var step = NewStep();
            var end = false;
            if (waitFn == null) end = true;
            step.SetOnUpdate(() =>
            {
                if (end) return true;
                end = waitFn();
                return end;
            });
            return step;
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
        /// <summary>
        /// 返回是否结束
        /// </summary>
        /// <returns></returns>
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
    /// <summary>
    /// 一组 就是一次StartStep的所有步骤
    /// </summary>
    public class StepGroup
    {
        private Step curr;

        public bool End { get; private set; }
        public bool IsNeedUpdate { get; private set; }
        //public Step Curr;
        public IEnumerator<Step> IEnumerator { get; private set; }

        public StepGroup(IEnumerator<Step> iEnumerator)
        {
            IEnumerator = iEnumerator;
            End = false;
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>是否结束</returns>
        public bool Run()
        {
            if (End) return true;

            curr = IEnumerator.Current;
            var stepEnd = curr.Start();
            if (stepEnd)
            {
                TryStepNext();
                if (!End)
                    return Run();
            }

            if (curr.IsNeedUpdate)
            {
                IsNeedUpdate = true;
            }
            return false;
        }
        private void TryStepNext()
        {
            if (curr != null) StepPool.RecoveryStep(curr);
            curr = null;
            if (!IEnumerator.MoveNext())
            {
                //如果没有下一步了,结束
                End = true;
                return;
            }
            Run();
        }
        public void Update()
        {
            if (curr.IsCompleted)
            {
                IsNeedUpdate = false;
                TryStepNext();
                return;
            }
            curr.Update();
        }

    }
    public class StepProcess
    {
        private DictList<IEnumerator<Step>, StepGroup> mStepGroups;
        int i;
        StepGroup mStepGroup;
        public bool Update()
        {
            for (i = 0; i < mStepGroups.Count; i++)
            {
                mStepGroup = mStepGroups.GetItemAt(i);
                if (mStepGroup.End)
                {
                    mStepGroups.RemoveAt(i);
                    i--;
                    continue;
                }
                if (mStepGroup.IsNeedUpdate)
                {
                    mStepGroup.Update();

                    if (mStepGroup.End)
                    {
                        mStepGroups.RemoveAt(i);
                        i--;
                        continue;
                    }
                }
            }
            return mStepGroups == null || mStepGroups.Count == 0;
        }
        public IEnumerator<Step> StartStep(IEnumerator<Step> iEnumerator)
        {
            if (iEnumerator == null) return iEnumerator;
            if (!iEnumerator.MoveNext())
            {
                return iEnumerator;
            }
            if (mStepGroups == null) mStepGroups = new DictList<IEnumerator<Step>, StepGroup>();
            var newGroup = new StepGroup(iEnumerator);
            if (!newGroup.Run())
            {
                mStepGroups.Add(iEnumerator, newGroup);
            }
            return iEnumerator;
        }
        public void StopStep(IEnumerator<Step> iEnumerator)
        {
            if (iEnumerator == null) return;
            if (mStepGroups == null) return;
            if (!mStepGroups.ContainsKey(iEnumerator)) return;
            mStepGroups.Remove(iEnumerator);
            iEnumerator.Dispose();
            //if(iEnumerator)
        }



    }
    public class StepProcessOld
    {
        private List<IEnumerator<Step>> mNeedUpdateSteps;
        private bool mWaitNext = false;
        int i;
        IEnumerator<Step> mIEnumerator;
        public bool IsCompleted = false;
        public bool Update()
        {
            if (mWaitNext) return true;
            if (mNeedUpdateSteps == null) return true;
            for (i = 0; i < mNeedUpdateSteps.Count; i++)
            {
                mIEnumerator = mNeedUpdateSteps[i];
                if (mIEnumerator.Current.IsCompleted)
                {
                    var end = TryStepNext(mIEnumerator);

                    if (end)
                    {
                        mNeedUpdateSteps.Remove(mIEnumerator);
                        i--;
                        if (mNeedUpdateSteps.Count == 0)
                        {
                            mNeedUpdateSteps = null;
                            IsCompleted = true;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }

                }
                mIEnumerator.Current.Update();
            }
            return IsCompleted;
        }
        public IEnumerator<Step> StartStep(IEnumerator<Step> iEnumerator)
        {
            if (iEnumerator == null) return iEnumerator;
            // if(iEnumerator.Current != null){
            //     Debug.LogError("wrong");
            //     return null;
            // }
            if (!iEnumerator.MoveNext())
            {
                return iEnumerator;
            }
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
                //如果结束下一步
                TryStepNext(iEnumerator);
                return;
            }
            //如果还没有结束
            //如果需要update驱动
            if (iEnumerator.Current.IsNeedUpdate)
            {
                if (mNeedUpdateSteps == null) mNeedUpdateSteps = new List<IEnumerator<Step>>();
                mNeedUpdateSteps.Add(iEnumerator);
            }
            else
            {
                mWaitNext = true;
                iEnumerator.Current.AddOnComplete(() =>
                {
                    mWaitNext = false;
                    TryStepNext(iEnumerator);
                });
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iEnumerator"></param>
        /// <returns> end</returns>
        private bool TryStepNext(IEnumerator<Step> iEnumerator)
        {
            var step = iEnumerator.Current;
            if (step != null)
            {
                StepPool.RecoveryStep(step);
                step = null;
            }
            if (iEnumerator.MoveNext())
            {
                StartOneStep(iEnumerator);
                return false;
            }
            else
            {
                mWaitNext = false;
                return true;
                //没有下一步了完了.
            }

        }
    }
}