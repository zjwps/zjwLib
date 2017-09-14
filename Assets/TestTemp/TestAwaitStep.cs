#if NET_4_6
using UnityEngine;
/// <summary>
/// 测试 C#6的 async await
/// </summary>
public class TestAwaitStep : MonoBehaviour
{
    void Start ()
    {
        Test2();
        
    }
    async void Test2()
    {
        await new Step1();
        await new Step1();
        Debug.Log("Test2 wait over");
    }
    async void Test3(){
        await new Step1();
    }
    public class Step1: AwaitStep
    {
        public Step1()
        {
            
        }
        protected override void OnStart()
        {
            Complete();
        }

    }
}
#endif