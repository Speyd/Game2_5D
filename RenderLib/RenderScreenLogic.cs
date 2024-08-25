using ScreenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLib;
using RenderLib.InfoRender;
using System.Drawing;

namespace RenderLib.LogicRender
{
    internal class RenderScreenLogic(Screen screen, Entity entity)
    {
        private char definitionBlockChar(double distanceToWall)
        {
            char tempChar = ' ';

            foreach (var symbolValue in ListSymbolRender.symbols)
            {
                if (distanceToWall < entity.Depth / symbolValue.Key)
                {
                    tempChar = symbolValue.Value;
                    break;
                }
                else
                    tempChar = ' ';
            }

            return tempChar;
        }

        private void fillingScreenWithSymbol(int x, char blockChar, int ceiling, int floor)
        {
            for (int y = 0; y < screen.ScreenHeight; y++)
            {
                if (y <= ceiling)
                {
                    screen.ScreenChr[y * screen.ScreenWidth + x] = SymbolsFillingSurfaces.symbols[TypesOfSurfaces.CEILING];
                }
                else if (y > ceiling && y <= floor)
                {
                    screen.ScreenChr[y * screen.ScreenWidth + x] = blockChar;
                }
                else
                {
                    screen.ScreenChr[y * screen.ScreenWidth + x] = SymbolsFillingSurfaces.symbols[TypesOfSurfaces.FLOOR];
                }
            }
        }

        public void render(int x, ref double distanceToWall)
        {
            int ceiling = (int)(screen.ScreenHeight / 2d - screen.ScreenHeight / distanceToWall);
            int floor = screen.ScreenHeight - ceiling;

            char blockChar = definitionBlockChar(distanceToWall);
            fillingScreenWithSymbol(x, blockChar, ceiling, floor);
        }
    }
}
