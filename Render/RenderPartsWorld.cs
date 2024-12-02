using EntityLib;
using EntityLib.Player;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Render.RenderPartsWorld
{

    static public class RenderPartsWorld
    {
        private static Texture? skyTexture = new Texture(@"Resources\Image\PartsWorldTexture\Sky.png");
        private static SFML.Graphics.Color colorSky = new Color(100, 149, 237);

        private static Texture? bottomTexture = null;
        private static SFML.Graphics.Color colorBottom = new Color(20, 20, 20);

        private const int stretchingTexture = -5;

        private static RectangleShape filledTopRectangle = new RectangleShape();
        private static RectangleShape filledBottomRectangle = new RectangleShape();

        private static Sprite topRect = new Sprite();

        private static void SetCoordinate(ref int topRectHeight, ref int bottomRectHeight, Player player)
        {
            if (player.VerticalAngle > 0)
            {
                topRectHeight = (int)(Screen.Setting.HalfHeight / (1 + 1 * player.VerticalAngle));
                bottomRectHeight = Screen.ScreenHeight - topRectHeight;
            }
            else if (player.VerticalAngle < 0)
            {
                topRectHeight = (int)(Screen.Setting.HalfHeight * (1 + 1 * -player.VerticalAngle));
                bottomRectHeight = Screen.ScreenHeight - bottomRectHeight;
            }
            else
            {
                topRectHeight = Screen.Setting.HalfHeight;
                bottomRectHeight = Screen.ScreenHeight - topRectHeight;
            }
        }

        private static void RenderBottomRect(ref int topRectHeight, ref int bottomRectHeight, Player player)
        {
            if (bottomTexture is not null)
            {
                Sprite bottomRect = new Sprite(bottomTexture);

                bottomRect.Scale = new Vector2f(Screen.ScreenWidth / (float)bottomTexture.Size.X, bottomRectHeight / (float)bottomTexture.Size.Y);            
                bottomRect.Position = new Vector2f(0, topRectHeight);

                Screen.OutputPriority.AddToPriority(1, bottomRect);
            }
            else
            {
                filledBottomRectangle.Scale = new Vector2f(Screen.ScreenWidth, bottomRectHeight);
                filledBottomRectangle.FillColor = colorBottom;
                filledBottomRectangle.Size = new Vector2f(Screen.ScreenWidth, bottomRectHeight);
                filledBottomRectangle.Position = new Vector2f(0, topRectHeight);

                Screen.OutputPriority.AddToPriority(1, filledBottomRectangle);
            }
        }

        private static void RenderTopRect(ref int topRectHeight, ref int bottomRectHeight, Player player)
        {
            if (skyTexture is not null)
            {
                skyTexture.Repeated = true;
                float skyOffset = (float)(stretchingTexture * (player.Angle * (180 / (float)Math.PI)) % Screen.ScreenWidth);
                skyOffset /= Screen.MultWidth;

                topRect = new Sprite(skyTexture);

                float scaleX = Screen.ScreenWidth / (float)skyTexture.Size.X * Screen.MultWidth;
                float scaleY = Screen.ScreenWidth / (float)skyTexture.Size.Y * Screen.MultWidth;

                topRect.Scale = new Vector2f(scaleX, scaleY);

                topRect.Position = new Vector2f(skyOffset, 0);
                Screen.OutputPriority.AddToPriority(1, new Sprite(topRect));

                topRect.Position = new Vector2f(skyOffset - Screen.ScreenWidth, 0);
                Screen.OutputPriority.AddToPriority(1, new Sprite(topRect));

                topRect.Position = new Vector2f(skyOffset + Screen.ScreenWidth, 0);
                Screen.OutputPriority.AddToPriority(1, new Sprite(topRect));
            }
            else
            {
                filledTopRectangle.Scale = new Vector2f(Screen.ScreenWidth, bottomRectHeight);
                filledTopRectangle.FillColor = colorSky;
                filledTopRectangle.Size = new Vector2f(Screen.ScreenWidth, topRectHeight);

                Screen.OutputPriority.AddToPriority(1, filledTopRectangle);
            }
        }
        public static void Render(Player player)
        {
            int topRectHeight = 0;
            int bottomRectHeight = 0;

            SetCoordinate(ref topRectHeight, ref bottomRectHeight, player);

            RenderTopRect(ref topRectHeight, ref bottomRectHeight, player);
            RenderBottomRect(ref topRectHeight, ref bottomRectHeight, player);
        }


        public static void SetSkyTexture(Texture texture)
        {
            skyTexture = texture;
        }
        public static void SetBottomTexture(Texture texture)
        {
            bottomTexture = texture;
        }
    }
}
