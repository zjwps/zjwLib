using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 对象引用代码生成
/// </summary>
public class CodeHelper
{
    const string tag = "CareUI";
    // private const string preMenu = "ZjwTest/ui工具";
    // private static GameObject SelectedGameObject;
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
            // ,typeof(UIToggle)
            // ,typeof(UILoopTitleGrid)
            //,
            typeof(Transform)
        };
        //这样写如果是不同程序集会有问题
        // types = new List<Type>()
        // {
        //     Type.GetType("UIWindow")
        //     ,Type.GetType("RewardShowView")
        //     ,Type.GetType("UI2DSprite")
        //     ,Type.GetType("UILabel")
        //     ,Type.GetType("UIGrid")
        //     ,Type.GetType("UIToggle")
        //     ,Type.GetType("UILoopTitleGrid")
        //     ,typeof(Transform)
        // };
        for (int i = 0; i < types.Count; i++)
        {
            Debug.Log(types[i]);

        }
    }
    [InitializeOnLoadMethod]
    static void StartInitializeOnLoadMethod()
    {
        InitTypes();
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }
    //[MenuItem(preMenu + "/NGUI代码生成")]
    static void Test2(GameObject SelectedGameObject)
    {
        //print("Test2");
        var t = SelectedGameObject;
        if (t == null) return;
        var tool = new 对象引用代码生成(t);
        Debug.Log(tool.Result);
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
                if (t.tag != tag) return;
                //print(t.name);
                //EditorUtility.DisplayPopupMenu(new Rect(mousePosition.x, mousePosition.y, 0, 0), preMenu, null);
                var option = EditorUtility.DisplayDialog(
                "NGUI代码生成?"
                , "生成Objs代码并且拷贝到剪贴板"
                , "确定"
                , "取消"
                // ,"Quit without saving"
                );
                if (!option) return;
                // SelectedGameObject = t;
                Event.current.Use();
                Test2(t);
            }
        }

    }
    public class 对象引用代码生成
    {
        public string ClassStringBefore =
            "   public class Objs\\n" +
            "   {\n" +
            "       public GameObject MainObj;\n" +
            "       public Transform transform;\n" +
            "";
        public string ClassStringAfter = "" +
            "       }\n" +
            "   }";
        public string CtorStringBefore = "" +
            "       public Objs(GameObject t)\n" +
            "       {\n" +
            "           MainObj = t;\n" +
            "           transform = t.transform;\n" +
            "";
        public string LabelString =
            "           ";

        public string Result = "";
        public string 声明字段 = "" +
            "";
        public string 显示对象获取代码 = "";
        public string t = "" +
            "       public GameObject MainObj;\n" +
            "" +
            "";
        public GameObject MainObj;

        public 对象引用代码生成(GameObject t)
        {
            this.MainObj = t;
            if (MainObj == null) return;
            生成文本();
            组合文本();
        }
        private void 生成文本()
        {
            var t = MainObj.transform;
            找到关心对象(t, 生成对象的代码);
        }
        public void 找到关心对象(Transform tran, Func<Transform, string, bool> func, string currPath = "")
        {
            var goOn = func(tran, currPath);
            var path = currPath;
            if (tran.childCount >= 0 && goOn)
            {
                for (int i = 0; i < tran.childCount; i++)
                {
                    Transform child = tran.GetChild(i);
                    找到关心对象(child, func, path == "" ? child.name : path + "/" + child.name);
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
            if (t.tag != tag) return goOn;
            for (int i = 0; i < types.Count; i++)
            {
                if (Findcomment(types[i], t, CommentWriteCode, currPath)) return goOn;
            }
            return false;

        }
        private bool Findcomment(Type type, Transform tran,
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
        private void CommentWriteCode(string typeName, string name, string currPath)
        {
            var GetComponentStr = ".GetComponent<{0}>()";
            if (typeName == "Transform") GetComponentStr = "";
            var t = "       public {0} {1};\n";
            t = string.Format(t, typeName, name);
            声明字段 += t;

            if (currPath == "") t = "           {1} = transform" + GetComponentStr + ";\n";
            else t = "           {1} = transform.Find(\"{2}\")" + GetComponentStr + ";\n";
            t = string.Format(t, typeName, name, currPath);
            显示对象获取代码 += t;
            //Transform transform=null;
            //transform.Find("");

        }
        private bool Findcomment<T>(Transform tran, Action<string, string, string> action, string currPath = "")
        {
            var path = currPath;
            var comment = tran.GetComponent<T>();
            if (comment != null)
            {
                action(typeof(T).Name, tran.name, path);
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
