using System;
using System.Collections.Generic;
using UnityEngine;
public class UpdateDriver : IUpdate
{
    private List<Action> mNeedUpdateActions;
    int i;
    public bool Update()
    {
        for (i = 0; i < mNeedUpdateActions.Count; i++)
        {
            mNeedUpdateActions[i]();
        }
        return false;
    }
    public void AddUpdate(Func<bool> UpdateFunc)
    {
        bool end = false;
        Action needAddAction = null;
        needAddAction = () =>
         {
             if (end) return;
             if (UpdateFunc())
             {
                 end = true;
                 mNeedUpdateActions.Remove(needAddAction);
             }
         };
        if(mNeedUpdateActions==null) mNeedUpdateActions = new List<Action>();
        mNeedUpdateActions.Add(needAddAction);
    }
}
public class UpdateMono : MonoBehaviour
{
    void Awake (){
        // DontDestroyOnLoad(gameObject);
    }
    private readonly List<Action> mNeedUpdateActions = new List<Action>();
    int i;
    void Update (){
          for (i = 0; i < mNeedUpdateActions.Count; i++)
          {
              mNeedUpdateActions[i]();
          }  
    }
    public void Clear(){
        mNeedUpdateActions.Clear();
    }
    public void AddUpdate(Func<bool> UpdateFunc)
    {
        bool end = false;
        Action needAddAction = null;
        needAddAction = () =>
         {
             if (end) return;
             if (UpdateFunc())
             {
                 end = true;
                 mNeedUpdateActions.Remove(needAddAction);
             }
         };
        mNeedUpdateActions.Add(needAddAction);
    }

}