using EntityLib;
using MapLib.Obstacles.DiversityObstacle;
using MapLib.Obstacles.Texture;
using MapLib;
using ScreenLib;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlLib.TextureCollisionDetection
{
    internal class DetectionY(Screen screen, WallInfo wallInfo, EntityInfo entityInfo, DetectionActionInfo detectionInfo)
    {
        private bool IsCornerWall()
        {
            return wallDetermine == TextureWallSide.LeftCorner ||
                   wallDetermine == TextureWallSide.RightCorner ||
                   wallDetermine == TextureWallSide.BottomCorner ||
                   wallDetermine == TextureWallSide.TopCorner;
        }


        private float getMult(float baseMult)
        {
            float newMult = baseMult * (baseScreenHeight / screen.ScreenHeight) * (baseTextureHeight / wallTextureHeight);

            if (screen.Styles == Styles.Fullscreen)
                return newMult + 1.3f;

            return newMult;
        }

        private float calculateNegativeYCoo(float adjustedDistance, float radius, ref float addCoordinates)
        {
            float baseMult = getMult(2.5f);


            float safeDistance = Math.Max(adjustedDistance, 0.5f);

            if (IsCornerWall())
                return (safeDistance * safeDistance) / ((baseMult * safeDistance) * (1 / distancePoint));

            if (adjustedDistance >= 1)
                addCoordinates = radius / safeDistance;
            else
                addCoordinates = getMult(radius);

            return (safeDistance * safeDistance) / ((baseMult * safeDistance) * (1 / distancePoint));
        }

        private float calculatePozititiveYCoo(float adjustedDistance, float entityVertAngle, float radius, ref float addCoordinates)
        {
            float baseMult = getMult(2.7f);
            float safeDistance = Math.Max(adjustedDistance, 0.5f);

            if (IsCornerWall())
            {
                float baseCornerMult = getMult(2f);
                float temp = (safeDistance * safeDistance) / ((baseCornerMult * safeDistance) * (1 / distancePoint));
                return temp / Math.Max(entityVertAngle + (float)(setting.maxVerticalAngle), 0.1f);
            }

            if (adjustedDistance < 1)
                addCoordinates = getMult(radius);
            else
                addCoordinates = radius / safeDistance;

            float baseValue = (safeDistance * safeDistance) / ((baseMult * safeDistance) * (1 / distancePoint));
            return baseValue / Math.Max(entityVertAngle + 1, 0.1f);
        }



        private float calculateMultY(float adjustedDistance, float entityVertAngle, float radius, ref float addCoordinates)
        {
            if (entityVertAngle <= 0f)
                return calculateNegativeYCoo(adjustedDistance, radius, ref addCoordinates);
            else
                return calculatePozititiveYCoo(adjustedDistance, entityVertAngle, radius, ref addCoordinates);
        }


        private float calculateTextureY(TexturedWall wall,
            float ProjHeight, float entityVertAngle,
            float mult, float addCoordinates)
        {
            float textureY = ProjHeight * entityVertAngle * mult;

            return wall.TextureObst.TextureHeight / 2 + textureY - addCoordinates;
        }
        private float getTextureY(HitPoint hitPoint, TexturedWall wall, float radius)
        {
            entityVertAngle = (float)entity.getEntityVerticalA();

            float addCoordinates = 0;

            float adjustedDistance = (float)hitPoint.Distance / screen.Setting.Tile;

            float ProjHeight = (float)entity.ProjCoeff / (float)hitPoint.Distance;


            float mult = calculateMultY(adjustedDistance, entityVertAngle, radius, ref addCoordinates);

            return calculateTextureY(wall, ProjHeight, entityVertAngle, mult, addCoordinates);
        }
    }
}
