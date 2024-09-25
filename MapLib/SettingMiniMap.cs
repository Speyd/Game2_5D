using ScreenLib;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.MiniMapLib.Setting
{
    public class SettingMiniMap
    {
        private const int miniMapSlowdownFactor = 19;   //slow movement on minimap
        private const int radiusCircle = 6;

        private const int sizeMainRayX = 50;
        private const int sizeMainRayY = 50;

        public readonly float centerX;
        public readonly float centerY;

        public readonly int mapTile;

        public VertexArray line = new VertexArray(PrimitiveType.Lines, 2);
        public SettingMiniMap(Screen screen, RenderTexture Window, double mapScale)
        {
            centerX = Window.Size.X / 2;
            centerY = Window.Size.Y / 2;

            mapTile = screen.setting.Tile / (int)mapScale;
        }

        public int getSizeMainRayX() => sizeMainRayX;
        public int getSizeMainRayY() => sizeMainRayY;

        public int getMiniMapSlowdownFactor() => miniMapSlowdownFactor;
        public int getRadiusCircle() => radiusCircle;

    }
}
