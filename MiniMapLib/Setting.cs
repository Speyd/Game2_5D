using ScreenLib;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMapLib.SettingMap
{
    public class Setting
    {
        private float miniMapSlowdownFactor = 19;   //slow movement on minimap
        private int radiusCircle = 5;

        private const int sizeMainRayX = 50;
        private const int sizeMainRayY = 50;

        public readonly float centerX;
        public readonly float centerY;

        public readonly int mapTile;

        #region Zoom
        public readonly float minZoom = 0.1f;
        public readonly float maxZoom = 2;

        private float zoom = 1;
        public float Zoom
        {
            get => zoom;
            set
            {
                if (value < minZoom)
                    zoom = minZoom;
                else if(value > maxZoom)
                    zoom = maxZoom;
                else
                    zoom = value;
            }
        }
        #endregion


        public VertexArray line = new VertexArray(PrimitiveType.Lines, 2);
        public Positions Positions { get; set; }
        public Vector2f coorinatesPositionWindow;

        public Setting(Screen screen, RenderTexture Window, Positions positions, double mapScale, float zoom)
        {
            Positions = positions;

            centerX = Window.Size.X / 2;
            centerY = Window.Size.Y / 2;

            this.zoom = zoom;

            mapTile = (screen.Setting.Tile / (int)mapScale);
            miniMapSlowdownFactor = mapTile;

            coorinatesPositionWindow = GetPosition(screen, mapScale);
            Zoom = zoom;
        }

        private float GetLowerY(Screen screen, double mapScale)
        {
            float mapHeight = screen.ScreenHeight / (float)mapScale;
            return screen.ScreenHeight - (mapHeight * (float)(Math.PI / 2));
        }
        private float GetLowerX(Screen screen, double mapScale)
        {
            float mapWidth = screen.ScreenWidth / (float)mapScale;
            return screen.ScreenWidth - (mapWidth / (float)(Math.PI / 2));
        }

        private Vector2f GetLowerLeftCorner(Screen screen, double mapScale)
        {
            return new Vector2f(0, GetLowerY(screen, mapScale));
        }

        private Vector2f GetLowerRightCorner(Screen screen, double mapScale)
        {
            return new Vector2f(GetLowerX(screen, mapScale), GetLowerY(screen, mapScale));
        }

        private Vector2f GetUpperRightCorner(Screen screen, double mapScale)
        {
            return new Vector2f(GetLowerX(screen, mapScale), 0);
        }

        private Vector2f GetUpperLeftCorner()
        {
            return new Vector2f(0, 0);
        }


        public Vector2f GetPosition(Screen screen, double mapScale)
        {
            Vector2f coorinatesPos = new Vector2f();
            switch (Positions) 
            { 
                case Positions.LowerLeftCorner:
                    coorinatesPos = GetLowerLeftCorner(screen, mapScale); break;
                case Positions.LowerRightCorner:
                    coorinatesPos = GetLowerRightCorner(screen, mapScale); break;
                case Positions.UpperRightCorner:
                    coorinatesPos = GetUpperRightCorner(screen, mapScale); break;
                case Positions.UpperLeftCorner:
                    coorinatesPos = GetUpperLeftCorner(); break;
                default:
                    coorinatesPos = GetUpperRightCorner(screen, mapScale); break;
            }

            return coorinatesPos;
        }

        public void SetPositions(Screen screen, double mapScale, Positions positions)
        {
            Positions = positions;
            GetPosition(screen, mapScale);
        }
        public int GetSizeMainRayX() => sizeMainRayX;
        public int GetSizeMainRayY() => sizeMainRayY;

        public float GetMiniMapSlowdownFactor() => miniMapSlowdownFactor;
        public int GetRadiusCircle() => radiusCircle;

    }
}
