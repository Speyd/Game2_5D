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
       // private float miniMapSlowdownFactor = 19;   //slow movement on minimap
        //private int radiusCircle = 5;

       // private const int sizeMainRayX = 50;
        //private const int sizeMainRayY = 50;

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

        //#region Zoom
        //public readonly float minZoom = 0.1f;
        //public readonly float maxZoom = 2;

        //private float zoom = 1;
        //public float Zoom
        //{
        //    get => zoom;
        //    set
        //    {
        //        if (value < minZoom)
        //            zoom = minZoom;
        //        else if(value > maxZoom)
        //            zoom = maxZoom;
        //        else
        //            zoom = value;
        //    }
        //}
        //#endregion


       // public VertexArray line = new VertexArray(PrimitiveType.Lines, 2);
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
