
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Step
namespace ZjwTools
{
    /// <summary>
    /// 
    /// </summary>
    public class StepData
    {
        public uint uint32;
        public int int32;
        public string str;
        public object data;
    }
    /// <summary>
    /// 模仿promise 写一个方法驱动的流程控制。
    /// </summary>
    public class Step
    {
        //public Step StartStep;
        public Step ThenStep;
        public Step ElseStep;
        //给下一个传递的数据
        public StepData ToNextData;
        //来自上一个step的数据
        public StepData ParentData;
        public Step FromStep;
        // 1 未开始 2.执行中.3有结果了 4.下一步开始
        public int State = 1;
        //0还没有结果 1成功 2失败 
        public int result = 0;
        ///// <summary>
        ///// 重置，需要复用的
        ///// </summary>
        //public void Reset()
        //{
        //    OnReset();
        //    result = 0;
        //    State = 1;
        //    //ToNextData = null;
        //    //ParentData = null;
        //    //ThenStep = null;
        //    //ElseStep = null;
        //}
        //public virtual void  OnReset()
        //{

        //}
        //几个常用数据
        public Step Then(Step step)
        {
            ThenStep = step;
            ThenStep.FromStep = this;
            OnSetNextStep();
            return step;
        }
        private void OnSetNextStep()
        {
            if (State != 3) return;
            if (result == 1)
            {
                TryNext(ThenStep); return;
            }
            TryNext(ElseStep);

        }

        internal object If(Func<bool> p, out object ifPassStep)
        {
            throw new NotImplementedException();
        }

        public Step Else(Step step)
        {
            ElseStep = step;
            ElseStep.FromStep = this;
            OnSetNextStep();
            return step;
        }

        public Step Start()
        {
            if (State != 1) return this;
            State = 2;
            //this.Log("类型", this.GetType().Name,"开始");
            OnStart();
            return this;
            //State = 3;

        }
        public void Start(int value)
        {
            if (ToNextData == null)
            {
                ToNextData = new StepData();
            }

            ToNextData.int32 = value;
            Start();
        }
        private void TryNext(Step nexStep)
        {
            if (!OnRunInfo())
            {
                this.Log("类型", this.GetType().Name, "执行完毕");
            }
            OnEnd();
            if (nexStep != null)
            {
                if (ToNextData != null)
                {
                    nexStep.ParentData = ToNextData;
                }
                nexStep.Start();
                //下一步开始了
                State = 4;
            }

        }
        protected virtual void OnEnd()
        {

        }
        /// <summary>
        /// 开始的时候输出信息
        /// </summary>
        /// <returns>有信息？</returns>
        public virtual bool OnRunInfo()
        {
            return false;
        }
        public virtual void OnStart()
        {

        }
        public void Succ()
        {
            if (State != 2)
            {
                //wrong 
                return;
            }
            result = 1;
            State = 3;
            TryNext(ThenStep);
        }
        public bool IsNoStart
        {
            get { return State == 1; }
        }
        public bool IsEnd
        {
            get { return State > 2; }
        }
        public void Stop()
        {
            OnStop();
            OnEnd();
            //如果开始了
            if (State > 1)
            {
                if (ThenStep != null)
                {
                    ThenStep.Stop();
                }
                if (ElseStep != null)
                {
                    ElseStep.Stop();
                }
            }
            ThenStep = null;
            ElseStep = null;
            ParentData = null;
            ToNextData = null;
        }
        public virtual void OnStop()
        {

        }
        public void Fail()
        {
            if (State != 2) return;
            result = 2;
            State = 3;
            TryNext(ElseStep);
        }
        protected void Log(params object[] args)
        {
            string str = "";
            for (int i = 0; i < args.Length; i++)
            {
                str += " " + args[i];
            }
            //Debug.Log(str);
        }
    }

    public class StartSteps
    {
        private List<Step> steps;

        public StartSteps()
        {
            steps = new List<Step>();
        }
        public Step NewStartThen(Step step)
        {
            var t = new StartStep();
            steps.Add(t);
            t.Then(step);
            return step;
        }

        public Step NewStart()
        {
            var t = new StartStep();
            steps.Add(t);
            return t;
        }
        public void Stop()
        {
            for (int i = 0; i < steps.Count; i++)
            {
                steps[i].Stop();
            }
            steps.Clear();
        }
    }
    /// <summary>
    /// 自动成功的ActionStep
    /// </summary>
    public class RunActionStep : Step
    {
        private Action Action;

        public RunActionStep(Action Action)
        {
            this.Action = Action;
        }
        public override void OnStart()
        {
            base.OnStart();
            if (Action != null)
                Action();
            Succ();
        }
        public override bool OnRunInfo()
        {
            this.Log("执行方法:", this.Action.Method.Name);
            return true;
        }
    }

    
    /// <summary>
    /// 根据bool值执行成功或失败
    /// </summary>
    public class ActionBoolStep : Step
    {
        //public delegate bool FuncBool();
        private Func<bool> Action;
        private bool fnResult;
        public ActionBoolStep(Func<bool> Action)
        {
            this.Action = Action;
        }
        public override void OnStart()
        {
            base.OnStart();
            if (Action != null)
            {
                fnResult = Action();

                if (fnResult) Succ();
                else Fail();
            }
        }
        public override bool OnRunInfo()
        {
            this.Log(this.Action.Method.Name, "吗?", " ", fnResult ? "是的" : "不是");
            return true;
        }
    }
    public class StartStep : Step
    {
        //public Step Curr;
        public override void OnStart()
        {
            base.OnStart();
            Succ();
        }
    }
    /// <summary>
    /// 带帧更新的step 
    /// </summary>
    public class UpdateStep : Step
    {
        private MonoBehaviour go;

        public UpdateStep(MonoBehaviour go)
        {
            this.go = go;
        }

        public override sealed void OnStart()
        {
            base.OnStart();
            go.StartCoroutine(Update());
            OnStarted();
        }
        public virtual void OnStarted()
        {

        }

        private IEnumerator Update()
        {
            OnUpdate();
            yield return null;
        }

        public virtual void OnUpdate()
        {

        }
    }
    //public class NeedUpdateStepT<T> : Step
    //{
    //    private List<Action> updates;
    //    private Func<bool> needUpdateAction;
    //    private Func<T, Func<bool>> action;
    //    private bool addedListener = false;
    //    private T parm;

    //    public NeedUpdateStepT(Func<T,Func<bool>> action, T parm,List<Action> updates)
    //    {
    //        this.parm = parm;
    //        this.updates = updates;
    //        this.action = action;
    //    }
    //    public override void OnStart()
    //    {
    //        updates.Add(Update);
    //        addedListener = true;
    //        needUpdateAction = action(parm);
    //        action = null;
    //    }
    //    public void Update()
    //    {
    //        if (needUpdateAction != null)
    //        {
    //            if (needUpdateAction())
    //            {
    //                needUpdateAction = null;
    //                Succ();
    //            }
    //        }
    //    }
    //    protected override void OnEnd()
    //    {
    //        if (addedListener)
    //        {
    //            updates.Remove(Update);
    //            addedListener = false;
    //        }
    //        action = null;
    //    }
    //}
    public class NeedUpdateStep : Step
    {
        private List<Action> updates;
        private Func<bool> needUpdateAction;
        private Func<Func<bool>> action;
        private bool addedListener = false;
        public NeedUpdateStep(Func<Func<bool>> action, List<Action> updates)
        {
            this.updates = updates;
            this.action = action;
        }
        public override void OnStart()
        {
            updates.Add(Update);
            addedListener = true;
            needUpdateAction = action();
            action = null;
        }
        public void Update()
        {
            if (needUpdateAction != null)
            {
                if (needUpdateAction())
                {
                    needUpdateAction = null;
                    Succ();
                }
            }
        }
        protected override void OnEnd()
        {
            if (addedListener)
            {
                updates.Remove(Update);
                addedListener = false;
            }
            action = null;
        }
    }
    /// <summary>
    /// 自己调用 step.succ() step.fail()
    /// </summary>
    public class ActionStep : Step
    {
        private Action<Step> Action;

        public ActionStep(Action<Step> Action)
        {
            this.Action = Action;
        }
        public override void OnStart()
        {
            base.OnStart();
            if (Action != null)
                Action(this);
        }
    }
    public class ActionTStep<T> : Step
    {
        private Action<T> Action;
        private T parm;
        public ActionTStep(Action<T> Action, T parm)
        {
            this.Action = Action;
            this.parm = parm;
        }
        public override void OnStart()
        {
            base.OnStart();
            if (Action != null)
                Action(parm);
            Succ();
        }
    }
    /// <summary>
    /// 给节点添加多节点支持。
    /// </summary>
    public class MulStep
    {
        private Step fromStep;
        private Step[] thenSteps;
        private Step[] elseSteps;
        private Step oldThenStep;
        private Step oldElseStep;

        public MulStep(Step fromStep)
        {
            this.fromStep = fromStep;

            oldThenStep = fromStep.ThenStep;
            oldElseStep = fromStep.ElseStep;

            fromStep.Then(new RunActionStep(FromStepThen));
            fromStep.Else(new RunActionStep(ElseFromStepThen));
        }
        public void Then(params Step[] args)
        {
            thenSteps = args;
            if (oldThenStep != null)
            {
                thenSteps = new Step[args.Length + 1];
                args.CopyTo(thenSteps, 0);
                thenSteps[args.Length] = oldThenStep;
            }
        }
        public void Else(params Step[] args)
        {
            this.elseSteps = args;
            if (oldElseStep != null)
            {
                elseSteps = new Step[args.Length + 1];
                args.CopyTo(elseSteps, 0);
                elseSteps[args.Length] = oldElseStep;
            }
        }
        private void ArrAddOne(Step[] steps, Step step)
        {

        }
        private void ElseFromStepThen()
        {
            if (elseSteps != null)
            {
                for (int i = 0; i < elseSteps.Length; i++)
                {
                    var step = elseSteps[i];
                    step.ParentData = fromStep.ToNextData;
                }
            }
        }

        private void FromStepThen()
        {
            if (thenSteps != null)
            {
                for (int i = 0; i < thenSteps.Length; i++)
                {
                    var step = thenSteps[i];
                    step.ParentData = fromStep.ToNextData;
                }
            }
        }

    }
    /// <summary>
    /// 选择step
    /// 其中一个子step succ就succ
    /// </summary>
    public class SelectorStep : Step
    {
        private int index;
        private Step[] steps;
        private Step startStep;
        public SelectorStep(params Step[] steps)
        {
            SetData(steps);
        }

        private void SetData(Step[] steps)
        {
            this.steps = steps;
            //startStep = steps[0];
            Step parentStep = null;
            for (int i = 0; i < steps.Length; i++)
            {
                if (startStep == null)
                {
                    startStep = steps[i];
                    startStep.Then(End);
                    parentStep = startStep;
                    continue;
                }
                parentStep.Then(End);
                parentStep = parentStep.Else(steps[i]);
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            startStep.Start();
        }
        private void End()
        {
            startStep.Stop();
            Succ();
        }
    }
    #region 一些扩展方法
    public static class StepTools
    {

        /// <summary>
        /// 把不定数量的steps 设置 then step
        /// </summary>
        /// <param name="thenStep"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Step From(this Step thenStep, params Step[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                args[i].Then(thenStep);
            }
            return thenStep;
        }
        /// <summary>
        /// 把不定数量的steps 设置 Else step
        /// </summary>
        /// <param name="thenStep"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Step ElseFrom(this Step thenStep, params Step[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                args[i].Else(thenStep);
            }
            return thenStep;
        }
        public static MulStep MulThen(this Step step, params Step[] args)
        {
            var t = new MulStep(step);
            t.Then(args);
            return t;
        }

        public static Step Start(this Action<Step> action)
        {
            return new ActionStep(action).Start();
        }
        public static Step Start<T>(this Action<T> action, T data)
        {
            return new ActionTStep<T>(action, data).Start();
        }
        public static Step Start(this Action action)
        {
            return new RunActionStep(action).Start();
        }
        //public static Step Then(this bool op,Step step)
        //{
        //    return new StartStep().Then(step);
        //}

        public static Step Then<T>(this Step step, Action<T> runAction, T parm)
        {
            var nextStep = new ActionTStep<T>(runAction, parm);
            step.Then(nextStep);
            return nextStep;
        }
        public static Step Then(this Step step, Action runAction)
        {
            return step.Then(new RunActionStep(runAction));
        }
        public static Step Then(this Step step, Func<bool> runAction, out Step nextStep)
        {
            nextStep = step.Then(new ActionBoolStep(runAction));
            return nextStep;
        }
        //public static Step Then(this Step step, Func<bool> runAction)
        //{
        //    var nextStep = step.Then(new ActionBoolStep(runAction));
        //    return nextStep;
        //}
        public static Step If(this Step step, Func<bool> runAction)
        {
            return step.Then(new ActionBoolStep(runAction));
        }

        public static Step If(this Step step, Func<bool> runAction,out Step runStep)
        {
            runStep = step.If(runAction);
            return runStep;
        }
        public static Step Then(this Step step, Action<Step> runAction)
        {
            return step.Then(new ActionStep(runAction));
        }
        public static Step Then(this Step step, Func<Func<bool>> runAction, List<Action> Updats)
        {
            return step.Then(new NeedUpdateStep(runAction, Updats));
        }

        public static Step Then(this Step step, Action<Step> runAction, out Step nextStep)
        {
            var r = step.Then(new ActionStep(runAction));
            nextStep = r;
            return r;
        }
        //public static Step ThenOneShot(this Step step,Action runAction)
        //{
        //   return  step.Then(new RunActionStep(runAction));
        //}
        public static Step ThenBool(this Step step, Func<bool> runAction)
        {
            return step.Then(new ActionBoolStep(runAction));
        }
        //public static Step ThenActionStep(this Step step, Action<Step> runAction)
        //{
        //    return step.Then(new ActionStep(runAction));
        //}


        public static Step Else<T>(this Step step, Action<T> runAction, T parm)
        {
            var nextStep = new ActionTStep<T>(runAction, parm);
            step.Else(nextStep);
            return nextStep;
        }
        public static Step Else(this Step step, Action runAction)
        {
            return step.Else(new RunActionStep(runAction));
        }
        public static Step Else(this Step step, Func<bool> runAction, out Step nextStep)
        {
            nextStep = step.Else(new ActionBoolStep(runAction));
            return nextStep;
        }
        public static Step Else(this Step step, Step newNextStep, out Step nextStep)
        {
            nextStep = newNextStep;
            return newNextStep;
        }
        public static Step ElseBool(this Step step, Func<bool> runAction)
        {
            return step.Else(new ActionBoolStep(runAction));
        }
        public static Step Else(this Step step, Action<Step> runAction)
        {
            return step.Else(new ActionStep(runAction));
        }
        public static Step Else(this Step step, Action<Step> runAction, out Step nextStep)
        {
            nextStep = step.Else(new ActionStep(runAction));
            return nextStep;
        }
        public static Step Select(this Step step, params Step[] steps)
        {
            var t = new SelectorStep(steps);
            return t;
        }
        public static Step Select(this Step step, params Func<bool>[] steps)
        {
            var t = new Step[steps.Length];
            for (int i = 0; i < steps.Length; i++)
            {
                t[i] = new ActionBoolStep(steps[i]);
            }
            var t1 = new SelectorStep(t);
            step.Then(t1);
            return t1;
        }
    }

    #endregion
}

#endregion
#region Handle
namespace ZjwTools
{
    public interface IHandle
    {
        void Update();
    }
    public class Handle : IHandle
    {
        public void Update()
        {
            OnUpdate();
        }
        protected virtual void OnUpdate()
        {

        }
    }
    public class Handles
    {
        private List<Handle> handles;
        private IEnumerator iEnumerator;
        private bool started = false;
        private bool running = false;
        private MonoBehaviour go;

        public Handles()
        {
            handles = new List<Handle>();
        }
        public Handles(params Handle[] parms)
        {
            handles = new List<Handle>(parms);
        }
        private IEnumerator Update()
        {
            while (running)
            {
                for (int i = 0; i < handles.Count; i++)
                {
                    handles[i].Update();
                }
                yield return null;
            }
        }
        public Handles Init(MonoBehaviour go)
        {
            this.go = go;
            iEnumerator = Update();
            running = true;
            go.StartCoroutine(iEnumerator);
            return this;
        }
        public void Stop()
        {
            running = false;
            if (!started) return;
            if (go == null) return;
            //go.StopIEnumerator(iEnumerator);
            go.StopCoroutine(iEnumerator);
        }
    }


}


#endregion
#region 属性绑定


namespace ZjwTools
{
    /// <summary>
    /// 绑定的数据集合。
    /// 
    /// </summary>
    public class RpData : IReset
    {

        private List<IReset> propertys;

        public List<IReset> Propertys
        {
            get
            {
                if (propertys == null) propertys = new List<IReset>();
                return propertys;
            }
        }

        public void Clear()
        {
            if (propertys == null) return;
            for (int i = 0; i < Propertys.Count; i++)
            {
                Propertys[i].Reset();
            }
        }
        public IReset Add(IReset data)
        {
            Propertys.Add(data);
            return data;
        }
        public BindList<T> AddBindList<T>()
        {
            var t = new BindList<T>();
            Propertys.Add(t);
            return t;
        }
        public RpProperty<T> AddNewProperty<T>(T value)
        {
            var data = new RpProperty<T>();
            data.Value = value;
            Add(data);
            return data;
        }
        public static RpProperty<T> NewProperty<T>(T value)
        {
            var data = new RpProperty<T>();
            data.Value = value;
            return data;
        }
        public RpProperty<T> AddNewProperty<T>()
        {
            var data = new RpProperty<T>();
            Add(data);
            return data;
        }

        public void Reset()
        {
            Clear();
        }
    }
    public interface IReset
    {
        void Reset();
    }
    public class RpProperty<ValueType> : IReset
    {
        private ValueType value;

        public ValueType Value
        {
            get
            {
                return value;
            }

            set
            {
                if (EqualityComparer.Equals(this.value, value)) return;
                //if (this.value == value) return;
                this.value = value;
                if (pauseBind) return;
                if (rpHandle != null) rpHandle.Run(value);
            }
        }
        private EqualityComparer<ValueType> EqualityComparer;
        public RpProperty()
        {
            EqualityComparer = EqualityComparer<ValueType>.Default;
        }

        private bool pauseBind;
        public bool PauseBind
        {
            get
            {
                return pauseBind;
            }

            set
            {
                pauseBind = value;
            }
        }
        public RpHandle<ValueType> RpHandle
        {
            get
            {
                if (rpHandle == null) rpHandle = new RpHandle<ValueType>();
                return rpHandle;
            }
        }



        private RpHandle<ValueType> rpHandle;
        public RpProperty<ValueType> Bind(Action<ValueType> handle,bool run = true)
        {
            RpHandle.Bind(handle);
            if(run)
            RpHandle.Run(value);
            return this;
        }
        public void Reset()
        {
            pauseBind = false;
            if (rpHandle != null) rpHandle.Reset();
            rpHandle = null;
        }
    }
    public class RpHandle<ValueType> : IReset
    {
        private Action<ValueType> handle;
        private List<Action<ValueType>> handles;
        public void Bind()
        {

        }
        public void Bind(Action<ValueType> handle)
        {
            TryBind(handle);
        }
        private void TryBind(Action<ValueType> handle)
        {
            if (handles != null)
            {
                handles.Add(handle);
                return;
            }
            if (this.handle == null)
            {
                this.handle = handle;
                return;
            }
            handles = new List<Action<ValueType>>
            {
                this.handle,
                handle
            };
        }
        public void Remove(Action<ValueType> handle)
        {
            if (handles != null)
            {
                handles.Remove(handle);
                return;
            }
            if (this.handle == handle)
            {
                this.handle = null;
                return;
            }
            Debug.LogError("不存在的bind？");
        }

        public void Reset()
        {
            if (handles != null)
            {
                handles.Clear();
                handles = null;
            }
            handle = null;
        }

        public void Run(ValueType value)
        {
            if (handles != null)
            {
                for (int i = 0; i < handles.Count; i++)
                {
                    if(handles[i]==null)continue;
                    handles[i](value);
                }
                return;
            }
            if (handle == null) return;
            handle(value);
        }
    }

    public class BindList<T> : IReset
    {
        private RpData rpdata;
        private List<T> lists;
        private RpProperty<T> addItem;
        private RpProperty<T> removeItem;
        private bool binding = false;
        //private Action<T> onAddItem;
        //private Action<T> onRemoveItem;
        public BindList()
        {
            lists = new List<T>();
        }
        public BindList(Action<T> onAddItem, Action<T> onRemoveItem)
        {
            lists = new List<T>();
            Bind(onAddItem, onRemoveItem);
        }

        public void Bind(Action<T> onAddItem, Action<T> onRemoveItem)
        {
            addItem = Rpdata.AddNewProperty<T>().Bind(onAddItem,false);
            removeItem = Rpdata.AddNewProperty<T>().Bind(onRemoveItem,false);
            binding = true;
            if (lists.Count > 0)
            {
                for (int i = 0; i < lists.Count; i++)
                {
                    onAddItem(lists[i]);
                }
            }
        }
        public T GetItem(Func<T,bool> match)
        {
            for (int i = 0; i < lists.Count; i++)
            {
                var item = lists[i];
                if (match(item)) return item;
            }
            return default(T);
        }
        public List<T> GetItems(Func<T, bool> match)
        {
            
            List<T> t = null;
            for (int i = 0; i < lists.Count; i++)
            {
                var item = lists[i];
                if (match(item))
                {
                    if (t == null) t = new List<T>();
                    t.Add(item);
                }
            }

            return t;
        }
        public RpData Rpdata
        {
            get
            {
                rpdata = new RpData();
                return rpdata;
            }
        }
        public void Clear(bool clearBind = true)
        {
            if (removeItem != null)
            {
                for (int i = 0; i < lists.Count; i++)
                {
                    removeItem.Value = lists[i];
                }
            }
            lists.Clear();
            if (clearBind && binding)
            {
                rpdata.Clear();
                addItem = null;
                removeItem = null;
            }
        }
        public void RemoveItem(T item){
            if (removeItem == null) return;

            lists.Remove(item);

            if (binding)
                removeItem.Value = item;
            
        }
        public void Add(T item)
        {

            lists.Add(item);
            if (binding)
                addItem.Value = item;
        }
        public void AddItems(T[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                Add(items[i]);
            }
        }
        public void AddItems(List<T> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Add(items[i]);
            }
        }

        public void Reset()
        {
            Clear();
        }
    }
}

#endregion