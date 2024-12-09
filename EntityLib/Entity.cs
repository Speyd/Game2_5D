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

namespace EntityLib
{
    public class Entity
    {

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



        public double MaxRayMapDistance { get; private set; }
        public double MaxRaySpriteDistance { get; private set; }
        public double DeltaAngle { get; init; }


        public double Dist { get; init; }
        public double ProjCoeff { get; init; }

        public double Side { get; set; } = 10;


        public double Y { get; set; }
        public double X { get; set; }
        public double Angle { get; set; }
        public double VerticalAngle { get; set; }

        public Entity(Setting setting, double maxDistance,
            double fov = Math.PI / 3,
            double x = 0, double y = 0,
            double angle = 0, double verticalAngle = 0)
        {
            Fov = fov;

            X = x <= 0 ? setting.HalfWidth : x;
            Y = y <= 0 ? setting.HalfHeight : y;
            Angle = angle;
            VerticalAngle = verticalAngle;
          
            HalfFov = (float)Fov / 2;
            DeltaAngle = (float)Fov / setting.AmountRays;
           // CenterRay = (int)(Screen.Setting.AmountRays / 2) - 1;

            Dist = setting.AmountRays / (2 * (float)Math.Tan(HalfFov));
            ProjCoeff = Dist * setting.Tile;
            MaxRayMapDistance = maxDistance;

            MaxRaySpriteDistance = 1000;
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
