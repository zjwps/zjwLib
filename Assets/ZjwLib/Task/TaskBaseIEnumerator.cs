using System.Collections;
using UnityEngine;
public class TaskBaseIEnumerator:IEnumerator
{
    protected TaskBase task;

    public TaskBaseIEnumerator(TaskBase task)
    {
        this.task = task;
    }

    public object Current { get { return null; } }

    public bool MoveNext()
    {
        if (task.IsFinished) return false;
        task.Update(Time.deltaTime);
        return !task.IsFinished;
    }
    public string GetTaskName(){
        return task.GetType().Name;
    }
    public void Cancel(){
        if(task!=null)
        task.Cancel();
    }


    public void Reset()
    {
        //task.Cancel();
    }
} 
