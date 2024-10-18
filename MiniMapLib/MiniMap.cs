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
using MiniMapLib.SettingMap;
using System.Threading;
using MapLib;

namespace MiniMapLib
{
    public class MiniMap
    {
        private readonly float mapScale = 5;


        private Screen screen;
        private Map map;
        private Color fill;


        private WindowRender Window { get; init; }
        public Setting Setting { get; init; }
        private Border BorderMap { get; init; }


        public MiniMap(Screen screen, Map map, Color fill,
            Positions position,
            float mapScale = 5, float zoom = 0,
            string? pathBorder = null)
        {
            this.mapScale = mapScale;
            this.screen = screen;
            this.map = map;
            this.fill = fill;


            Window = new WindowRender(screen, mapScale);
            Setting = new Setting(screen, Window.WindowMap, position, mapScale, zoom);
            BorderMap = new Border(pathBorder);
        }

        VertexArray renderLineSight(double entityA)
        {
            Setting.line[0] = new Vertex(new Vector2f(Setting.centerX, Setting.centerY), Color.Green);

            float endX = (float)(Setting.centerX - Setting.getSizeMainRayX() * Math.Cos(entityA));
            float endY = (float)((Setting.centerY - Setting.getSizeMainRayY() * (Math.Sin(entityA))));
            Setting.line[1] = new Vertex(new Vector2f(endX, endY), Color.Green);

            return Setting.line;
        }
        CircleShape renderEntityShape()
        {
            float x = Setting.centerX - Setting.getRadiusCircle();
            float y = Setting.centerY - Setting.getRadiusCircle();

            CircleShape entityShape = new CircleShape(Setting.getRadiusCircle())
            {
                FillColor = Color.Red,
                Position = new Vector2f(x, y)
            };
            return entityShape;
        }

        void renderObstacle(double mapX, double mapY)
        {
            foreach (var obstacle in map.Obstacles)
            {
                float x = (obstacle.Key.Item1 / map.Setting.ScreenTile) * (Setting.mapTile - Setting.zoom);
                float y = (obstacle.Key.Item2 / map.Setting.ScreenTile) * (Setting.mapTile - Setting.zoom);

                RectangleShape rectangleShape = new RectangleShape(new Vector2f(Setting.mapTile, Setting.mapTile));

                obstacle.Value.fillingMiniMapShape(rectangleShape);
                rectangleShape.Position = new Vector2f
                    (
                        (float)(Setting.centerX - (x - mapX) - Setting.getMiniMapSlowdownFactor()),
                        (float)(Setting.centerY - (y - mapY) - Setting.getMiniMapSlowdownFactor())
                    );

                Window.WindowMap.Draw(rectangleShape);
            }
        }
        void drawDebugGrid()
        {
            int gridSize = 20;
            for (int i = 0; i < Window.WindowMap.Size.X; i += gridSize)
            {
                VertexArray line = new VertexArray(PrimitiveType.Lines, 2);
                line[0] = new Vertex(new Vector2f(i, 0), Color.White);
                line[1] = new Vertex(new Vector2f(i, Window.WindowMap.Size.Y), Color.White);
                Window.WindowMap.Draw(line);
            }
            for (int i = 0; i < Window.WindowMap.Size.Y; i += gridSize)
            {
                VertexArray line = new VertexArray(PrimitiveType.Lines, 2);
                line[0] = new Vertex(new Vector2f(0, i), Color.White);
                line[1] = new Vertex(new Vector2f(Window.WindowMap.Size.X, i), Color.White);
                Window.WindowMap.Draw(line);
            }
        }
        void drawMiniMapBorder()
        {
            if (BorderMap.borderTexture is null)
                return;


            float scaleX = (float)Window.WindowMap.Size.X / BorderMap.borderTexture.Size.X * 1.1f;
            float scaleY = (float)Window.WindowMap.Size.Y / BorderMap.borderTexture.Size.Y * 1.1f;

            BorderMap.borderSprite = new Sprite(BorderMap.borderTexture)
            {
                Scale = new Vector2f(scaleX, scaleY)
            };


            BorderMap.borderSprite.Position = new Vector2f(
                (Window.WindowMap.Size.X - (BorderMap.borderTexture.Size.X * scaleX)) / 2,
                (Window.WindowMap.Size.Y - (BorderMap.borderTexture.Size.Y * scaleY)) / 2
            );

            Window.WindowMap.Draw(BorderMap.borderSprite);
        }
        public void render(double entityX, double entityY, double entityA)
        {
            int mapX = (int)(entityX / mapScale);
            int mapY = (int)(entityY / mapScale);


            Window.WindowMap.Clear(fill);


            Window.WindowMap.Draw(renderLineSight(entityA));
            renderObstacle(mapX, mapY);
            Window.WindowMap.Draw(renderEntityShape());
            drawMiniMapBorder();


            Window.WindowMap.Display();


            Window.MiniMapSprite = new Sprite(Window.WindowMap.Texture)
            {
                Position = Setting.coorinatesPositionWindow
            };
            screen.Window.Draw(Window.MiniMapSprite);
        }
    }
}
