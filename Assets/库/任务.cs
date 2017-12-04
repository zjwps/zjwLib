using System;

namespace 库
{
    public class 任务
    {
        public bool 完成了 { get; private set; }
        protected event Func<bool> 开始;
        protected 任务(){

        }
        public static 任务 创建一个任务(){
            return new 任务();
        }
        public 任务 准备开始()
        {
            if (开始 != null && 开始())
            {
                完成();
            }
            return this;
        }
        protected event Func<bool> 更新;
        public 任务 准备更新(){
            if(this.完成了) return this;
            if (更新 != null && 更新())
            {
                完成();
            }
            return this;
        }
        protected 任务 完成()
        {
            完成了 = true;
            准备释放();
            return this;
        }
        protected event Action 释放;
        private void 准备释放()
        {
            if(释放!=null)释放();
            回收();
        }

        private void 回收()
        {
            释放 = null;
            开始 = null;
            更新= null;
            // Delegate[] dels  = 释放.GetInvocationList();
            // foreach (var item in dels)
            // {
            //     释放-= item as Action;
            // }
        }

        

    }
}