using MiniMapLib.ObjectInMap.Positions;
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
        public float CenterX { get; private set; }
        public float CenterY { get; private set; }

        public float MapTile { get; private set; }

        //----------------MapScale----------------
        private void SetMiniMapTile(int mapScale)
        {
            MapTile = Screen.Setting.Tile / mapScale;
        }



        private float _mapScale;
        public float MapScale 
        {
            get => _mapScale;
            set
            {
                _mapScale = value;
                SetMiniMapTile((int)value);
            }
        }

        public PositionsMiniMap Positions { get; set; }
        public Vector2f coorinatesPositionWindow;

        public Setting(RenderTexture Window, PositionsMiniMap positions, float mapScale = 5)
        {
            MapScale = mapScale;
            Positions = positions;

            CenterX = Window.Size.X / 2;
            CenterY = Window.Size.Y / 2;
        }
    }
}
