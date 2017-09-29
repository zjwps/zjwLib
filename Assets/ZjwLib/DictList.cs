using System;
using System.Collections.Generic;
public class DictList<KeyType, ValueType>
{
    private List<ValueType> mList = new List<ValueType>();
    private List<KeyType> mKeys = new List<KeyType>();
    private Dictionary<KeyType, ValueType> mDictionary = new Dictionary<KeyType, ValueType>();
    public int Count { get { return mList.Count; } }
    public ValueType Add(KeyType key, ValueType value)
    {
        if (mDictionary.ContainsKey(key)) return value;
        mDictionary.Add(key, value);
        mList.Add(value);
        mKeys.Add(key);
        return value;
    }
    public void For(Action<ValueType> action)
    {
        for (int i = 0; i < mList.Count; i++)
        {
            action(mList[i]);
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
        return mList[index];
    }
    public KeyType GetkeyAt(int index)
    {
        return mKeys[index];
    }
    public ValueType GetItem(KeyType key)
    {
        return mDictionary[key];
    }
    public ValueType TryGetItem(KeyType key)
    {
        if (!mDictionary.ContainsKey(key)) return default(ValueType);
        return mDictionary[key];
    }
    public ValueType Remove(KeyType key)
    {
        ValueType value;
        var have = mDictionary.TryGetValue(key, out value);
        if (!have)
        {
            return value;
        }
        mDictionary.Remove(key);
        mList.Remove(value);
        mKeys.Remove(key);
        return value;
    }
}

