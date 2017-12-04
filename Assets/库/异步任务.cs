using System;
using System.Collections;
using UnityEngine;

namespace 库
{
    public class 异步组任务 : 任务
    {
        private readonly IEnumerator mIEnumerator;

        public 异步组任务(IEnumerator iEnumerator)
        {
            this.mIEnumerator = iEnumerator;
            this.开始 += 异步更新;
            this.更新 += 异步更新;
        }

        private bool 异步更新()
        {
            if(mIEnumerator.MoveNext())return true;
            return false;
        }
    }
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