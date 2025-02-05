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
using HitBoxLib;
using HitBoxLib.PositionObject;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EntityLib
{
    public class Entity
    {
        

        public Vector2f Position
        {
            get
            {
                return new Vector2f((float)X.Axis, (float)Y.Axis) / Screen.Setting.Tile;
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


        //---------------------Coordinates----------------------
        public HitBox HitBox { get; init; }
        public Coordinate Y { get; init; }
        public Coordinate X { get; init; }
        public Coordinate Z { get; init; }


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




        public Entity(Setting setting, double maxDistance,
            double fov = Math.PI / 3,
            double x = 0, double y = 0,
            double angle = 0, double verticalAngle = 0)
        {
            Fov = fov;
            HalfFov = (float)Fov / 2;

            HitBox = new HitBox();
            X = new Coordinate(CoordinatePlane.X, HitBox);
            Y = new Coordinate(CoordinatePlane.Y, HitBox);
            Z = new Coordinate(CoordinatePlane.Z, HitBox);
            Z.Axis = 50;

            Angle = angle;
            VerticalAngle = verticalAngle;

            EntitySettingChangesFun();
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
            double nextX = X.Axis + deltaX;
            double nextY = Y.Axis + deltaY;
            return (nextX, nextY);
        }
        public (float x1, float y1) CalculateEndPoint()
        {
            float x1 = (float)(X.Axis + MaxRayMapDistance * Math.Cos(Angle));
            float y1 = (float)(Y.Axis + MaxRayMapDistance * Math.Sin(Angle));

            return (x1, y1);
        }
    }
}
