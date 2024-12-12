using EntityLib;
using MapLib.Obstacles.Texture;
using MapLib;
using ScreenLib;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using Render.InterfaceRender;
using MapLib.Obstacles.DiversityObstacle.TexturedWallLib;
using TextureLib;
using Render;

namespace RayTracingLib.Detection
{
    static public class RayDetectionY
    {
        public static float AddMultFullScreen { get; } = 1.3f;
        public static float AddCoordinates { get; private set; } = 0;


        const float baseMultNegativeCoo = 2.6f;

        const float baseMultPozititiveCoo = 2.5f;


        const float distanceLimitation = 1.2f;
        const float distanceLimitationValue = 1f;



        private static bool IsCornerWall(HitPoint hitPoint)
        {
            return hitPoint.WallDetermine == TextureWallSide.LeftCorner ||
                   hitPoint.WallDetermine == TextureWallSide.RightCorner ||
                   hitPoint.WallDetermine == TextureWallSide.BottomCorner ||
                   hitPoint.WallDetermine == TextureWallSide.TopCorner;
        }


        #region Mult
        private static float CalculateNegativeMult(HitPoint hitPoint, IDrawable obst, float heightObj)
        {
            float safeDistance = hitPoint.DistanceToWall <= distanceLimitation ? 
                Math.Min(hitPoint.DistanceToWall, distanceLimitationValue) :
                hitPoint.DistanceToWall;


            float baseMult = obst.GetAveragedMult(baseMultNegativeCoo, AddMultFullScreen);
            AddCoordinates = heightObj;

            return (safeDistance * safeDistance) / (baseMult * safeDistance * (1 / hitPoint.DistanceToPoint));
        }
        private static float CalculatePozititiveMult(HitPoint hitPoint, IDrawable obst, Entity entity, float heightObj)
        {
            float safeDistance = hitPoint.DistanceToWall <= distanceLimitation ?
                Math.Min(hitPoint.DistanceToWall, distanceLimitationValue) :
                hitPoint.DistanceToWall;


            float baseMult = obst.GetAveragedMult(baseMultPozititiveCoo, AddMultFullScreen);          
            AddCoordinates = heightObj;


            float baseValue = (baseMult * safeDistance) * (1 / hitPoint.DistanceToPoint);
            baseValue = (safeDistance * safeDistance) / baseValue;

            return baseValue / (float)Math.Max(entity.VerticalAngle + 1, 0.1f);
        }      
        
        private static float CalculateMult(HitPoint hitPoint, IDrawable obst, Entity entity, float heightObj)
        {
            if (entity.VerticalAngle <= 0f)
                return CalculateNegativeMult(hitPoint, obst, heightObj);
            else
                return CalculatePozititiveMult(hitPoint, obst, entity, heightObj);
        }
        #endregion

        public static float GetTextureCoordinate(HitPoint hitPoint, IDrawable obst, Entity entity, float heightObj)
        {
            AddCoordinates = 0;
            float ProjHeight = (float)entity.ProjCoeff / hitPoint.DistanceToWallWithoutTile;

            float mult = CalculateMult(hitPoint, obst, entity, heightObj);
            return obst.CalculateTextureY(entity, ProjHeight, mult, AddCoordinates);
        }
    }
}
