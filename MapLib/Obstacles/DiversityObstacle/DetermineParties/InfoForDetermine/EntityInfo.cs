using EntityLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Obstacles.DiversityObstacle.DetermineParties.InfoForDetermine
{
    public class EntityInfo
    {
        public float X { get; set; } = 0;
        public float Y { get; set; } = 0;

        public float CosAngle { get; set; } = 0;
        public float SinAngle { get; set; } = 0;

        public float Angle { get; set; } = 0;
        public float VertAngle { get; set; } = 0;

        public float ProjCoeff { get; set; } = 0;

        public EntityInfo(Entity entity)
        {
            X = (float)entity.getEntityX();
            Y = (float)entity.getEntityY();

            Angle = (float)entity.getEntityA();
            VertAngle = (float)entity.getEntityVerticalA();

            CosAngle = (float)Math.Cos(Angle);
            SinAngle = (float)Math.Sin(Angle);

            ProjCoeff = (float)entity.ProjCoeff;
        }
        public EntityInfo()
        { }

        public void RefreshData(Entity entity)
        {
            X = (float)entity.getEntityX();
            Y = (float)entity.getEntityY();

            Angle = (float)entity.getEntityA();
            VertAngle = (float)entity.getEntityVerticalA();

            CosAngle = (float)Math.Cos(Angle);
            SinAngle = (float)Math.Sin(Angle);

            ProjCoeff = (float)entity.ProjCoeff;
        }
        public void RefreshData(Entity entity, double angle)
        {
            X = (float)entity.getEntityX();
            Y = (float)entity.getEntityY();

            Angle = (float)angle;
            VertAngle = (float)entity.getEntityVerticalA();

            CosAngle = (float)Math.Cos(angle);
            SinAngle = (float)Math.Sin(angle);

            ProjCoeff = (float)entity.ProjCoeff;
        }
    }
}
