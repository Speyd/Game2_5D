using System.Collections.Concurrent;

namespace DataPipes.Pool
{
    public class ListPool<T>
    {
        private readonly ConcurrentBag<List<T>> _pool = new ConcurrentBag<List<T>>();
        public List<T> Get()
        {
            if (_pool.TryTake(out var list))
            {
                list.Clear();
                return list;
            }

            return new List<T>();
        }

        public void Return(List<T> list)
        {
            list.Clear();
            _pool.Add(list);
        }
    }
}
