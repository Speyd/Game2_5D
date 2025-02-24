using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataPipes.Dictionary;
public class UniqueDictionary<KEY, VALUE>
    where KEY : notnull
    where VALUE : notnull
{
    private Dictionary<KEY, VALUE> myMap = new Dictionary<KEY, VALUE>();
    private Dictionary<VALUE, bool> valuePresence = new Dictionary<VALUE, bool>();
    public int Count { get { return myMap.Count; } }

    public UniqueDictionary()
    { }
    public UniqueDictionary(List<(KEY, VALUE)> values)
    {
        foreach ((KEY, VALUE) value in values)
        {
            Insert(value.Item1, value.Item2);
        }
    }

    public bool Insert(KEY key, VALUE value)
    {
        if (valuePresence.ContainsKey(value))
        {
            return false;
        }

        myMap[key] = value;
        valuePresence[value] = true;
        return true;
    }

    public VALUE? GetValue(KEY key)
    {
        if (myMap.TryGetValue(key, out VALUE? value))
        {
            return value;
        }
        return default;
    }
    public List<VALUE> GetValues()
    {
        List <VALUE> values = new();
        foreach(VALUE value in myMap.Values)
            values.Add(value);

        return values;
    }
    public List<KEY> GetAllKey()
    {      
        return myMap.Keys.ToList();
    }
    public VALUE? GetFirstValue()
    {
        if (Count == 0)
            return default;

        return myMap.First().Value;
    }
    public bool PresenceKey(KEY key)
    {
        return myMap.ContainsKey(key);
    }

    public Dictionary<KEY, VALUE> GetUniqueDictionary() => myMap;

    public void Print()
    {
        foreach (var pair in myMap)
        {
            Console.WriteLine($"{pair.Key}: {pair.Value}");
        }
    }

    public VALUE? this[KEY key]
    {
        get
        {
            if (myMap.TryGetValue(key, out VALUE? value))
            {
                return value;
            }
            return default;
        }
    }
}