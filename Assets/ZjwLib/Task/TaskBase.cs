using UnityEngine;
using System;
using System.Collections;

public class TaskBase
{
    public Signal onStarted;
    public Signal onCompleted;
    public Signal onCancelled;
    public Signal onPaused;
    public Signal onFinished;
    public Signal onResumed;

    protected bool isStarted = false;

    public bool IsStarted
    {
        get
        {
            return isStarted;
        }
    }

    protected bool isCompleted = false; 
    public bool IsCompleted { get { return isCompleted; } }

    protected bool isFinished = false;

    public bool IsFinished
    {
        get
        {
            return isFinished;
        }
    }

    protected bool isPaused = false;

    public bool IsPaused
    {
        get
        {
            return isPaused;
        }
    } 

    protected float timeElpase = -1;

    protected float deltaTime;
    public float DeltaTime
    {
        get
        {
            return deltaTime;
        }
    }

    /// <summary>
    /// 构造的时候设置，不能在Start里设置
    /// </summary>
    protected bool hasLateUpate = false;
    public bool HasLateUpate
    {
        get
        {
            return hasLateUpate;
        }
    } 
  
    public virtual void Update(float deltaTime)
    {
        if (!IsStarted)
        {
            isStarted = true;
            timeElpase = 0;
            Start();
            onStarted.Dispatch();
        }

        this.deltaTime = deltaTime;
        timeElpase += deltaTime;
    }

    public virtual void LateUpdate()
    {

    }

    protected virtual void Start()
    {
    }

    //only the action can complete itself
    protected void Complete()
    {
        if (IsFinished)
            return;
        isCompleted = true;
        isFinished = true;
        onCompleted.Dispatch();
        onFinished.Dispatch();
        Dispose();
    }

    public void Pause()
    {
        isPaused = true;
        onPaused.Dispatch();
    }

    public void Resume()
    {
        isPaused = false;
        onResumed.Dispatch();
    }

    public void Cancel()
    {
        if (IsFinished)
            return; 
        isFinished = true;
        onCancelled.Dispatch();
        onFinished.Dispatch();
        Dispose();
    }

    public virtual void ResetTaskState()
    {
        isStarted = isPaused = isFinished = isCompleted = false;
        timeElpase = -1;
    }

    protected virtual void Dispose()
    {  
        onStarted.Clear();
        onCompleted.Clear();
        onCancelled.Clear();
        onPaused.Clear();
        onFinished.Clear();
        onResumed.Clear(); 
    }
}