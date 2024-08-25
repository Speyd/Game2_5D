using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderLib.InfoRender
{
    internal struct SymbolsFillingSurfaces
    {
        public static readonly Dictionary<TypesOfSurfaces, char> symbols = new Dictionary<TypesOfSurfaces, char>
        {
            { TypesOfSurfaces.FLOOR, '.' },
            { TypesOfSurfaces.CEILING, ' ' },
        };
    }
}
