
using System;
using System.Runtime.CompilerServices;
public static class FuncEx{
    public static Func<X,Func<Y,R>> Currey<X,Y,R>(this Func<X,Y,R> func){
        return x=>y=>func(x,y);
    }
}

public class AwaitStep :INotifyCompletion
{
    public event Action OnStartAction;
    private Action continuation;
    private bool isCompleted;
    public bool IsCompleted { get { return isCompleted; } }
    public virtual void OnCompleted(Action continuation)
    {
        this.continuation = continuation;
        if(OnStartAction!=null)OnStartAction();
        OnStart();
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
