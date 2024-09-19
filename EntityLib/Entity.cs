using System;
using System.Collections.Generic;
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
        public double EntityFov { get; init; }
        public double HalfFov { get; init; }


        public double DeltaAngle { get; init; }


        public double Dist { get; init; }
        public double ProjCoeff { get; init; }


        protected double entityY;
        protected double entityX;
        protected double entityA;

        public Entity(Setting setting,
            double entityFov = Math.PI / 3,
            double entityX = 0, double entityY = 0, 
            double entityA = 0)
        {
            EntityFov = entityFov;

            this.entityX = entityX <= 0 ? setting.HalfWidth : entityX;
            this.entityY = entityY <= 0 ? setting.HalfHeight : entityY;
            this.entityA = entityA;


            HalfFov = entityFov / 2;
            DeltaAngle = entityFov / setting.AmountRays;

            Dist = setting.AmountRays / (2 * Math.Tan(HalfFov));
            ProjCoeff = Dist * setting.Tile;
        }

        
        public double getEntityY()
        {
            return entityY;
        }

        public double getEntityX()
        {
            return entityX;
        }

        public double getEntityA()
        {
            return entityA;
        }

    }
}
