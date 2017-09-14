using System;
using System.Collections.Generic;

namespace zjw.TestFrameWork
{
    public class Item
    {
        private readonly EventDispatcher  mEventDispatcher = new EventDispatcher();
        private List<Item> mMyObjs;
        public void AddListener<T>(Action<T> onEvent){
            //监听自身发出的事件
            mEventDispatcher.AddListener(onEvent);
            //监听子对象事件
        }
        public void AddItem(Item man){
            if(mMyObjs==null)mMyObjs=new List<Item>();
            mMyObjs.Add(man);
        }

    }
}
