using EntityLib;
using Render.RenderInterface;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SFML.Window.Mouse;
using ScreenLib;

namespace ObstacleLib.SpriteLib.Hitbox
{
    internal static class CollisionHitbox
    {
        static double[] ZValues = { 46.296295, 92.59259, 138.88889, 185.18518, 231.48148, 277.77777, 324.07407 };
        static double[] topOffsets = { -1.15, -1.35, -1.45, -1.55, -1.63, -1.7, -1.77 };
        static double[] bottomOffsets = { -0.8, -1.15, -1.35, -1.49, -1.59, -1.67, -1.74 };
        const double baseMultTexture = 0.47253788;

        const double zeroTopMult = 0.5;
        const double negativeTopMult = 0.1;
        const double pozitiveTopMult = 0.06;
        const double highestTopObjectMult = 0.385;

        const double zeroBottomMult = 0.05;
        const double negativeBottomMult = 0.6;
        const double pozitiveBottomMult = 0.06;
        const double highestBottomObjectMult = 0.375;
        const double trueBottomObjectMult = 0.01;



        public static bool IsRayTouchesObjectX(SpriteObstacle sprite, Entity entity, float currentRayX)
        {
            return (int)currentRayX > sprite.Left && (int)currentRayX < sprite.Right;
        }

        public static bool IsRayTouchesObjectY(SpriteObstacle sprite, Entity entity, float currentRayY)
        {
            return (int)currentRayY > sprite.Top && (int)currentRayY < sprite.Bottom;
        }

        public static bool IsRayTouchesObjectZ(SpriteObstacle sprite, Entity entity)
        {
            if (sprite.CurrentRenderTexture is null)
                return true;

            float multTexture = IRayPassability.BaseRayPassObjectHeight / sprite.CurrentRenderTexture.Height;


            float distance = (float)Math.Sqrt(Math.Pow(sprite.X - entity.X, 2) + Math.Pow(sprite.Y - entity.Y, 2)) / Screen.Setting.Tile;      
            float Bottom = (sprite.Z - sprite.Scale / 2) / distance;
            float Top = (sprite.Z + sprite.Scale / 2) / distance;


            
            double playerTopAngle = Math.Atan(-entity.VerticalAngle * distance) + sprite.Z / 1000;
            playerTopAngle += Top + CalculateTopOffset(sprite.Z, multTexture);

            double playerBottomAngle = Math.Atan(-entity.VerticalAngle * distance) + sprite.Z / 1000;
            playerBottomAngle += Bottom + CalculateOffsetBotom(sprite.Z);


            if (playerBottomAngle >= Bottom && playerTopAngle <= Top)
                return true;
            else
                return false;
        }
        static double CalculateTopOffset(double Z, double multTexture )
        {
            double multOffsets = (multTexture - baseMultTexture) / 50;

            for (int i = 0; i < ZValues.Length - 1; i++)
            {
                if (Z >= ZValues[i] && Z <= ZValues[i + 1])
                {
                    double k = (Z - ZValues[i]) / (ZValues[i + 1] - ZValues[i]);
                    return (topOffsets[i] - multOffsets) + k * (topOffsets[i + 1] - topOffsets[i] - multOffsets);
                }
            }

            if (Z < ZValues[0])
            {
                double k = (Z - ZValues[0]) / (ZValues[1] - ZValues[0]);
                double offset = (topOffsets[0] - multOffsets) + k * (topOffsets[1] - topOffsets[0] - multOffsets);

                if (Z == 0)
                    offset += zeroTopMult / multOffsets;
                else if (Z < 0)
                    offset += negativeTopMult - Z / 100 - (multOffsets - baseMultTexture) / 10;
                else
                    offset += (multOffsets - baseMultTexture) / 10 - pozitiveTopMult - Z / 1000;

                return offset;
            }
            else
            {
                double k = (Z - ZValues[ZValues.Length - 2]) / (ZValues[ZValues.Length - 1] - ZValues[ZValues.Length - 2]);
                double offset = (topOffsets[topOffsets.Length - 2] - multOffsets) + k * (topOffsets[topOffsets.Length - 1] - topOffsets[topOffsets.Length - 2] - multOffsets);

                offset += highestTopObjectMult - Z / 1000;
                return offset;
            }
        }
        static double CalculateOffsetBotom(double Z)
        {
            for (int i = 0; i < ZValues.Length - 1; i++)
            {
                if (Z >= ZValues[i] && Z <= ZValues[i + 1])
                {
                    double k = (Z - ZValues[i]) / (ZValues[i + 1] - ZValues[i]);
                    double offset = bottomOffsets[i] + k * (bottomOffsets[i + 1] - bottomOffsets[i]);
                    return offset - trueBottomObjectMult;
                }
            }

            if (Z < ZValues[0])
            {
                double k = (Z - ZValues[0]) / (ZValues[1] - ZValues[0]);
                double offset = bottomOffsets[0] + k * (bottomOffsets[1] - bottomOffsets[0]);
                if (Z == 0)
                    offset += zeroTopMult;
                if (Z < 0)
                    offset += negativeBottomMult - Z / 100;
                else
                    offset += pozitiveBottomMult - Z / 1000;

                return offset;
            }
            else
            {
                double k = (Z - ZValues[ZValues.Length - 2]) / (ZValues[ZValues.Length - 1] - ZValues[ZValues.Length - 2]);
                double offset = bottomOffsets[bottomOffsets.Length - 2] + k * (bottomOffsets[bottomOffsets.Length - 1] - bottomOffsets[bottomOffsets.Length - 2]);
                
                offset += highestBottomObjectMult - Z / 1000;
                return offset;
            }
        }
    }
}
