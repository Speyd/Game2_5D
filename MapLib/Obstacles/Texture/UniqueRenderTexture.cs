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
            // Проверяем, есть ли уже такое значение
            if (valuesSet.Contains(value))
            {
                return false; // Значение уже существует
            }

            // Вставляем в Dictionary и HashSet
            myMap[key] = value; // Если ключ уже существует, он будет обновлён
            valuesSet.Add(value);
            return true;
        }

        public VALUE? GetTexture(KEY key)
        {
            // Используем метод TryGetValue для безопасного доступа к значению
            if (myMap.TryGetValue(key, out VALUE? value))
            {
                return value; // Возвращаем текстуру, если ключ найден
            }

            // Возвращаем null или другое значение по умолчанию, если ключ не найден
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