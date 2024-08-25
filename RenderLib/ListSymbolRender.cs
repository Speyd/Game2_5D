using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderLib.InfoRender
{
    internal struct ListSymbolRender
    {
        public static readonly Dictionary<double, char> symbols = new Dictionary<double, char> 
        {
            { 4d, '\u2588' },
            { 3d, '\u2593' },
            { 2d, '\u2592' },
            { 1d, '\u2591' },    
        };
    }
}
