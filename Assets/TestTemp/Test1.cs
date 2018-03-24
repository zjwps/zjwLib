using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using zjw.Tools.WaitStep;
namespace Zjwlib{
	public class Entity{
		public int Id;
		public Entity Parent { get; set; }
	}
	public class Component{

	}
	public class Service{
		/*
		服务考虑外部代码添加一个服务适配器
		 */
	}
	public class Context{

	}
    public class UpdateService : MonoBehaviour,IService
    {
		
        /*update服务*/
        public void Service(IServiceTarget target)
        {
			(target as IUpdate).Update();
        }
		void Update (){
			
		}
    }
    public interface IService{
		void Service(IServiceTarget target);
	}
	public interface IServiceTarget{

	}
	public interface IUpdate:IServiceTarget{
		void Update();
	}
}

namespace zjwTest1{
	public class TestILruntime:MonoBehaviour{
		IEnumerator Start()
        {
            var path = Application.streamingAssetsPath;
            path = Path.Combine(path, "HotFix_Project.dll");
            byte[] dll;
            using (var www = UnityWebRequest.Get(path))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                    yield break;
                }
                else
                {
                    // Show results as text
                    //Debug.Log(www.downloadHandler.text);

                    // Or retrieve results as binary data
                    byte[] results = www.downloadHandler.data;
                    Debug.Log(results.Length);
                    dll = results;
                }
            }
            //虚拟机
            // var appdomain = new AppDomain();
            // using (var ms = new MemoryStream(dll))
            // {
            //     appdomain.LoadAssembly(ms);
            // }
            // // HelloWorld，第一次方法调用
            // appdomain.Invoke("HotFix_Project.InstanceClass", "StaticFunTest", null, null);
        }
	}
	public class Container{
		
		protected readonly List<Component> ChildMap = new List<Component>();
		public void AddComponent<T>()where T:Component
        {
            //type.Assembly.CreateInstance(type.FullName) as T;
            // ChildMap.Add(component);
			// var type = typeof(T);
            // var component =  type.Assembly.CreateInstance(type.FullName) as Component;
			// //BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            // BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            // //MethodInfo method = type.GetMethod(name, flag);
            // //return (T)method.Invoke(instance, param);
			// type.GetMethod("Start",flag).Invoke(component,null);

			var type = typeof(T);
            var component = type.Assembly.CreateInstance(type.FullName);
            type.GetMethod("Start").Invoke(component, null);
		}
	}
	public class Component{
		private Component(){
			
		}
		public void Start(){
			Debug.Log("Component start");
			
		}
	}
}
public class Test1 : MonoBehaviour {
    private UpdateDriver mUpdateDriver;
    private StepProcess stepTask;

    // Use this for initialization
    void Start () {
		mUpdateDriver = new UpdateDriver();
        mUpdateDriver.AddUpdate(stepTask.Update);
        stepTask = new StepProcess();
		 
		 stepTask.StartStep(Fn1());
	}
    void TestA1(){
        var map = new HashSet<Zjwlib.Entity>();
        map.Add(new Zjwlib.Entity());
        // map.UnionWith(new HashSet<int>(){2,3,1});
        var t1 = new LinkedList<int>();
        var t2 = new Hashtable();
        
    }

    private IEnumerator<Step> Fn1()
    {
		Debug.Log("startTime:"+Time.time);
		yield return StepPool.NewWaitTime(0.5f);
		Debug.Log("Time1:"+Time.time);
		yield return StepPool.NewWaitFrame(2);
		Debug.Log("Time1:"+Time.time);
    }
	private IEnumerator<Step>Fn2(){
		yield return StepPool.NewWaitFrame();
	}

    // Update is called once per frame
    void Update () {
		mUpdateDriver.Update();
	}
}
