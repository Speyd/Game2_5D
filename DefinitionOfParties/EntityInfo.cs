using EntityLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlLib.TextureCollisionDetection
{
    public class EntityInfo
    {
        public float X { get; set; } = 0;
        public float Y { get; set; } = 0;

        public float CosAngle { get; set; } = 0;
        public float SinAngle { get; set; } = 0;

        public float Angle {  get; set; } = 0;
        public float VertAngle { get; set; } = 0;

        public EntityInfo(Entity entity)
        {
            X = (float)entity.getEntityX();
            Y = (float)entity.getEntityX();

            Angle = (float)entity.getEntityX();
            VertAngle = (float)entity.getEntityX();

            CosAngle = (float)Math.Cos(Angle);
            SinAngle = (float)Math.Sin(Angle);
        }

        public void RefreshData(Entity entity)
        {
            X = (float)entity.getEntityX();
            Y = (float)entity.getEntityX();

            Angle = (float)entity.getEntityX();
            VertAngle = (float)entity.getEntityX();

            CosAngle = (float)Math.Cos(Angle);
            SinAngle = (float)Math.Sin(Angle);
        }
    }
}
