using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
public class AwaitTools
{
    public class AwaitFrame : INotifyCompletion
    {
        private bool isWaiting = true;

        private Action continuation;
        public int WaitCount;

        public AwaitFrame()
        {
        }
        public bool IsCompleted
        {
            get
            {
                return !isWaiting;
            }
        }
        public void End()
        {
            isWaiting = false;
            continuation();
        }
        public void Reset()
        {
            isWaiting = true;
        }

        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
            //这个方法只有ui组件被dispose之后才会执行.不需要continuation
            if (!isWaiting)
            {
                continuation();
            }
        }
        public void GetResult()
        {

        }
        public AwaitFrame GetAwaiter()
        {
            return this;
        }

    }
    private List<AwaitFrame> waits = new List<AwaitFrame>();
    private List<AwaitFrame> waitTemps = new List<AwaitFrame>();
    private List<AwaitFrame> waitPool = new List<AwaitFrame>();
    protected List<AwaitTools> childs = new List<AwaitTools>();
    public void Update()
    {
        for (int j = 0; j < childs.Count; j++)
        {
            childs[j].Update();
        }
        if (waitTemps.Count > 0)
        {
            for (int i = 0; i < waitTemps.Count; i++)
            {
                var one = waitTemps[i];
                one.Reset();
                waitPool.Add(one);
            }
            waitTemps.Clear();
        }
        var count = waits.Count;
        //if (count > 0)
        //{
        //    ZLog.Info($"等待一帧 {waits.Count}");
        //}
        for (int i = 0; i < count; i++)
        {
            waitTemps.Add(waits[i]);
        }
        waits.Clear();
        for (int i = 0; i < waitTemps.Count; i++)
        {
            waitTemps[i].End();
        }
    }
    public async Task WaitBool(Func<bool> isWaiting)
    {
        while (true)
        {
            if (isWaiting())
            {
                await WaitFrame();
            }
            else
            {
                break;
            }

        }
    }
    public async Task WaitLoop(Action action)
    {
        while (true)
        {
            await WaitFrame();
            action();
        }
    }
    public async Task WaitFrame()
    {
        AwaitFrame t = null;
        if (waitPool.Count > 0)
        {
            t = waitPool[0];
            waitPool.RemoveAt(0);
        }
        else
        {
            t = new AwaitFrame();
        }
        waits.Add(t);
        await t;
    }


    public AwaitTools AddNewWaitChild(AwaitTools child)
    {
        //var t = new AwaitTools();
        childs.Add(child);
        return child;
    }
    public bool RemoveWaitChild(AwaitTools child)
    {
       return childs.Remove(child);
    }
    public void ClearChild()
    {
        childs.Clear();
    }


}


