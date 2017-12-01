using System;
using System.Collections;

namespace 库
{
    /// <summary>
    /// 依赖外部更新驱动
    /// 设计成每一次开始异步,就一个并行的任务;
    /// </summary>
    public class 异步容器任务:任务
    {
        //public 任务 开始异步(IEnumerator iEnumerator)
        //{
        //    iEnumerator.MoveNext();
        //    return null;
        //}
    }
    public class 任务
    {
        public bool 完成了 { get; private set; }
        protected Func<bool> 开始;
        public 任务 准备开始()
        {
            if (开始!=null && 开始())
            {
                完成();
            }
            return this;
        }


        private 任务 完成()
        {
            完成了 = true;
            准备释放();
            return this;
        }
        private void 准备释放()
        {
            释放();
            回收();
        }

        private void 回收()
        {

        }

        protected void 释放()
        {

        }

    }
}