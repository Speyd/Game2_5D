using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataPipes.Dictionary;
/// <summary> Unique Dictionary</summary>
public class UniqueDictionary<KEY, VALUE>
    where KEY : notnull
    where VALUE : notnull
{
    private Dictionary<KEY, VALUE> myMap = new Dictionary<KEY, VALUE>();
    private Dictionary<VALUE, bool> valuePresence = new Dictionary<VALUE, bool>();
    /// <summary>Count item in dictionary</summary>
    public int Count { get { return myMap.Count; } }
    /// <summary>UniqueDictionary class constructor</summary> 
    public UniqueDictionary()
    { }
    /// <summary>ObjectPool class constructor</summary>
    /// <param name="values">UniqueDictionary class object</param>
    public UniqueDictionary(List<(KEY, VALUE)> values)
    {
        foreach ((KEY, VALUE) value in values)
        {
            Insert(value.Item1, value.Item2);
        }
    }

    /// <summary>ObjectPool class constructor</summary>
    /// <param name="key">Key in cortege</param>
    /// <param name="value">Value in cortege</param>
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
    /// <summary>Get value from dictionary</summary>
    /// <param name="key">Key in cortege</param>
    public VALUE? GetValue(KEY key)
    {
        if (myMap.TryGetValue(key, out VALUE? value))
        {
            return value;
        }
        return default;
    }
    /// <summary>Get all values from dictionary</summary>
    public List<VALUE> GetValues()
    {
        List <VALUE> values = new();
        foreach(VALUE value in myMap.Values)
            values.Add(value);

        return values;
    }
    /// <summary>Get keys from dictionary</summary>
    public List<KEY> GetAllKey()
    {      
        return myMap.Keys.ToList();
    }
    /// <summary>Get first value from dictionary</summary>
    public VALUE? GetFirstValue()
    {
        if (Count == 0)
            return default;

        return myMap.First().Value;
    }
    /// <summary>Get value from dictionary</summary>
    /// <param name="key">Key in cortege</param>
    /// <returns>
    /// <c>true</c> if the dictionary contains an element with the specified key; otherwise, <c>false</c>.
    /// </returns>
    public bool ContainsKey(KEY key)
    {
        return myMap.ContainsKey(key);
    }
    /// <summary>Clear Map</summary>
    public void Clear()
    {
        myMap.Clear();
    }

    /// <summary>Get dictionary</summary>
    public Dictionary<KEY, VALUE> GetUniqueDictionary() => myMap;
    /// <summary>Print dictionary</summary>
    public void Print()
    {
        foreach (var pair in myMap)
        {
            Console.WriteLine($"{pair.Key}: {pair.Value}");
        }
    }
    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose value to get.</param>
    /// <returns>
    /// The value associated with the specified key, or the default value of <typeparamref name="VALUE"/> if the key is not found.
    /// </returns>
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