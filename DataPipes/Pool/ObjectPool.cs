using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPipes.Pool
{
    public class ObjectPool<T> where T : IResettable, new()
    {
        private ConcurrentBag<T> _pool = new ConcurrentBag<T>();

        public T Get()
        {
            if (_pool.TryTake(out var item))
            {
                item.Reset();
                return item;
            }

            return new T();
        }

        public void Return(T item)
        {
            item.Reset();
            _pool.Add(item);
        }

    }
}
