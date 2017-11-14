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

    // Update is called once per frame
    public void Update()
    {
        测试方法.ForEach((x)=>{

            if(x.运行){
                x.运行=false;
                try
                {
                    if(!string.IsNullOrEmpty(x.字符串参数)){
                        this.GetType().GetMethod(x.方法名).Invoke(this, new []{x.字符串参数 });
                    }else{

                        this.GetType().GetMethod(x.方法名).Invoke(this, null);
                    }

                }
                catch (System.Exception e)
                {

                    Debug.Log("err:"+x.方法名+" "+e);
                }
            }
        });
        
    }
    [System.Serializable]
    public class TestData{
        public string 备注;
        public string 方法名;
        public string 字符串参数;
        public bool  运行;
    }
}
