using System;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Update驱动的Yield异步实现.
/// </summary>
public class YieldAsync
{
    private List<Func<bool>> _asyncs = new List<Func<bool>>();
    public void StartAsync(IEnumerator enumerator)
    {
        _asyncs.Add(BuildOne(enumerator));
    }
    public void Update()
    {
        for (int i = 0; i < _asyncs.Count; i++)
        {
            var one = _asyncs[i];
            if (_asyncs[i]())
            {
                _asyncs.RemoveAt(i);
                i--;
            }
        }
    }
    private Func<bool> BuildOne(IEnumerator enumerator)
    {
        //0 未开始 1 运行中 2 结束了
        var state = 1;
        var oldCount = 0;
        var needUpdates = new List<Func<bool>>();
        Func<IEnumerator, bool> update = null;
        update = (item) =>
        {
            if (item == null) return true;
            if (!item.MoveNext()) return true;
            if (item.Current == null) return false;
            if (item.Current is IEnumerator)
            {
                needUpdates.Add(() => update(item.Current as IEnumerator));
            }
            return false;
        };
        needUpdates.Add(() => update(enumerator));

        var index = 0;
        Func<bool> fn = () =>
        {
            if (state == 2) return true;
            oldCount = needUpdates.Count;
            if (oldCount > 0)
            {
                index = needUpdates.Count - 1;
                if (needUpdates[index]())
                {
                    needUpdates.RemoveAt(index);
                    if (needUpdates.Count >= oldCount)
                    {
                        Update();
                    }
                }
                else
                {
                    if (needUpdates.Count > oldCount)
                    {
                        Update();
                    }
                }

            }
            if (needUpdates.Count == 0)
            {
                state = 2;
                return true;
            }
            return false;
        };
        return fn;
    }
}
/// <summary>
/// 简单IEnumerator异步
/// </summary>
public class TaskIEnumerator
{
    public List<OneIEnumerator> IEnumerators { get; } = new List<OneIEnumerator>();
    public void StartAsync(IEnumerator enumerator)
    {
        var oneIEnumerator = new OneIEnumerator(enumerator);
        IEnumerators.Add(oneIEnumerator);
    }
    public void Update()
    {
        for (int i = 0; i < IEnumerators.Count; i++)
        {
            var oneIEnumerator = IEnumerators[i];
            if (oneIEnumerator.IsEnd)
            {
                IEnumerators.RemoveAt(i);
                i--;
            }
            else
            {
                oneIEnumerator.Update();
            }
        }
    }

}
public class OneIEnumerator
{
    private IEnumerator enumerator;
    private List<Func<bool>> needUpdates;
    /// <summary>
    /// 0 未开始 1 运行中 2 结束了
    /// </summary>
    public int State { private set; get; } = 0;
    public bool IsEnd { get { return State == 2; } }
    public OneIEnumerator(IEnumerator enumerator)
    {
        this.enumerator = enumerator;
    }

    protected void Start()
    {
        needUpdates = new List<Func<bool>>();
        SetUpdate();
    }
    private void SetUpdate()
    {
        //var item = enumerator;
        Func<IEnumerator, bool> update = null;
        update = (item) =>
        {
            if (item == null) return true;
            if (!item.MoveNext()) return true;
            if (item.Current == null) return false;
            if (item.Current is IEnumerator)
            {
                needUpdates.Add(() => update(item.Current as IEnumerator));
            }
            return false;
        };
        needUpdates.Add(() => update(enumerator));
    }
    private int index = 0;
    private int oldCount = 0;
    public void Update()
    {
        if (State == 0)
        {
            Start();
            State = 1;
        }
        if (State == 2) return;
        oldCount = needUpdates.Count;
        if (oldCount > 0)
        {
            index = needUpdates.Count - 1;
            if (needUpdates[index]())
            {
                needUpdates.RemoveAt(index);
                if (needUpdates.Count >= oldCount)
                {
                    Update();
                }
            }
            else
            {
                if (needUpdates.Count > oldCount)
                {
                    Update();
                }
            }

        }
        if (needUpdates.Count == 0) State = 2;
    }
}