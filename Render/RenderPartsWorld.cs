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


        private static void SetCoordinate(ref int topRectHeight, ref int bottomRectHeight,
            Screen screen, Player player)
        {
            if (player.GetEntityVerticalA() > 0)
            {
                topRectHeight = (int)(screen.Setting.HalfHeight / (1 + 1 * player.GetEntityVerticalA()));
                bottomRectHeight = screen.ScreenHeight - topRectHeight;
            }
            else if (player.GetEntityVerticalA() < 0)
            {
                topRectHeight = (int)(screen.Setting.HalfHeight * (1 + 1 * -player.GetEntityVerticalA()));
                bottomRectHeight = screen.ScreenHeight - bottomRectHeight;
            }
            else
            {
                topRectHeight = screen.Setting.HalfHeight;
                bottomRectHeight = screen.ScreenHeight - topRectHeight;
            }
        }

        private static void RenderBottomRect(ref int topRectHeight, ref int bottomRectHeight, Screen screen, Player player)
        {
            if (bottomTexture is not null)
            {
                Sprite bottomRect = new Sprite(bottomTexture);

                bottomRect.Scale = new Vector2f(screen.ScreenWidth / (float)bottomTexture.Size.X, bottomRectHeight / (float)bottomTexture.Size.Y);
                bottomRect.Position = new Vector2f(0, topRectHeight);

                screen.Window.Draw(bottomRect);
            }
            else
            {
                RectangleShape filledRectangle = new RectangleShape(new Vector2f(screen.ScreenWidth, bottomRectHeight))
                {
                    FillColor = colorBottom,
                    Size = new Vector2f(screen.ScreenWidth, bottomRectHeight),
                    Position = new Vector2f(0, topRectHeight),
                };
                screen.Window.Draw(filledRectangle);
            }
        }

        private static void RenderTopRect(ref int topRectHeight, ref int bottomRectHeight, Screen screen, Player player)
        {
            if (skyTexture is not null)
            {
                skyTexture.Repeated = true;
                float skyOffset = (float)(stretchingTexture * (player.GetEntityA() * (180 / (float)Math.PI)) % screen.ScreenWidth);

                Sprite topRect = new Sprite(skyTexture);

                float scaleX = screen.ScreenWidth / (float)skyTexture.Size.X;
                float scaleY = screen.ScreenWidth / (float)skyTexture.Size.Y;
                topRect.Scale = new Vector2f(scaleX, scaleY);

                topRect.Position = new Vector2f(skyOffset, 0);
                screen.Window.Draw(topRect);

                topRect.Position = new Vector2f(skyOffset - screen.ScreenWidth, 0);
                screen.Window.Draw(topRect);

                topRect.Position = new Vector2f(skyOffset + screen.ScreenWidth, 0);
                screen.Window.Draw(topRect);
            }
            else
            {
                RectangleShape filledRectangle = new RectangleShape(new Vector2f(screen.ScreenWidth, bottomRectHeight))
                {
                    FillColor = colorSky,
                    Size = new Vector2f(screen.ScreenWidth, topRectHeight),
                };
                screen.Window.Draw(filledRectangle);
            }
        }
        public static void Render(Screen screen, Player player)
        {
            int topRectHeight = 0;
            int bottomRectHeight = 0;

            SetCoordinate(ref topRectHeight, ref bottomRectHeight, screen, player);

            RenderTopRect(ref topRectHeight, ref bottomRectHeight, screen, player);
            RenderBottomRect(ref topRectHeight, ref bottomRectHeight, screen, player);
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
