using UnityEngine;
/// <summary>
/// 测试 C#6的 async await
/// </summary>
public class TestAwaitStep : MonoBehaviour
{
    Step1 mStep1;
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
    public class Step1:AwaitStep{
        public Step1()
        {
            //OnCompleted(null);
        }
        protected override void OnStart()
        {
            Complete();
        }



    }
}