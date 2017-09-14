using System;
using System.Collections.Generic;
public class EventDispatcher
{
    private Dictionary<Type,List<Delegate>> mListenerMap;
    public void DisPatchEvent<T>(T data){
        var type = typeof(T);
        if (!mListenerMap.ContainsKey(type)) return;
        var listeners = mListenerMap[type];
        Delegate action=null;
        for (int i = 0; i < listeners.Count; i++)
        {
            action = listeners[i];
            if(action==null)continue;
            ((Action<T>)action)(data);
        }
        
    }
    public void AddListener<T>(Action<T> onEvent){
        var type = typeof(T);
        if (mListenerMap == null) mListenerMap = new Dictionary<Type, List<Delegate>>();
        if(!mListenerMap.ContainsKey(type)){
            mListenerMap.Add(type ,new List<Delegate>());
        }
        mListenerMap[type].Add(onEvent);
    }
    public void RemoveListener<T>(Action<T> onEvent){
        var type = typeof(T);
        if (!mListenerMap.ContainsKey(type))return;
        mListenerMap[type].Remove(onEvent);
    }
}
