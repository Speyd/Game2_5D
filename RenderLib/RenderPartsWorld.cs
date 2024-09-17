using EntityLib.Player;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RenderLib.RenderPartsWorld
{
    static public class RenderPartsWorld
    {
        private static RectangleShape? topRect;
        private static RectangleShape? bottomRect;
       // private static double tempA = 0;

        public static void setTopRect(int ScreenWidth, int halfHeight, Color color, Vector2f? vector = null)
        {
            topRect = new RectangleShape(new Vector2f(ScreenWidth, halfHeight));
            topRect.FillColor = color;
            if (vector is not null && vector is Vector2f tempVector)
                topRect.Position = tempVector;
            else
                topRect.Position = new Vector2f(0, 0);
        }
        public static void setBottomRect(int ScreenWidth, int halfHeight, Color color, Vector2f? vector = null)
        {
            bottomRect = new RectangleShape(new Vector2f(ScreenWidth, halfHeight));
            bottomRect.FillColor = color;
            if (vector is not null && vector is Vector2f tempVector)
                bottomRect.Position = tempVector;
            else
                bottomRect.Position = new Vector2f(0, halfHeight);
        }
        public static void renderPartsWorld(Screen screen, Player player)
        {

           if(topRect is not null)
                screen.Window.Draw(topRect);

           if (bottomRect is not null)
                screen.Window.Draw(bottomRect);
        }
    }
}
