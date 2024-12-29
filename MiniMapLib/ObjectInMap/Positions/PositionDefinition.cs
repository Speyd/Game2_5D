using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using MiniMapLib.SettingMap;
using ScreenLib;

namespace MiniMapLib.ObjectInMap.Positions
{
    public class PositionDefinition
    {
        //---------------------Setting---------------------
        Setting Setting { get; init; }


        //-------------------Positions---------------------
        private PositionsMiniMap _positions = PositionsMiniMap.UpperRightCorner;
        public PositionsMiniMap Positions
        {
            get => _positions;
            set
            {
                _positions = value;
                SetPosition();
            }
        }


        //-------------------Coordinates---------------------
        public Vector2f CooPositions { get; set; }



        public PositionDefinition(Setting setting,
            PositionsMiniMap positions = PositionsMiniMap.UpperRightCorner)
        {
            Setting = setting;
            Positions = positions;

            Screen.WidthChangesFun += SetPosition;
            Screen.HeightChangesFun += SetPosition;
        }



        private float GetLowerY()
        {
            float mapHeight = Screen.ScreenHeight / Setting.MapScale;
            return Screen.ScreenHeight - mapHeight * (float)(Math.PI / 2);
        }
        private float GetLowerX()
        {
            float mapWidth = Screen.ScreenWidth / Setting.MapScale;
            return Screen.ScreenWidth - mapWidth / (float)(Math.PI / 2);
        }



        private Vector2f GetLowerLeftCorner()
        {
            return new Vector2f(0, GetLowerY());
        }
        private Vector2f GetLowerRightCorner()
        {
            return new Vector2f(GetLowerX(), GetLowerY());
        }
        private Vector2f GetUpperRightCorner()
        {
            return new Vector2f(GetLowerX(), 0);
        }
        private Vector2f GetUpperLeftCorner()
        {
            return new Vector2f(0, 0);
        }


        public void SetPosition()
        {
            switch (Positions)
            {
                case PositionsMiniMap.LowerLeftCorner:
                    CooPositions = GetLowerLeftCorner(); break;
                case PositionsMiniMap.LowerRightCorner:
                    CooPositions = GetLowerRightCorner(); break;
                case PositionsMiniMap.UpperRightCorner:
                    CooPositions = GetUpperRightCorner(); break;
                case PositionsMiniMap.UpperLeftCorner:
                    CooPositions = GetUpperLeftCorner(); break;
                default:
                    CooPositions = GetUpperRightCorner(); break;
            }
        }
    }
}
