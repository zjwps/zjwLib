using System;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 结合TaskBase实现的一个异步协程
/// </summary>
public class TaskAsync:TaskBase{
    private TaskList mTaskList;
    //同id只会有一个task
    private Dictionary<int, TaskBase> mTaskMap;
    protected override void Start()
    {
        mTaskList = TaskList.parallel(null, false);
        onCancelled += mTaskList.Cancel;
    }
    protected override void Dispose()
    {
        base.Dispose();
        
        if (mTaskList == null) return;
        mTaskList = null;

    }
    public TaskBase RunTask(TaskBase task)
    {
        mTaskList.PushBack(task);
        return task;
    }
    public T RunTask<T>(T task, int id)where T:TaskBase
    {
        if (mTaskMap == null) mTaskMap = new Dictionary<int, TaskBase>();
        if (mTaskMap.ContainsKey(id))
        {
            var oldTask = mTaskMap[id];
            if (oldTask != null)
            {
                oldTask.Cancel();
                //ZLog.Info("干掉了老任务 id: ",id);
            }
            mTaskMap.Remove(id);
        }
        mTaskMap.Add(id, task);
        mTaskList.PushBack(task);
        return task;
    }
    public TaskBase StartAsync(IEnumerator enumerator)
    {
        TryStart();
        var task = new TaskAwait(enumerator);
        mTaskList.PushBack(task);
        return task;
    }
    public TaskBase StartAsync(IEnumerator enumerator,int id)
    {
        TryStart();
        var task = new TaskAwait(enumerator);
        RunTask(task,id);
        return task;
    }

    private void TryStart()
    {
        //确保已经开始了,或者改为没开始就不执行
        if (!isStarted)
        {
            Update(0);
        }
    }
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if(mTaskList!=null)
        mTaskList.Update(deltaTime);
        
    }
    public class TaskAwait : TaskBase
    {
        private IEnumerator enumerator;
        private List<Func<bool>> needUpdates;
        private TaskBaseIEnumerator mRunningTask;

        public TaskAwait(IEnumerator enumerator)
        {
            this.enumerator = enumerator;
            
        }
        private void CancelRunningTask()
        {
            if (mRunningTask != null)
            {
                mRunningTask.Cancel();
                mRunningTask = null;
            }
        }
        protected override void Dispose()
        {
            base.Dispose();
            enumerator = null;
        }
        protected override void Start()
        {
            base.Start();
            onCancelled += CancelRunningTask;
            needUpdates = new List<Func<bool>>();
            SetUpdate();
        }
        private void SetUpdate()
        {
            //var item = enumerator;
            Func<IEnumerator, bool> update = null;
            update = (item) =>
           {
               if (item is TaskBaseIEnumerator) mRunningTask = item as TaskBaseIEnumerator;
               if (item == null) return true;
               if (!item.MoveNext()) return true;
               if (item.Current == null) return false;
               if (item.Current is IEnumerator)
               {
                   needUpdates.Add(() => update(item.Current as IEnumerator));
               }
               return false;
           };
            needUpdates.Add(()=>update(enumerator) );
        }
        private int index = 0;
        private int oldCount = 0;
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            //只更新最后一个update
            oldCount = needUpdates.Count;
            if (oldCount > 0)
            {
                index = needUpdates.Count - 1;
                if (needUpdates[index]())
                {
                    needUpdates.RemoveAt(index);
                    if (needUpdates.Count >= oldCount)
                    {
                        Update(0);
                    }
                }
                else
                {
                    if (needUpdates.Count > oldCount)
                    {
                        Update(0);
                    }
                }
                
            }
            if (needUpdates.Count == 0) Complete();
        }
    }
}
