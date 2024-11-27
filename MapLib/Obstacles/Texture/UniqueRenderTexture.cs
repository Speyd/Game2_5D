using MapLib.Obstacles.DiversityObstacle;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Obstacles.Texture
{
    public class UniqueDictionary<KEY, VALUE>
    {
        private Dictionary<KEY, VALUE> myMap = new Dictionary<KEY, VALUE>();
        private HashSet<VALUE> valuesSet = new HashSet<VALUE>();

        public UniqueDictionary(List<(KEY, VALUE)> values)
        {
            foreach ((KEY, VALUE) value in values)
            {
                Insert(value.Item1, value.Item2);
            }
        }

        public bool Insert(KEY key, VALUE value)
        {
            if (valuesSet.Contains(value))
            {
                return false;
            }

            myMap[key] = value;
            valuesSet.Add(value);
            return true;
        }

        public VALUE? GetTexture(KEY key)
        {
            if (myMap.TryGetValue(key, out VALUE? value))
            {
                return value;
            }
            return default(VALUE);
        }

        public Dictionary<KEY, VALUE> getUniqueDictionary() => myMap;

        public void Print()
        {
            foreach (var pair in myMap)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
            }
        }
    }
}