using System;
using System.Collections.Generic;
public class DictList<KeyType, ValueType>
{
    public class Data
    {
        public KeyType key;
        public ValueType value;
        public int Index;
    }

    private List<Data> mList = new List<Data>();
    private List<KeyType> mKeys = new List<KeyType>();
    private Dictionary<KeyType, Data> mDictionary = new Dictionary<KeyType, Data>();
    public int Count { get { return mList.Count; } }
    public ValueType Add(KeyType key, ValueType value)
    {
        if (mDictionary.ContainsKey(key))
        {
            throw new ArgumentOutOfRangeException("已经存在的数据");
            //return value;
        }
        var data = new Data();
        data.value = value;
        mDictionary.Add(key, data);
        mList.Add(data);
        data.Index = mList.Count - 1;
        mKeys.Add(key);
        return value;
    }
    public void For(Action<ValueType> action)
    {
        for (int i = 0; i < mList.Count; i++)
        {
            action(mList[i].value);
        }
    }
    public void Clear()
    {
        mDictionary.Clear();
        mList.Clear();
        mKeys.Clear();
    }
    public bool ContainsKey(KeyType key)
    {
        return mDictionary.ContainsKey(key);
    }
    public ValueType GetItemAt(int index)
    {
        return mList[index].value;
    }
    public KeyType GetkeyAt(int index)
    {
        return mKeys[index];
    }
    public ValueType GetItem(KeyType key)
    {
        if (!mDictionary.ContainsKey(key))
        {
            UnityEngine.Debug.LogError(key);
        }
        return mDictionary[key].value;
    }
    public ValueType TryGetItem(KeyType key)
    {
        if (!mDictionary.ContainsKey(key)) return default(ValueType);
        return mDictionary[key].value;
    }
    public void RemoveAt(int index)
    {
        var key = GetkeyAt(index);
        Remove(key);
    }
    public ValueType Remove(KeyType key)
    {
        var have = mDictionary.ContainsKey(key);
        if (!have)
        {
            return default(ValueType);
        }
        var data = mDictionary[key];
        mDictionary.Remove(key);
        var index = data.Index;
        var start = index + 1;
        var count = mList.Count;
        for (int i = start; i < count; i++)
        {
            mList[i].Index = i - 1;
        }
        mList.RemoveAt(index);
        mKeys.RemoveAt(index);

        return data.value;
    }
}

