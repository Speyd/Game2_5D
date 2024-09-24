using EntityLib;
using ScreenLib;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderLib
{
    internal class Setting
    {
        public double Depth { get; set; } = 0;
        public double Offset { get; set; } = 0;
        public double ProjHeight { get; set; } = 0;

        private int textureHeightMultiplierMear = 6;

        public Texture? Texture { get; set; } = null;


        private void redefinitionValues(
            ref ValueTuple<Texture?, Texture?> textures, ref Entity entity,
            double depth_v, double depth_h,
            double hx, double vy,
            double car_angle)
        {
            if (depth_v < depth_h)
            {
                Offset = vy;
                Depth = depth_v;
                Texture = textures.Item1;
            }
            else
            {
                Offset = hx;
                Depth = depth_h;
                Texture = textures.Item2;
            }
            Depth *= Math.Cos(entity.getEntityA() - car_angle);
        }

        public void calculationSettingRender(ref Screen screen, ref Entity entity,
            ref ValueTuple<Texture?, Texture?> textures,
            double depth_v, double depth_h,
            double hx, double vy,
            double car_angle)
        {
            redefinitionValues(ref textures, ref entity, depth_v, depth_h, hx, vy, car_angle);
            

            Offset = (int)Offset % screen.setting.Tile;
            Depth = Math.Max(Depth, 0.1);
            ProjHeight = Math.Min((int)(entity.ProjCoeff / Depth), 6 * screen.ScreenHeight);
        }
    }
}
