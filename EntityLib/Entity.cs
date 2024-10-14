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

        public double MaxDistance { get; private set; }
        public double DeltaAngle { get; init; }


        public double Dist { get; init; }
        public double ProjCoeff { get; init; }


        protected double entityY;
        protected double entityX;
        protected double entityA;
        protected double entityVerticalA;

        public Entity(Setting setting, double maxDistance,
            double entityFov = Math.PI / 3,
            double entityX = 0, double entityY = 0,
            double entityA = 0, double entityVerticalA = 0)
        {
            EntityFov = entityFov;

            this.entityX = entityX <= 0 ? setting.HalfWidth : entityX;
            this.entityY = entityY <= 0 ? setting.HalfHeight : entityY;
            this.entityA = entityA;
            this.entityVerticalA = entityVerticalA;


            HalfFov = (float)entityFov / 2;
            DeltaAngle = (float)entityFov / setting.AmountRays;

            Dist = setting.AmountRays / (2 * (float)Math.Tan(HalfFov));
            ProjCoeff = Dist * setting.Tile;
            MaxDistance = maxDistance;
        }


        public double getEntityY() => entityY;

        public double getEntityX() => entityX;

        public double getEntityA() => entityA;
        public double getEntityVerticalA() => entityVerticalA;

    }
}
