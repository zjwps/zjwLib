using System;

namespace TestTemp.TestFrameWork
{
    public class Connect : IConnect
    {
        private bool mBinded = false;
        private Item father;
        private Item child;
        private DictList<Type, bool> mListenedMap;
        public void Bind(Item father, Item child)
        {
            this.child = child;
            this.father = father;
        }
        public void FatherDoEvent<T>(T data)
        {
            var type = typeof(T);
            if (father == null) return;
            father.DoEvent(data);
        }

        public void ChildDoEvent<T>(T data)
        {
            if (child == null) return;
            
        }
        
    }
    
    public interface IConnect{
        void FatherDoEvent<T>(T data);
        void ChildDoEvent<T>(T data);
    }
}
