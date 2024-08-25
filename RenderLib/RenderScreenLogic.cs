using ScreenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLib;
using RenderLib.InfoRender;
using System.Drawing;
using System.Runtime.CompilerServices;
using MapLib;
using System.Data;

namespace RenderLib.LogicRender
{
    internal class RenderScreenLogic(Screen screen, Entity entity, Map map)
    {
        

        private MapLib.Object definitionBlockChar(ref MapLib.Object? obj, ref bool isBound, double distanceToWall)
        {
            char tempChar = ' ';

            if (obj is null)
               obj = new MapLib.Object(0, 0, ' ', true);

            if (isBound == true)
                return new MapLib.Object(obj.HeightExceedingMiddle, obj.DepthExceedingMiddle, '|', obj.Passability);

            foreach (var symbolValue in ListSymbolRender.symbols)
            {
                if (distanceToWall < entity.Depth / symbolValue.Key)
                {
                    tempChar = symbolValue.Value;
                    break;
                }
            }

            return new MapLib.Object(obj.HeightExceedingMiddle, obj.DepthExceedingMiddle, tempChar, obj.Passability);
        }

        private void fillingScreenWithSymbol(int x, MapLib.Object blockChar, int ceiling, int floor)
        {
            int blockTop = ceiling - blockChar.HeightExceedingMiddle;
            int blockBottom = floor + blockChar.DepthExceedingMiddle;

            for (int y = 0; y < screen.ScreenHeight; y++)
            {
                if (y < blockTop)
                {
                    screen.ScreenChr[y * screen.ScreenWidth + x] = SymbolsFillingSurfaces.symbols[TypesOfSurfaces.CEILING];
                }
                else if (y >= blockTop && y <= blockBottom)
                {
                    screen.ScreenChr[y * screen.ScreenWidth + x] = blockChar.Symbol;
                }
                else
                {
                    screen.ScreenChr[y * screen.ScreenWidth + x] = SymbolsFillingSurfaces.symbols[TypesOfSurfaces.FLOOR];
                }
            }
        }

        public void render(ref MapLib.Object? obj, int x, ref bool isBound, ref double distanceToWall)
        {
            int ceiling = (int)(screen.ScreenHeight / 2d - screen.ScreenHeight * entity.EntityFov / distanceToWall);
            int floor = screen.ScreenHeight - ceiling;

            MapLib.Object blockChar = definitionBlockChar(ref obj, ref isBound, distanceToWall);
            fillingScreenWithSymbol(x, blockChar, ceiling, floor);
        }
    }
}
