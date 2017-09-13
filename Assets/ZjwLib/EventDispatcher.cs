using System;
using System.Collections.Generic;
public class EventDispatcher
{
    private Dictionary<Type,List<Delegate>> listenerMap;
    public void DisPatchEvent<T>(T data){
        var type = typeof(T);
        if (!listenerMap.ContainsKey(type)) return;
        var listeners = listenerMap[type];
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
        if (listenerMap == null) listenerMap = new Dictionary<Type, List<Delegate>>();
        if(!listenerMap.ContainsKey(type)){
            listenerMap.Add(type ,new List<Delegate>());
        }
        listenerMap[type].Add(onEvent);
    }
    public void RemoveListener<T>(Action<T> onEvent){
        var type = typeof(T);
        if (!listenerMap.ContainsKey(type))return;
        listenerMap[type].Remove(onEvent);
    }
}
