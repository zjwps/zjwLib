using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestUiHelper : MonoBehaviour {
    private const string preMenu = "ZjwTest/ui工具";
    private static GameObject SelectedGameObject;
    private static List<Type> types;
    public static void InitTypes()
    {
        types = new List<Type>()
        {
            // typeof(UIWindow)
            // ,typeof(RewardShowView)
            // ,typeof(UI2DSprite)
            // ,typeof(UILabel)
            // ,typeof(UIGrid)
            // ,typeof(Transform)
        };
    }
    [InitializeOnLoadMethod]
    static void StartInitializeOnLoadMethod()
    {
        InitTypes();
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }
    [MenuItem(preMenu + "/NGUI代码生成")]
    static void Test2()
    {
        //print("Test2");
        var t = SelectedGameObject;
        var tool = new NGUI代码生成(t);
        print(tool.Result);
        CopyToClipBoard(tool.Result);
    }
    static void CopyToClipBoard(string str)
    {
        TextEditor te = new TextEditor();
        //te.content = new GUIContent(str);
        te.text = str;
        te.SelectAll();
        te.Copy();
    }
    private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        if (Event.current != null && selectionRect.Contains(Event.current.mousePosition)
             && Event.current.button == 1 && Event.current.type <= EventType.mouseUp)
        {
            GameObject t = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (t)
            {
                Vector2 mousePosition = Event.current.mousePosition;
                if (t.tag != "CareUI") return;
                //print(t.name);
                EditorUtility.DisplayPopupMenu(new Rect(mousePosition.x, mousePosition.y, 0, 0), preMenu, null);
                SelectedGameObject = t;
                Event.current.Use();
            }
        }
       
    }
    public class NGUI代码生成
    {
        public string ClassStringBefore =
            "   public class UiObjs\n" +
            "   {\n" +
            "       public GameObject MainObj;\n" +
            "       public Transform transform;\n" +
            "";
        public string ClassStringAfter ="" +
            "       }\n" +
            "   }";
        public string CtorStringBefore = "" +
            "       public UiObjs(GameObject t)\n" +
            "       {\n" +
            "           MainObj = t;\n" +
            "           transform = t.transform;\n" +
            "";
        public string LabelString = 
            "           ";

        public string Result="";
        public string 声明字段 = "" +
            "";
        public string 显示对象获取代码 = "";
        public string t = "" +
            "       public GameObject MainObj;\n" +
            "" +
            "";
        public GameObject MainObj;
     
        public NGUI代码生成(GameObject t)
        {
            this.MainObj = t;

            生成文本();
            组合文本();
        }
        private void 生成文本()
        {
            var t = MainObj.transform;
           找到关心对象(t, 生成对象的代码);
        }
        public void 找到关心对象(Transform tran, Func<Transform,string,bool> func, string currPath = "")
        {
            var goOn = func(tran, currPath);
            var path = currPath;
            if (tran.childCount >= 0 && goOn)
            {
                for (int i = 0; i < tran.childCount; i++)
                {
                    Transform child = tran.GetChild(i);
                    找到关心对象(child, func, path==""?child.name:path+"/"+child.name);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns>是否继续</returns>
        private bool 生成对象的代码(Transform t, string currPath = "")
        {
            var goOn = t.gameObject.activeSelf;
            if (t.tag != "CareUI") return goOn;
            for (int i = 0; i < types.Count; i++)
            {
                if (Findcomment(types[i],t, CommentWriteCode, currPath)) return goOn;
            }
            return false;

        }
        private bool Findcomment(Type type,Transform tran,
            Action<string, string, string> action, string currPath = "")
        {
            var path = currPath;
            var comment = tran.GetComponent(type);
            if (comment != null)
            {
                action(type.Name, tran.name, path);
                return true;
            }
            return false;

        }
        private void CommentWriteCode(string typeName,string name,string currPath)
        {
            var t = "       public {0} {1};\n";
            t = string.Format(t,typeName, name);
            声明字段 += t;
            
            if(currPath=="")t = "           {1} = transform.GetComponent<{0}>();\n";
            else t = "           {1} = transform.Find(\"{2}\").GetComponent<{0}>();\n";
            t = string.Format(t, typeName, name,currPath);
            显示对象获取代码 += t;
            //Transform transform=null;
            //transform.Find("");

        }
        private bool Findcomment<T>(Transform tran,Action<string,string,string> action, string currPath = "")
        {
            var path = currPath;
            var comment = tran.GetComponent<T>();
            if (comment != null)
            {
                action(typeof(T).Name, tran.name,path);
                return true;
            }
            return false;

        }
        
        private void 组合文本()
        {
            Result += ClassStringBefore;

            Result += 声明字段;
            Result += CtorStringBefore;
            Result += 显示对象获取代码;

            Result += ClassStringAfter;
        }
    }
   

}
