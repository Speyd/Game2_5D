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
using MapLib.MiniMapLib.Setting;
using System.Threading;

namespace MapLib.MiniMapLib
{
    public class MiniMap
    {
        private Screen screen;
        private Map map;
        private Color fill;

        private RenderTexture Window { get; init; }
        private readonly double mapScale = 5;
        public Sprite MiniMapSprite { get; set; } = new Sprite();

        public SettingMiniMap Setting { get; init; }

        private Texture? borderTexture;
        private Sprite borderSprite = new Sprite();

        public MiniMap(Screen screen, Map map, Color fill, string? path = null, double mapScale = 5)
        {
            this.mapScale = mapScale;

            uint sizeX = (uint)(screen.ScreenWidth / (mapScale * (Math.PI / 2)));
            uint sizeY = (uint)(screen.ScreenHeight / (mapScale / (Math.PI / 2)));
            Window = new RenderTexture(sizeX, sizeY);
            Setting = new SettingMiniMap(screen, Window, mapScale);

            this.screen = screen;
            this.map = map;
            this.fill = fill;

            borderTexture = path is not null ? new Texture(path) : null;
            if(borderTexture is not null)
                borderTexture.Smooth = true;
        }

        VertexArray renderLineSight(int mapX, int mapY, double entityA)
        {
            Setting.line[0] = new Vertex(new Vector2f(Setting.centerX, Setting.centerY), Color.Green);


            float endX = (float)(Setting.centerX - Setting.getSizeMainRayX() * Math.Cos(entityA));
            float endY = (float)((Setting.centerY - Setting.getSizeMainRayY() * (Math.Sin(entityA))));
            Setting.line[1] = new Vertex(new Vector2f(endX, endY), Color.Green);

            return Setting.line;
        }
        CircleShape renderEntityShape(int mapX, int mapY)
        {
            CircleShape entityShape = new CircleShape(Setting.getRadiusCircle())
            {
                FillColor = Color.Red,
                Position = new Vector2f(Setting.centerX - Setting.getRadiusCircle(), Setting.centerY - Setting.getRadiusCircle())
            };
            return entityShape;
        }

        void renderObstacle(double mapX, double mapY)
        {
            foreach (var coo in map.Obstacles)
            {
                float x = (coo.Key.Item1 / map.Setting.ScreenTile) * Setting.mapTile;
                float y = (coo.Key.Item2 / map.Setting.ScreenTile) * Setting.mapTile;

                RectangleShape tile = new RectangleShape(new Vector2f(Setting.mapTile, Setting.mapTile));

                tile.FillColor = coo.Value.Color;
                tile.OutlineThickness = 1;
                tile.OutlineColor = Color.Black;
                tile.Position = new Vector2f
                    (
                        (float)(Setting.centerX - (x - mapX) - Setting.getMiniMapSlowdownFactor()),
                        (float)(Setting.centerY - (y - mapY) - Setting.getMiniMapSlowdownFactor())
                    );

                Window.Draw(tile);
            }
        }
        void drawDebugGrid()
        {
            int gridSize = 20;
            for (int i = 0; i < Window.Size.X; i += gridSize)
            {
                VertexArray line = new VertexArray(PrimitiveType.Lines, 2);
                line[0] = new Vertex(new Vector2f(i, 0), Color.White);
                line[1] = new Vertex(new Vector2f(i, Window.Size.Y), Color.White);
                Window.Draw(line);
            }
            for (int i = 0; i < Window.Size.Y; i += gridSize)
            {
                VertexArray line = new VertexArray(PrimitiveType.Lines, 2);
                line[0] = new Vertex(new Vector2f(0, i), Color.White);
                line[1] = new Vertex(new Vector2f(Window.Size.X, i), Color.White);
                Window.Draw(line);
            }
        }

        private float getMultiY()
        {
            if (borderTexture is null)
                return 0;

            if (5000 / borderTexture.Size.Y > 1)
            {
                return 0.6f + ((float)Math.Floor(5000f / borderTexture.Size.Y) / 10) - 0.1f;
            }
            else if (5000 / borderTexture.Size.Y == 1)
            {
                return 5000f / borderTexture.Size.X;
            }
            else
            {
                return 0.8f + ((borderTexture.Size.Y / 5000) / 10) - 0.1f;
            }
        }
        private float getMultiX()
        {
            if (borderTexture is null)
                return 0;

            if (5000 / borderTexture.Size.X > 1)
            {
                return 0.8f + ((float)Math.Floor(5000.0 / borderTexture.Size.X) / 10) - 0.1f;
            }
            else if (5000 / borderTexture.Size.X == 1)
            {
                return 5000f / borderTexture.Size.Y;
            }
            else
            {
                return 0.8f - ((5000 / borderTexture.Size.X) / 10) + 0.1f;
            }
        }

        void drawMiniMapBorder()
        {
            if (borderTexture is null)
                return;

            float multiY = getMultiY();
            float multiX = getMultiX();

            float scaleX = (float)Window.Size.X / borderTexture.Size.X / multiX;
            float scaleY = (float)Window.Size.Y / borderTexture.Size.Y * multiY;

            borderSprite = new Sprite(borderTexture)
            {
                Scale = new Vector2f(scaleX, scaleY)
            };


            borderSprite.Position = new Vector2f(
                (Window.Size.X - (borderTexture.Size.X * scaleX)) / 2,
                (Window.Size.Y - (borderTexture.Size.Y * scaleY)) / 2 - 60
            );

            Window.Draw(borderSprite);
        }
        public void render(double entityX, double entityY, double entityA)
        {
            Window.Clear(fill);

            int mapX = (int)(entityX / mapScale);
            int mapY = (int)(entityY / mapScale);

            Window.Draw(renderLineSight(mapX, mapY, entityA));
            Window.Draw(renderEntityShape(mapX, mapY));
            renderObstacle(mapX, mapY);
            drawMiniMapBorder();

            Window.Display();


            MiniMapSprite = new Sprite(Window.Texture)
            {
                Position = new Vector2f(0, screen.ScreenHeight - (int)(screen.ScreenHeight / mapScale))
            };

            screen.Window.Draw(MiniMapSprite);
        }
    }
}
