using UnityEngine;
/// <summary>
/// 测试 C#6的 async await
/// </summary>
public class TestAwaitStep : MonoBehaviour
{
    Step1 mStep1;
    async void Start (){
        mStep1 = new Step1();
        Debug.Log("wait start");
        await Tesetfn1();
        Debug.Log("wait over");
    }

    private int count = 2;
    private void Update()
    {
        if (mStep1 != null)
        {
            if (count > 0)
            {
                Debug.Log(count);
                count--;
                return;
            }

            mStep1.Complete();
            mStep1 = null;
        }
    }
    AwaitStep Tesetfn1(){
        return mStep1;
    }
    public class Step1:AwaitStep{
        public Step1()
        {
            //OnCompleted(null);
        }

        
    }
}