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

namespace RayTracingLib.Detection
{
    static public class RayDetectionY
    {
        const float addMultFullScreen = 1.3f;


        const float baseMultNegativeCoo = 2.6f;
        const float baseMultNegativeCornerCoo = 2.4f;

        const float baseMultPozititiveCoo = 2.5f;
        const float baseMultPozititiveCornerCoo = 2.1f;


        const float distanceLimitation = 1.2f;
        const float distanceLimitationValue = 1f;


        private static bool IsCornerWall(HitPoint hitPoint)
        {
            return hitPoint.WallDetermine == TextureWallSide.LeftCorner ||
                   hitPoint.WallDetermine == TextureWallSide.RightCorner ||
                   hitPoint.WallDetermine == TextureWallSide.BottomCorner ||
                   hitPoint.WallDetermine == TextureWallSide.TopCorner;
        }
        private static float GetAveragedMult(TexturedWall wall, float baseMult)
        {
            if (wall.CurrentRenderTexture is null)
                throw new Exception("CurrentRenderTexture is null(GetAveragedMult)");


            float newMult = baseMult * Screen.MultHeight;
            newMult *= (float)TextureObstacle.BaseHeight / (float)wall.CurrentRenderTexture.Base.Height;

            if (Screen.Styles == Styles.Fullscreen)
                return newMult + addMultFullScreen;

            return newMult;
        }
       
        
        private static float CalculateNegativeMult(HitPoint hitPoint, TexturedWall wall, float radius, ref float addCoordinates)
        {
            float safeDistance = hitPoint.DistanceToWall <= distanceLimitation ? 
                Math.Min(hitPoint.DistanceToWall, distanceLimitationValue) :
                hitPoint.DistanceToWall;


            float baseMult = 1;
            if (!IsCornerWall(hitPoint))
            {
                addCoordinates = radius;
                baseMult = GetAveragedMult(wall, baseMultNegativeCoo);
            }
            else
                baseMult = GetAveragedMult(wall, baseMultNegativeCornerCoo);

            return (safeDistance * safeDistance) / (baseMult * safeDistance * (1 / hitPoint.DistanceToPoint));
        }
        private static float CalculatePozititiveMult(HitPoint hitPoint, TexturedWall wall, Entity entity, float radius, ref float addCoordinates)
        {
            float safeDistance = hitPoint.DistanceToWall <= distanceLimitation ?
                Math.Min(hitPoint.DistanceToWall, distanceLimitationValue) :
                hitPoint.DistanceToWall;

            if (IsCornerWall(hitPoint))
            {
                float baseCornerMult = GetAveragedMult(wall, baseMultPozititiveCornerCoo);

                float baseCornerValue = baseCornerMult * safeDistance * (1 / hitPoint.DistanceToPoint);
                baseCornerValue = safeDistance * safeDistance / baseCornerValue;

                return baseCornerValue / (float)Math.Max(entity.VerticalAngle + MoveLib.Setting.MaxVerticalAngle, 0.1f);
            }


            float baseMult = GetAveragedMult(wall, baseMultPozititiveCoo);
            addCoordinates = radius;

            float baseValue = baseMult * safeDistance * (1 / hitPoint.DistanceToPoint);
            baseValue = (safeDistance * safeDistance) / baseValue;

            return baseValue / (float)Math.Max(entity.VerticalAngle + 1, 0.1f);
        }
       
        
        private static float CalculateMultY(HitPoint hitPoint, TexturedWall wall, Entity entity, float radius, ref float addCoordinates)
        {
            if (entity.VerticalAngle <= 0f)
                return CalculateNegativeMult(hitPoint, wall, radius, ref addCoordinates);
            else
                return CalculatePozititiveMult(hitPoint, wall, entity, radius, ref addCoordinates);
        }


        private static float CalculateTextureY(TexturedWall wall, Entity entity,
            float ProjHeight, float mult, float addCoordinates)
        {
            if (wall.CurrentRenderTexture is null)
                throw new Exception("CurrentRenderTexture is null(GetAveragedMult)");

            float textureY = ProjHeight * (float)entity.VerticalAngle * mult;
            return wall.CurrentRenderTexture.Base.HulfHeight + textureY - addCoordinates;
        }
        public static float GetTextureCoordinate(HitPoint hitPoint, TexturedWall wall, Entity entity, float radius)
        {
            float addCoordinates = 0;
            float ProjHeight = (float)entity.ProjCoeff / hitPoint.DistanceToWallWithoutTile;
            //Console.WriteLine(hitPoint.DistanceToWall);
            float mult = CalculateMultY(hitPoint, wall, entity, radius, ref addCoordinates);
            return CalculateTextureY(wall, entity, ProjHeight, mult, addCoordinates);
        }
    }
}
