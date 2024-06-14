using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BiDirectionalDictionary<T1, T2>
{
    private readonly Dictionary<T1, T2> forward = new Dictionary<T1, T2>();
    private readonly Dictionary<T2, T1> backward = new Dictionary<T2, T1>();

    public void Add(T1 key, T2 value)
    {
        if (forward.ContainsKey(key))
        {
            backward.Remove(forward[key]);
        }
        if (backward.ContainsKey(value))
        {
            forward.Remove(backward[value]);
        }

        forward[key] = value;
        backward[value] = key;
    }

    public T2 GetByKey(T1 key)
    {
        return forward.ContainsKey(key) ? forward[key] : default(T2);
    }

    public T1 GetByValue(T2 value)
    {
        return backward.ContainsKey(value) ? backward[value] : default(T1);
    }

    public bool TryGetByKey(T1 key, out T2 value)
    {
        return forward.TryGetValue(key, out value);
    }

    public bool TryGetByValue(T2 value, out T1 key)
    {
        return backward.TryGetValue(value, out key);
    }

    public void RemoveByKey(T1 key)
    {
        if (forward.ContainsKey(key))
        {
            var value = forward[key];
            forward.Remove(key);
            backward.Remove(value);
        }
    }

    public void RemoveByValue(T2 value)
    {
        if (backward.ContainsKey(value))
        {
            var key = backward[value];
            backward.Remove(value);
            forward.Remove(key);
        }
    }

    public int Count => forward.Count;

    public void Clear()
    {
        forward.Clear();
        backward.Clear();
    }

    public IEnumerable<T1> Keys => forward.Keys;
    public IEnumerable<T2> Values => backward.Keys;
}
