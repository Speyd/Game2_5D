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

        private static void calculatingCoordinatesY(Screen screen, Player player, ref int firstNum, ref int secondNum)
        {
            firstNum = (int)(screen.ScreenHeight * (1 - Math.Sin(player.entityVerticalAngle)));
            secondNum = screen.ScreenHeight - firstNum;
        }
        private static void calculatingCoordinatesX(Screen screen, Player player, ref int firstNum, ref int secondNum)
        {
            firstNum = (int)(screen.ScreenHeight * (1 - Math.Cos(player.entityVerticalAngle)));
            secondNum = screen.ScreenHeight - firstNum;
        }
        public static void renderPartsWorld(Screen screen, Player player)
        {

            int topRectHeight = 0;
            int bottomRectHeight = 0;
            if (topRect is not null && bottomRect is not null)
            {
                if(player.entityVerticalAngle < 0)
                    calculatingCoordinatesY(screen, player, ref topRectHeight, ref bottomRectHeight);
                else if(player.entityVerticalAngle > 0)
                    calculatingCoordinatesX(screen, player, ref bottomRectHeight, ref topRectHeight);
            }
            else
                throw new Exception("topRect or bottomRect is null");



           topRect.Size = new Vector2f(screen.ScreenWidth, topRectHeight);
           screen.Window.Draw(topRect);

           bottomRect.Size = new Vector2f(screen.ScreenWidth, bottomRectHeight);
           bottomRect.Position = new Vector2f(0, topRectHeight);
           screen.Window.Draw(bottomRect);
        }
    }
}
