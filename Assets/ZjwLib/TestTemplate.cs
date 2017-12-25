using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 方便测试的东西
/// </summary>
public class TestTemplate : MonoBehaviour
{
    
    public List<TestData> 测试方法;
    // Use this for initialization
    void Start()
    {
        
    }
    public void test1(){
        
    }
    public void 执行静态方法(string str)
    {

        //限制同一个Assembly
        if (string.IsNullOrEmpty(str)) return;


        var dotStrs = str.Split('.');
        var paramStrs = new List<string>();
        var paramStr = "";
        var fnName = "";


        if (dotStrs.Length == 1)
        {
            ZLog.Error("格式不对");
            return;
        }


        {
            var fnStr = dotStrs[dotStrs.Length - 1];
            int index = fnStr.IndexOf('(');
            if (index < 0)
            {
                ZLog.Error("格式不对");
                return;
            }
            int last = fnStr.LastIndexOf(')');
            fnName = fnStr.Substring(0, index).Trim();
            paramStr = fnStr.Substring(index + 1, last - index - 1);
        }
        // 解析方法参数
        {
            var paramsList = paramStr.Split(',');

            for (int i = 0; i < paramsList.Length; i++)
            {
                bool isSucc = true;
                var code = paramsList[i].Trim();
                var param = "";
                var value = "";
                {
                    var arr = code.Split(':');

                    if (arr.Length >= 2)
                    {
                        param = arr[0].Trim();
                        value = arr[1].Trim();
                    }
                    else if (arr.Length == 1)
                    {
                        value = arr[0].Trim();
                    }
                    else
                    {
                        isSucc = false;
                    }
                }
                if (isSucc)
                {
                    paramStrs.Add(value);
                }
                else
                {
                    paramStrs.Add(null);
                }
            }
        }
        {
            var assembly = this.GetType().Assembly;
            string typestr = "";
            for (int i = 0; i < (dotStrs.Length - 1); i++)
            {
                typestr += dotStrs[i];
                if (i != (dotStrs.Length - 2))
                {
                    typestr += ".";
                }
            }
            var type = assembly.GetType(typestr);
            if (type == null)
            {
                ZLog.Error("找不到类: ", typestr);
                return;
            }
            else
            {
                ZLog.Info("找到类: ", typestr);
            }
            //ActorTaskHelp.SetActiveWall
            var methodInfo = type.GetMethod(fnName);
            if (methodInfo == null)
            {
                ZLog.Error("找不到方法: ", fnName);
                return;
            }
            {
                var paramList = methodInfo.GetParameters();
                object[] params1 = new object[paramList.Length];

                for (int i = 0; i < params1.Length; i++)
                {
                    var info = paramList[i];
                    if (info.ParameterType == typeof(bool))
                    {
                        params1[i] = bool.Parse(paramStrs[i]);
                    }
                    else if (info.ParameterType == typeof(string))
                    {
                        params1[i] = paramStrs[i];
                    }
                    else if (info.ParameterType == typeof(int))
                    {
                        params1[i] = int.Parse(paramStrs[i]);
                    }
                    else if (info.ParameterType == typeof(float))
                    {
                        params1[i] = float.Parse(paramStrs[i]);
                    }
                }
                methodInfo.Invoke(null, params1);
            }
        }
    }
    // Update is called once per frame
    public void Update()
    {
        OnUpdate();
        测试方法.ForEach((x)=>{

            if(x.运行){
                x.运行=false;

                if(!string.IsNullOrEmpty(x.字符串参数)){
                        this.GetType().GetMethod(x.方法名).Invoke(this, new []{x.字符串参数 });
                    }else{

                        this.GetType().GetMethod(x.方法名).Invoke(this, null);
                    }
                // try
                // {
                //     if(!string.IsNullOrEmpty(x.字符串参数)){
                //         this.GetType().GetMethod(x.方法名).Invoke(this, new []{x.字符串参数 });
                //     }else{

                //         this.GetType().GetMethod(x.方法名).Invoke(this, null);
                //     }

                // }
                // catch (System.Exception e)
                // {

                //     Debug.Log("err:"+x.方法名+" "+e);
                // }
            }
        });
        
    }

    protected virtual void OnUpdate()
    {
    }

    [System.Serializable]
    public class TestData{
        public string 备注;
        public string 方法名;
        public string 字符串参数;
        public bool  运行;
    }
}
