using System;
using UnityEngine;

namespace 库
{
    public class 异步任务 : CustomYieldInstruction
    {
        public 任务 任务 { get; private set; }

        public 异步任务(任务 任务)
        {
            if (任务 == null)
            {
                throw new ArgumentNullException("task");
            }
            this.任务 = 任务;
        }

        public override bool keepWaiting
        {
            get
            {
                 return 任务.完成了;
            }
        }
    }
}