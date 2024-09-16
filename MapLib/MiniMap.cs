using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace MapLib.MiniMapLib
{
    public class MiniMap(Screen screen, List<ValueTuple<int, int>> miniMap, Color fill)
    {
        const int miniMapSlowdownFactor = 19;

        const double mapScale = 5;
        const int radiusCircle = 6;

        const int sizeMainRayX = 50;
        const int sizeMainRayY = 50;

        readonly float centerX = screen.miniMapTexture.Size.X / 2;
        readonly float centerY = screen.miniMapTexture.Size.Y / 2;
        readonly int mapTile = screen.setting.Tile / (int)mapScale;
        // int rayWidth = screen.ScreenWidth / screen.setting.AmountRays;


        VertexArray line = new VertexArray(PrimitiveType.Lines, 2);
        public Sprite MiniMapSprite { get; set; }

        VertexArray renderLineSight(int mapX, int mapY, double entityA)
        {
            line[0] = new Vertex(new Vector2f(centerX, centerY), Color.Green);


            float endX = (float)((centerX - sizeMainRayX * (Math.Cos(entityA))));
            float endY = (float)((centerY - sizeMainRayY * (Math.Sin(entityA))));
            line[1] = new Vertex(new Vector2f(endX, endY), Color.Green);

            return line;
        }
        CircleShape renderEntityShape(int mapX, int mapY)
        {
            CircleShape entityShape = new CircleShape(radiusCircle)
            {
                FillColor = Color.Red,
                Position = new Vector2f(centerX - radiusCircle, centerY - radiusCircle)
            };
            return entityShape;
        }

        void miniMapCameraMovement(double mapX, double mapY)
        {
            foreach (var coo in miniMap)
            {
                RectangleShape tile = new RectangleShape(new Vector2f(mapTile, mapTile))
                {
                    FillColor = Color.Green,
                    OutlineThickness = 1,
                    OutlineColor = Color.Black,
                    Position = new Vector2f(
                (float)(centerX - (coo.Item1 - mapX) - miniMapSlowdownFactor),
                (float)(centerY - (coo.Item2 - mapY) - miniMapSlowdownFactor)
            )
                };

                screen.miniMapTexture.Draw(tile);
            }
        }
        void DrawDebugGrid()
        {
            int gridSize = 20;
            for (int i = 0; i < screen.miniMapTexture.Size.X; i += gridSize)
            {
                VertexArray line = new VertexArray(PrimitiveType.Lines, 2);
                line[0] = new Vertex(new Vector2f(i, 0), Color.White);
                line[1] = new Vertex(new Vector2f(i, screen.miniMapTexture.Size.Y), Color.White);
                screen.miniMapTexture.Draw(line);
            }
            for (int i = 0; i < screen.miniMapTexture.Size.Y; i += gridSize)
            {
                VertexArray line = new VertexArray(PrimitiveType.Lines, 2);
                line[0] = new Vertex(new Vector2f(0, i), Color.White);
                line[1] = new Vertex(new Vector2f(screen.miniMapTexture.Size.X, i), Color.White);
                screen.miniMapTexture.Draw(line);
            }
        }
        public void render(double entityX, double entityY, double entityA)
        {        
            screen.miniMapTexture.Clear(fill);


            int mapX = (int)(entityX / mapScale);
            int mapY = (int)(entityY / mapScale);


            screen.miniMapTexture.Draw(renderLineSight(mapX, mapY, entityA));
            screen.miniMapTexture.Draw(renderEntityShape(mapX, mapY));

            miniMapCameraMovement(mapX, mapY);

            //DrawDebugGrid();
            screen.miniMapTexture.Display();


            MiniMapSprite = new Sprite(screen.miniMapTexture.Texture)
            {
                Position = new Vector2f(0, screen.ScreenHeight - (int)(screen.ScreenHeight / mapScale))
            };

        }
    }
}
