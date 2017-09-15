using System;
using System.Collections.Generic;

namespace TestTemp.TestFrameWork
{
    /// <summary>
    /// 设计为所有的对象都继承这个,一个数据类型只能有一个监听
    /// 事件设计为父对象处理
    ///
    /// </summary>
    public class Item
    {
        public int Id;
        private List<Item> mMyItems;
        private DictList<Type,bool> mEvents;
        public void AddListener<T>(){
            if(mEvents==null)mEvents=new DictList<Type, bool>();
            var type = typeof(T);
            if(mEvents.ContainsKey(type))
            {
                Log("不要重复监听");
                return;
            }
            //监听自身发出的事件
            //监听子对象事件
            if(mMyItems==null)return;
            for (int i = 0; i < mMyItems.Count; i++)
            {
                mMyItems[i].AddListener<T>();
            }
        }
        private void Log(string info){
            //
        }
        protected  void DisatchEvent<T>(T data){

        }
        public void OnEvent<T>(T data){
            
        }
        public void AddItem(Item item){
            if(mMyItems==null)mMyItems=new List<Item>();
            mMyItems.Add(item);
        }
        public class LogData{
            public string Info;
            
        }
    }

    public class Data{
        //数据基类
        //数据可追踪
    }
}
