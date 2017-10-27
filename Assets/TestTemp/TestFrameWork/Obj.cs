using System;
using System.Reflection;
using UnityEngine;

namespace TestTemp.TestFrameWork
{
    public class Obj : IObj
    {
        [Inject]
        public IObj Need{get;private set;}
        private IObj mContainer;
        private static Type injectType = typeof(InjectAttribute);
        public Obj(IObj container=null)
        {
            this.mContainer = container;
            var t  = this.GetType();
            var  props = t.GetProperties();
            Debug.Log("count1 "+props.Length);

            for (int i = 0; i < props.Length; i++)
            {
                var item  = props[i];
                var att =  item.GetCustomAttribute(injectType);
                if(att==null)continue;
                Debug.Log("name "+ item.Name);
                Debug.Log("PropertyType "+ item.PropertyType);
                Debug.Log("item "+ item);
                
            }
            var attributes =  this.GetType().GetCustomAttributes(typeof(InjectAttribute),true);
            Debug.Log("count2 " + attributes.Length);
            
            //Debug.Log(attributes[0].GetType().Name);
            
        }
    }
    public interface IObj{

    }
    
    [System.AttributeUsage(System.AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    sealed class InjectAttribute : System.Attribute
    {
        // See the attribute guidelines at
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        
        // This is a positional argument
        public InjectAttribute()
        {

        }
        // This is a named argument
        public int NamedInt { get; set; }
    }
    public class ObjTest{
        public ObjTest()
        {
            new Obj();
            //new Obj(new Obj());
        }
    }
    
}
