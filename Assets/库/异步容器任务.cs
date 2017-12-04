using System;
using System.Collections;

namespace 库
{
    /// <summary>
    /// 依赖外部更新驱动
    /// 设计成每一次开始异步,就一个并行的任务;
    /// </summary>
    public class 异步容器任务 : 任务
    {
        public 任务 开始异步(IEnumerator iEnumerator)
        {
             return new 异步组任务(iEnumerator);
            //iEnumerator.MoveNext();
            // return null;
        }
    }
}