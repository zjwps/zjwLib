using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/// <summary>
/// AddListener: 避免重复添加
/// AddOnce: 避免重复添加, 派发后清空侦听;
/// </summary>  
public static class SignalExtensions
{
    //Signal 
    public static void Dispatch(this Signal signal)
    {
        if (signal == null)
            return;
        signal.DispatchInternal(); 
    }

    public static void Clear(this Signal signal)
    {
        if (signal == null)
            return;
        signal.ClearInternal();
    }

    //Signal<T> 
    public static void Dispatch<T>(this Signal<T> signal,T t)
    {
        if (signal == null)
            return;
        signal.DispatchInternal(t);
    }

    public static void Clear<T>(this Signal<T> signal)
    {
        if (signal == null)
            return;
        signal.ClearInternal();
    }
}

public partial class Signal
{
    List<Action> list;
    List<Action> listOnce;

    public Signal()
    {
        list = new List<Action>();
        listOnce = new List<Action>();
    } 

    /// <summary>
    /// add linstener
    /// </summary>
    /// <param name="signal"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static Signal operator +(Signal signal,Action callback)
    {
        if (callback == null)
            return signal; 

        if (signal == null)
            signal = new Signal(); 
        else if (signal.list.Contains(callback))
            return signal;

        signal.list.Add(callback);
        return signal;
    }

    /// <summary>
    /// remove linstener
    /// </summary>
    /// <param name="signal"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static Signal operator -(Signal signal, Action callback)
    {
        if (callback == null || signal == null)
            return signal;
        signal.list.Remove(callback);
        return signal;
    }

    /// <summary>
    /// add once
    /// </summary>
    /// <param name="signal"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static Signal operator *(Signal signal, Action callback)
    {
        if (callback == null)
            return signal;

        if (signal == null)
            signal = new Signal();
        else if (signal.listOnce.Contains(callback))
            return signal;

        signal.listOnce.Add(callback);
        return signal;
    }
    
    /// <summary>
    /// remove once
    /// </summary>
    /// <param name="signal"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static Signal operator /(Signal signal, Action callback)
    {
        if (callback == null || signal == null)
            return signal;
        signal.listOnce.Remove(callback);
        return signal;
    }

    internal void DispatchInternal()
    {
        if(list.Count>0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i]();
            }
        }
        if(listOnce.Count>0)
        {
            for (int i = 0; i < listOnce.Count; i++)
            {
                listOnce[i]();
            }
            listOnce.Clear();
        } 
    }

    internal void ClearInternal()
    {
        listOnce.Clear();
        list.Clear();
    }
}

public partial class Signal<T>
{
    List<Action<T>> list;
    List<Action<T>> listOnce;

    public Signal()
    {
        list = new List<Action<T>>();
        listOnce = new List<Action<T>>();
    } 

    /// <summary>
    /// add linstener
    /// </summary>
    /// <param name="signal"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static Signal<T> operator +(Signal<T> signal, Action<T> callback)
    {
        if (callback == null)
            return signal;

        if (signal == null)
            signal = new Signal<T>();
        else if (signal.list.Contains(callback))
            return signal;

        signal.list.Add(callback);
        return signal;
    }

    /// <summary>
    /// remove linstener
    /// </summary>
    /// <param name="signal"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static Signal<T> operator -(Signal<T> signal, Action<T> callback)
    {
        if (callback == null || signal == null)
            return signal;
        signal.list.Remove(callback);
        return signal;
    }

    /// <summary>
    /// add once
    /// </summary>
    /// <param name="signal"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static Signal<T> operator *(Signal<T> signal, Action<T> callback)
    {
        if (callback == null)
            return signal;

        if (signal == null)
            signal = new Signal<T>();
        else if (signal.listOnce.Contains(callback))
            return signal;

        signal.listOnce.Add(callback);
        return signal;
    }

    /// <summary>
    /// remove once
    /// </summary>
    /// <param name="signal"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static Signal<T> operator /(Signal<T> signal, Action<T> callback)
    {
        if (callback == null || signal == null)
            return signal;
        signal.listOnce.Remove(callback);
        return signal;
    }

    internal void DispatchInternal(T t)
    {
        if(list.Count>0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i](t);
            }
        }
        
        if(listOnce.Count>0)
        {
            for (int i = 0; i < listOnce.Count; i++)
            {
                listOnce[i](t);
            }
            listOnce.Clear();
        } 
    }

    internal void ClearInternal()
    {
        listOnce.Clear();
        list.Clear();
    }
}
 

