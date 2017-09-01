
using System;
using System.Runtime.CompilerServices;


public class AwaitStep :INotifyCompletion
{
    private Action continuation;
    private bool isCompleted;
    public bool IsCompleted { get { return isCompleted; } }
    public virtual void OnCompleted(Action continuation)
    {
        this.continuation = continuation;
        Start();
    }
    private AwaitStep Start()
    {
        OnStart();
        return this;
    }
    protected virtual void OnStart()
    {

    }
    public void Complete()
    {
        isCompleted = true;
        continuation();
    }
    public void GetResult() { }
    public AwaitStep GetAwaiter() => this;
}
