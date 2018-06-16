using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SafeDictionary<TKey, TValue>
{
    private Dictionary<TKey, TValue> dict;

    private readonly object dictLock = new object();

    public SafeDictionary()
    {
        dict = new Dictionary<TKey, TValue>();
    }

    public void Add(TKey key, TValue value)
    {
        lock(dictLock)
        {
            dict[key] = value;
        }
    }

    public TValue Get(TKey key)
    {
        TValue value = default(TValue);

        lock(dictLock)
        {
            value = dict[key];
        }
        
        return value;
    }

    public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
    {
        return dict.GetEnumerator();
    }

    public bool Remove(TKey key)
    {
        bool ret = false;

        lock(dictLock)
        {
            ret = dict.Remove(key);
        }

        return ret;
    }

    public bool ContainsKey(TKey key)
    {
        var ret = false;

        lock(dictLock)
        {
            ret = dict.ContainsKey(key);
        }

        return ret;
    }
}
