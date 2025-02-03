using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;
using ScreenLib.SettingScreen;
using SFML.System;
using SFML.Window;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EntityLib
{
    public class Entity
    {
        

        public Vector2f Position
        {
            get
            {
                return new Vector2f((float)X, (float)Y) / Screen.Setting.Tile;
            }
        }




        //-----------------Fov-----------------
        private double _fov;
        public double Fov 
        { 
            get => _fov;
            set
            {
                _fov = value;
                HalfFov = value / 2;
            }
        }
        public double HalfFov { get; private set; }


        //---------------------Ray Setting----------------------
        public double MaxRayMapDistance { get; private set; }
        public double MaxRaySpriteDistance { get; private set; }


        //---------------------Render Setting----------------------
        public double DeltaAngle { get; private set; }
        public double ProjCoeff { get; private set; }


        //---------------------Collision Setting----------------------
        public double Side { get; set; } = 10;


        //---------------------Coordinates----------------------
        public double Y { get; set; }
        public double X { get; set; }


        //-----------------------Angle-----------------------

        public Vector2f Direction { get; private set; }
        public Vector2f Plane { get; private set; }

        private double _angle;
        public double Angle
        {
            get => _angle;
            set
            {
                if (_angle != value)
                {
                    _angle = value;
                    float cos = (float)Math.Cos(_angle);
                    float sin = (float)Math.Sin(_angle);
                    Direction = new Vector2f(cos, sin);
                    Plane = new Vector2f(-sin, cos);
                }
            }
        }
        public double VerticalAngle { get; set; }


        //-----------------------Camera-----------------------
        private double _z;
        public double Z
        {
            get => _z;
            set => _z = value;
        }



        public Entity(Setting setting, double maxDistance,
            double fov = Math.PI / 3,
            double x = 0, double y = 0,
            double angle = 0, double verticalAngle = 0)
        {
            Fov = fov;
            HalfFov = (float)Fov / 2;
            Z = Screen.Setting.Tile;


            X = x <= 0 ? Screen.Setting.HalfWidth : x;
            Y = y <= 0 ? Screen.Setting.HalfHeight : y;


            Angle = angle;
            VerticalAngle = verticalAngle;
            DeltaAngle = (float)Fov / Screen.Setting.AmountRays;


            float dist = Screen.Setting.AmountRays / (2 * (float)Math.Tan(HalfFov));
            ProjCoeff = dist * Screen.Setting.Tile;
            Screen.WidthChangesFun += EntitySettingChangesFun;

            MaxRayMapDistance = maxDistance;
            MaxRaySpriteDistance = 1000;
        }


        private void EntitySettingChangesFun()
        {
            float dist = Screen.Setting.AmountRays / (2 * (float)Math.Tan(HalfFov));
            ProjCoeff = dist * Screen.Setting.Tile;

            DeltaAngle = (float)Fov / Screen.Setting.AmountRays;
        }
        public (double nextX, double nextY) CalculateNextPosition(double deltaX, double deltaY)
        {
            double nextX = X + deltaX;
            double nextY = Y + deltaY;
            return (nextX, nextY);
        }
        public (float x1, float y1) CalculateEndPoint()
        {
            float x1 = (float)(X + MaxRayMapDistance * Math.Cos(Angle));
            float y1 = (float)(Y + MaxRayMapDistance * Math.Sin(Angle));

            return (x1, y1);
        }
    }
}
