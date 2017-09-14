using System.Collections.Generic;
public class DictList<KeyType, ValueType>
{
    private List<ValueType> mList = new List<ValueType>();
    private Dictionary<KeyType, ValueType> mDictionary = new Dictionary<KeyType, ValueType>();
    public int Count { get { return mList.Count; } }
    public ValueType Add(KeyType key, ValueType value)
    {
        if (mDictionary.ContainsKey(key)) return value;
        mDictionary.Add(key, value);
        mList.Add(value);
        return value;
    }
    public void Clear()
    {
        mDictionary.Clear();
        mList.Clear();
    }
    public ValueType GetItemAt(int index)
    {
        return mList[index];
    }
    public ValueType Remove(KeyType key)
    {
        ValueType value;
        var have = mDictionary.TryGetValue(key, out value);
        if (!have) return value;
        mList.Remove(value);
        return value;
    }
}

