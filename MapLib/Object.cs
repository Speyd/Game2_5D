using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib
{
    public class Object
    {
        public int HeightExceedingMiddle { get; init; }
        public int DepthExceedingMiddle { get; init; }
        public char Symbol { get; init; }
        public bool Passability {  get; init; }

        public Object(int height, int depth, char symbol, bool passability)
        {
            HeightExceedingMiddle = height;
            DepthExceedingMiddle = depth;
            Symbol = symbol;
            Passability = passability;
        }

        public static void addUnique(Dictionary<char, MapLib.Object> list, Object objAdd)
        {
            if (list.Count == 0 || !list.ContainsKey(objAdd.Symbol))
                list.Add(objAdd.Symbol, objAdd);
        }
    }
}
