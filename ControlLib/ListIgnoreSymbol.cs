using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapLib;

namespace ControlLib
{
    internal struct ListIgnoreSymbol(Map map)
    {
        public readonly List<char> ignoreSymbol = new List<char>() 
        {
            {map.empty.Symbol },
            {map.lineSight.Symbol },
        };

    }
}
