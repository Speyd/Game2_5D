using System.Collections.Concurrent;


namespace DataPipes.Pool;
/// <summary>List Pool</summary>
public class ListPool<T>
{
    private readonly ConcurrentBag<List<T>> _pool = new ConcurrentBag<List<T>>();
    /// <summary>Get item from list</summary>
    public List<T> Get()
    {
        if (_pool.TryTake(out var list))
        {
            list.Clear();
            return list;
        }

        return new List<T>();
    }
    /// <summary>Return item to list</summary>
    public void Return(List<T> list)
    {
        list.Clear();
        _pool.Add(list);
    }
}
