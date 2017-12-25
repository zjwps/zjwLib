using System;
using System.Collections.Generic;
using UnityEngine;

public class ZLog
{
    const string DefaultTag = "Default";
    private Dictionary<string, bool> mEnableMap = new Dictionary<string, bool>();
    public string[] Tags { get; private set;}
    //info用LogErr来显示.
    public static bool InfoShowErr= false;
    private static ZLog mInstance;
    private static ZLog Instance {
        get
        {
            if (mInstance == null) mInstance = new ZLog();
            return mInstance;
        }
    }
    private ZLog()
    {
        mEnableMap.Add(DefaultTag, true);
        Tags = new string[] { DefaultTag };
    }
    public static void Assert(bool condition)
    {
        Debug.Assert(condition);
    }
    public static void Error(params object[] param)
    {
        Instance.ErrorIn(DefaultTag, param);
    }
    public static void ErrorTag(string tag = DefaultTag, params object[] param)
    {
        Instance.ErrorIn(tag,param);
    }
    public static void TagError(string tag = DefaultTag, params object[] param)
    {
        Instance.ErrorIn(tag, param);
    }
    private void ErrorIn(string tag = DefaultTag, params object[] param)
    {
        if (!mEnableMap.ContainsKey(tag)) return;
        Run(Error, param);
    }
    public static void Info(params object[] param)
    {
        if (!Instance.mEnableMap.ContainsKey(DefaultTag)) return;
        Instance.Run(Instance.Info, param);
    }
        
    public static void InfoTag(string tag = DefaultTag,  params object[] param)
    {
        if (!Instance.mEnableMap.ContainsKey(tag)) return;
        Instance.Run(Instance.Info, param);
    }
    public static void TagInfo(string tag = DefaultTag, params object[] param)
    {
        InfoTag(tag, param);
    }
    public static void InfoFormat(string format, params object[] args)
    {
        InfoFormatTag(DefaultTag,format, args);
        //Run(InfoFormat, param);
    }
    public static void InfoFormatTag(string tag, string format, params object[] args)
    {
        if (!Instance.mEnableMap.ContainsKey(tag)) return;
        Instance.InfoFormatIn(format, args);
        //Run(InfoFormat, param);
    }
    public static void SetEnableTag(params string[] tags)
    {
        Instance.mEnableMap.Clear();
        for (int i = 0; i < tags.Length; i++)
        {
            Instance.mEnableMap.Add(tags[i], true);
        }
        Instance.Tags = tags;
    }
    private void Run(Action<string> action, params object[] param)
    {
        var str = "";
        for (int i = 0; i < param.Length; i++)
        {
            str += " " + param[i].ToString();
        }
        action(str);
    }
    private void Info(string str)
    {
        if (InfoShowErr) Debug.LogError(str);
        Debug.Log(str);
    }
    private void InfoFormatIn(string str,params object[] args)
    {
        Debug.LogFormat(str, args);
    }
    private void Error(string str)
    {
        Debug.LogError(str);
    }
}
