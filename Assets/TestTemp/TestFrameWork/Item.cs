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
        private long mInstanceId;
        private readonly IConnect mConnect = new Connect();
        public IConnect Connect{get{return mConnect;}}

        private readonly List<Item> mMyItems =new List<Item>();
        private readonly DictList<Type,Delegate> mEvents =new DictList<Type, Delegate>();
        protected void AddListen<T>(Action<T> OnEvent){
            var type = typeof(T);
            if(mEvents.ContainsKey(type))
            {
                Log("不要重复监听");
                return;
            }
            //if(mMyItems==null)return;
            //for (int i = 0; i < mMyItems.Count; i++)
            //{
            //    mMyItems[i].Connect.AddListen<T>();
            //}
        }
        protected void RemoveListen<T>()
        {
            mEvents.Remove(typeof(T));
        }
        public void DoEvent<T>(T data)
        {
            var type = typeof(T);
            if (!mEvents.ContainsKey(type))
            {
                Connect.FatherDoEvent(data);
                return;
            }
            var action = mEvents.GetItem(type) as Action<T>;
            action(data);
        }

        private void Log(string info){
            //
        }
        protected  void DisatchEvent<T>(T data)
        {
            Connect.FatherDoEvent(data);
        }
        public void AddItem(Item item){
            mMyItems.Add(item);
        }
        public class LogData{
            public string Info;
            
        }
    }
        
}
