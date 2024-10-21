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
            radiusCircle = radiusCircle;
            miniMapSlowdownFactor = mapTile;

            coorinatesPositionWindow = getPosition(screen, mapScale);
            Zoom = zoom;
        }

        private float getLowerY(Screen screen, double mapScale)
        {
            float mapHeight = screen.ScreenHeight / (float)mapScale;
            return screen.ScreenHeight - (mapHeight * (float)(Math.PI / 2));
        }
        private float getLowerX(Screen screen, double mapScale)
        {
            float mapWidth = screen.ScreenWidth / (float)mapScale;
            return screen.ScreenWidth - (mapWidth / (float)(Math.PI / 2));
        }

        private Vector2f getLowerLeftCorner(Screen screen, double mapScale)
        {
            return new Vector2f(0, getLowerY(screen, mapScale));
        }

        private Vector2f getLowerRightCorner(Screen screen, double mapScale)
        {
            return new Vector2f(getLowerX(screen, mapScale), getLowerY(screen, mapScale));
        }

        private Vector2f getUpperRightCorner(Screen screen, double mapScale)
        {
            return new Vector2f(getLowerX(screen, mapScale), 0);
        }

        private Vector2f getUpperLeftCorner()
        {
            return new Vector2f(0, 0);
        }


        public Vector2f getPosition(Screen screen, double mapScale)
        {
            Vector2f coorinatesPos = new Vector2f();
            switch (Positions) 
            { 
                case Positions.LowerLeftCorner:
                    coorinatesPos = getLowerLeftCorner(screen, mapScale); break;
                case Positions.LowerRightCorner:
                    coorinatesPos = getLowerRightCorner(screen, mapScale); break;
                case Positions.UpperRightCorner:
                    coorinatesPos = getUpperRightCorner(screen, mapScale); break;
                case Positions.UpperLeftCorner:
                    coorinatesPos = getUpperLeftCorner(); break;
                default:
                    coorinatesPos = getUpperRightCorner(screen, mapScale); break;
            }

            return coorinatesPos;
        }

        public void setPositions(Screen screen, double mapScale, Positions positions)
        {
            Positions = positions;
            getPosition(screen, mapScale);
        }
        public int getSizeMainRayX() => sizeMainRayX;
        public int getSizeMainRayY() => sizeMainRayY;

        public float getMiniMapSlowdownFactor() => miniMapSlowdownFactor;
        public int getRadiusCircle() => radiusCircle;

    }
}
