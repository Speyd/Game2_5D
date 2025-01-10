using EntityLib;
using ScreenLib;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Render.InterfaceRender;

namespace Render.ResultAlgorithm
{
    public class Result
    {
        private const int textureStretchingCloseUp = 8;

        public double Depth { get; set; } = 0;
        public double Offset { get; set; } = 0;
        public double ProjHeight { get; set; } = 0;
        public double CarAngle {  get; set; } = 0;
        public int Ray { get; set; } = 0;
        public IRenderable? obstacle { get; set; }

        private void RedefinitionValues(
            ref ValueTuple<IRenderable?, IRenderable?> obstacles,
            ref Entity entity,
            double depth_v, double depth_h,
            double hx, double vy,
            double car_angle)
        {
            if (depth_v < depth_h)
            {
                Offset = vy;
                Depth = depth_v;
                obstacle = obstacles.Item1;
            }
            else
            {
                Offset = hx;
                Depth = depth_h;
                obstacle = obstacles.Item2;
            }


            Depth *= Math.Cos(entity.Angle - car_angle);
            Depth = Math.Max(Depth, 0.1);
            
        }

        public void CalculationSettingRender(Entity entity,
            ValueTuple<IRenderable?, IRenderable?> obstacles, int ray,
            double depth_v, double depth_h,
            double hx, double vy,
            double car_angle)
        {
            RedefinitionValues(ref obstacles, ref entity, depth_v, depth_h, hx, vy, car_angle);

            Ray = ray;
            CarAngle = car_angle;

            Offset = (int)Offset % Screen.Setting.Tile;
            ProjHeight = Math.Min((int)(entity.ProjCoeff / Depth), textureStretchingCloseUp * Screen.ScreenHeight);
        }

        public void CalculationSettingRender(Entity entity, int ray,double depth, double a, double car_angle)
        {
            // RedefinitionValues(ref obstacles, ref entity, depth_v, depth_h, hx, vy, car_angle);
            Offset = a;

            Depth = depth;
            Depth *= Math.Cos(entity.Angle - car_angle);
            Depth = Math.Max(Depth, 0.1);

            Ray = ray;
            CarAngle = car_angle;

            Offset = (int)Offset % Screen.Setting.Tile;
            ProjHeight = Math.Min((int)(entity.ProjCoeff / Depth), textureStretchingCloseUp * Screen.ScreenHeight);
        }
    }
}
