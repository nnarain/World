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

    public bool ContainsKey(TKey key)
    {
        return dict.ContainsKey(key);
    }
}
