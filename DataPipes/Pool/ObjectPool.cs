using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataPipes.Pool;
/// <summary>Objec tPool</summary>
public class ObjectPool<T> where T : IResettable, new()
{
    private readonly ConcurrentBag<T> _pool = new ConcurrentBag<T>();
    private readonly int? _maxCapacity;
    private int _count = 0;

    /// <summary>ObjectPool class constructor</summary>
    /// <param name="maxCapacity">Max amount pool object in list</param>
    public ObjectPool(int maxCapacity = 1500) 
    {
        _maxCapacity = maxCapacity;
    }
    /// <summary>ObjectPool class constructor</summary>
    public ObjectPool()
    {
        _maxCapacity = null;
    }
    /// <summary>Get item from object pool</summary>
    public T Get()
    {
        if (_pool.TryTake(out var item))
        {
            item.Reset();
            Interlocked.Decrement(ref _count);
            return item;
        }

        return new T();
    }
    /// <summary>Return item to object pool</summary>
    public void Return(T item)
    {
        item.Reset();

        if (Interlocked.Increment(ref _count) <= _maxCapacity || _maxCapacity is null)
        {
            _pool.Add(item);
        }
        else
        {
            Interlocked.Decrement(ref _count); 
        }
    }
}
