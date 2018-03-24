using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public sealed class TaskList : TaskBase
{
    /// <summary>
    /// 队列任务是顺序执行还是并行执行
    /// </summary>
    bool isSerial = false;
    public static TaskList serial(params TaskBase[]  tasks){
        return serial(tasks,true);
    }
    public static TaskList parallel(params TaskBase[] tasks)
    {
        return parallel(tasks, true);
    }
    /// <summary>
    /// 创建顺序队列
    /// </summary>
    /// <param name="array">初始任务队列</param>
    /// <param name="autoComplete">当队列没有任务时是否结束TaskList</param>
    /// <returns></returns>
    public static TaskList serial( TaskBase[] array = null, bool autoComplete = true)
    {
        TaskList taskList = new TaskList().Init(autoComplete,true); 

        if (array == null)
            return taskList;

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != null)
            {
                taskList.PushBack(array[i]);
            }
        }
        return taskList;
    }

    /// <summary>
    /// 创建并行队列
    /// </summary>
    /// <param name="array">初始任务队列</param>
    /// <param name="autoComplete">当队列没有任务时是否结束TaskList</param>
    /// <returns></returns>
    public static TaskList parallel(TaskBase[] array = null, bool autoComplete = true)
    {
        TaskList taskList = new TaskList().Init(autoComplete,false); 

        if (array == null)
            return taskList;

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != null)
            {
                taskList.PushBack(array[i]);
            }
        }
        return taskList;
    }

    public List<TaskBase> tasks;
    List<TaskBase> tasksLateUpate;

    int index = 0;

    public int Index
    {
        get
        {
            return index;
        }
    }

    /// <summary>
    /// 顺序队列当前正在执行的任务
    /// </summary>
    public TaskBase CurrentTask
    {
        get
        {
            if (tasks.Count > Index)
            {
                return tasks[Index];
            }
            return null;
        }
    }

    public int Count
    {
        get
        {
            return tasks.Count;
        }
    }

    /// <summary>
    /// 当队列没有任务时是否结束TaskList
    /// </summary>
    bool autoComplete;
     
    bool isUpdateing = false;

    public TaskList()
    {
        tasks = new List<TaskBase>();
        tasksLateUpate = new List<TaskBase>();
        hasLateUpate = true;
    }

    /// <summary>
    /// TaskList Init 
    /// </summary>
    /// <param name="autoComplete">当队列没有任务时是否结束TaskList</param>
    /// <param name="isSerial">是否是顺序队列</param>
    /// <returns></returns>
    public TaskList Init(bool autoComplete,bool isSerial)
    {
        this.isSerial = isSerial;
        this.autoComplete = autoComplete;
        onCancelled += CancelSubTasks;
        return this;
    }

    /// <summary>
    /// 在队列末尾添加新任务
    /// </summary>
    /// <param name="task"></param>
    public void PushBack(TaskBase task)
    {
        if (tasks.Contains(task))
            return;
        tasks.Add(task);
        if (!isSerial && task.HasLateUpate)
            tasksLateUpate.Add(task);
    }

    /// <summary>
    /// 在队列头部添加新任务
    /// </summary>
    /// <param name="task"></param>
    public void PushFront(TaskBase task)
    {
        if (tasks.Contains(task))
            return;
        tasks.Insert(0, task);
        if (!isSerial && task.HasLateUpate)
            tasksLateUpate.Insert(0, task);
    }

    protected override void Dispose()
    {
        base.Dispose();
        this.isSerial = false;
        this.index = 0;
        this.autoComplete = false;
        this.tasks.Clear();
        this.tasksLateUpate.Clear();
    }

    public override void Update(float deltaTime)
    {
        if (IsPaused || IsFinished  )
            return;

        if(isUpdateing)
        {
            //Debug.LogError("不能更新TaskList，当它正在更新的时候");
            return;
        }

        base.Update(deltaTime); 
        
        isUpdateing = true;
        for (int i = 0; i < tasks.Count; i++)
        {
            TaskBase task = tasks[i];

            if (!task.IsFinished && !task.IsPaused)
            {
                task.Update(deltaTime);
            }

            //防止TaskList已经被Cancel
            if (this.IsFinished)
            {
                break;
            }

            if (task.IsFinished)
            {
                tasks.RemoveAt(i);
                if (task.HasLateUpate)
                    tasksLateUpate.Remove(task);
                --i;
                continue;
            }

            index = i;

            if (isSerial)
            {
                break;
            }
        }

        if (tasks.Count == 0 && autoComplete)
        {
            Complete();
        }
        isUpdateing = false;
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        if (IsPaused || IsFinished)
        {
            return;
        }

        if (isSerial)
        {
            TaskBase currentTask = this.CurrentTask;
            if (currentTask != null && currentTask.HasLateUpate && !currentTask.IsFinished && !currentTask.IsPaused)
                currentTask.LateUpdate();
            return;
        }

        for (int i = 0; i < tasksLateUpate.Count; i++)
        {
            TaskBase task = tasksLateUpate[i];

            if (!task.IsFinished && !task.IsPaused)
            {
                task.LateUpdate();
            }

            //防止TaskList已经被Cancel
            if (this.IsFinished)
            {
                break;
            }
        }
    }

    void CancelSubTasks()
    {
        for (int i = 0; i < tasks.Count; i++)
        {
            tasks[i].Cancel();
        }

        this.tasks.Clear();
        this.tasksLateUpate.Clear();
    }
}