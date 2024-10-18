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
        public double Depth { get; set; } = 0;
        public double Offset { get; set; } = 0;
        public double ProjHeight { get; set; } = 0;
        public int Ray { get; set; } = 0;
        public IRenderable obstacle { get; set; }

        private void redefinitionValues(
            ref ValueTuple<IRenderable, IRenderable> obstacles,
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
            else if(depth_h < depth_v)
            {
                Offset = hx;
                Depth = depth_h;

                obstacle = obstacles.Item2;
            }

           
            Depth *= Math.Cos(entity.getEntityA() - car_angle);
            Depth = Math.Max(Depth, 0.1);
        }

        public void calculationSettingRender(ref Screen screen, ref Entity entity,
            ref ValueTuple<IRenderable, IRenderable> obstacles, int ray,
            double depth_v, double depth_h,
            double hx, double vy,
            double car_angle)
        {
            redefinitionValues(ref obstacles, ref entity, depth_v, depth_h, hx, vy, car_angle);

            Ray = ray;

            Offset = (int)Offset % screen.Setting.Tile;
            Depth = Math.Max(Depth, 0.1);
            ProjHeight = Math.Min((int)(entity.ProjCoeff / Depth), 6 * screen.ScreenHeight);
        }
    }
}
