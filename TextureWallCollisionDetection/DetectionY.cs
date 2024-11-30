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
using MapLib.Obstacles.DiversityObstacle.DetermineParties;
using MapLib.Obstacles.DiversityObstacle.DetermineParties.InfoForDetermine;
using SFML.Window;
using SFML.System;
using MapLib.Obstacles.DiversityObstacle.TexturedWallLib;

namespace TextureWallCollisionDetection
{
    internal class DetectionY(Screen screen, double maxVerticalAngle, WallInfo wallInfo, EntityInfo entityInfo, DetectionActionInfo detectionInfo)
    {
        float addMultFullScreen = 1.3f;

        float baseMultNegativeCoo = 2.5f;
        float baseMultPozititiveCoo = 2.7f;
        float baseMultPozititiveCornerCoo = 2f;

        float distanceLimitation = 0.5f;

        private bool IsCornerWall()
        {
            return detectionInfo.WallDetermine == TextureWallSide.LeftCorner ||
                   detectionInfo.WallDetermine == TextureWallSide.RightCorner ||
                   detectionInfo.WallDetermine == TextureWallSide.BottomCorner ||
                   detectionInfo.WallDetermine == TextureWallSide.TopCorner;
        }
        private float GetAveragedMult(float baseMult)
        {
            float newMult = baseMult / (screen.BaseScreenHeight / screen.ScreenHeight);
            newMult *= (wallInfo.BaseTextureHeight / wallInfo.TextureHeight);

            if (screen.Styles == Styles.Fullscreen)
                return newMult + addMultFullScreen;

            return newMult;
        }

        private float CalculateNegativeAngle(float radius, ref float addCoordinates)
        {
            float safeDistance = Math.Max(detectionInfo.DistanceToWall, distanceLimitation);
            float baseMult = GetAveragedMult(baseMultNegativeCoo);

            if (!IsCornerWall())
            {
                if (detectionInfo.DistanceToWall >= 1)
                    addCoordinates = radius / safeDistance;
                else
                    addCoordinates = GetAveragedMult(radius);
            }

            return (safeDistance * safeDistance) / ((baseMult * safeDistance) * (1 / detectionInfo.DistanceToPoint));
        }
        private float CalculatePozititiveAngle(float radius, ref float addCoordinates)
        {
            float safeDistance = Math.Max(detectionInfo.DistanceToWall, distanceLimitation);

            if (IsCornerWall())
            {
                float baseCornerMult = GetAveragedMult(baseMultPozititiveCornerCoo);

                float baseCornerValue = (baseCornerMult * safeDistance) * (1 / detectionInfo.DistanceToPoint);
                baseCornerValue = (safeDistance * safeDistance) / baseCornerValue;

                return baseCornerValue / Math.Max(entityInfo.VertAngle + (float)maxVerticalAngle, 0.1f);
            }


            float baseMult = GetAveragedMult(baseMultPozititiveCoo);

            if (detectionInfo.DistanceToWall < 1)
                addCoordinates = GetAveragedMult(radius);
            else
                addCoordinates = radius / safeDistance;

            float baseValue = (baseMult * safeDistance) * (1 / detectionInfo.DistanceToPoint);
            baseValue = (safeDistance * safeDistance) / baseValue;

            return baseValue / Math.Max(entityInfo.VertAngle + 1, 0.1f);
        }
        private float CalculateMultY(float radius, ref float addCoordinates)
        {
            if (entityInfo.VertAngle <= 0f)
                return CalculateNegativeAngle(radius, ref addCoordinates);
            else
                return CalculatePozititiveAngle(radius, ref addCoordinates);
        }


        private float CalculateTextureY(TexturedWall wall,float ProjHeight, float mult, float addCoordinates)
        {
            float textureY = ProjHeight * entityInfo.VertAngle * mult;

            return wall.BaseTexture.TextureHeight / 2 + textureY - addCoordinates;
        }
        public float GetTextureCoordinate(TexturedWall wall, float radius)
        {
            float addCoordinates = 0;

            float ProjHeight = entityInfo.ProjCoeff / detectionInfo.DistanceToWallWithTile;

            float mult = CalculateMultY(radius, ref addCoordinates);

            return CalculateTextureY(wall, ProjHeight, mult, addCoordinates);
        }
    }
}
