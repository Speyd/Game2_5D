using EntityLib;
using MapLib.Obstacles.DiversityObstacle.DetermineParties.InfoForDetermine;
using MapLib.Obstacles.DiversityObstacle.TexturedWallLib;
using MapLib.Obstacles.Texture;
using Render.InterfaceRender;
using ScreenLib;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Obstacles.DiversityObstacle.DetermineParties
{
    public class DetermineWallParties(EntityInfo entityInfo, WallInfo wallInfo)
    {
        float distanceToPoint = 0;
        float distanceToWall = 0;
        private Vector2f CalculateTextureHitPoint()
        {
            float tempValue = float.MaxValue;

            if (entityInfo.CosAngle != 0)
            {
                float tVerticalLeft = (wallInfo.Left - entityInfo.X) / entityInfo.CosAngle;
                if (tVerticalLeft >= 0)
                {
                    float hitYLeft = entityInfo.Y + tVerticalLeft * entityInfo.SinAngle;
                    if (hitYLeft >= wallInfo.Top && hitYLeft <= wallInfo.Bottom)
                        tempValue = tVerticalLeft;
                }

                float tVerticalRight = (wallInfo.Right - entityInfo.X) / entityInfo.CosAngle;
                if (tVerticalRight >= 0)
                {
                    float hitYRight = entityInfo.Y + tVerticalRight * entityInfo.SinAngle;
                    if (hitYRight >= wallInfo.Top && hitYRight <= wallInfo.Bottom)
                        tempValue = Math.Min(tempValue, tVerticalRight);
                }
            }
            if (entityInfo.SinAngle != 0)
            {
                float tHorizontalTop = (wallInfo.Top - entityInfo.Y) / entityInfo.SinAngle;
                if (tHorizontalTop >= 0)
                {
                    float hitXTop = entityInfo.X + tHorizontalTop * entityInfo.CosAngle;
                    if (hitXTop >= wallInfo.Left && hitXTop <= wallInfo.Right)
                        tempValue = Math.Min(tempValue, tHorizontalTop);
                }

                float tHorizontalBottom = (wallInfo.Bottom - entityInfo.Y) / entityInfo.SinAngle;
                if (tHorizontalBottom >= 0)
                {
                    float hitXBottom = entityInfo.X + tHorizontalBottom * entityInfo.CosAngle;
                    if (hitXBottom >= wallInfo.Left && hitXBottom <= wallInfo.Right)
                        tempValue = Math.Min(tempValue, tHorizontalBottom);
                }
            }

            if (tempValue == float.MaxValue)
                return new Vector2f(-1, -1);

            float hitX = entityInfo.X + tempValue * entityInfo.CosAngle - wallInfo.X;
            float hitY = entityInfo.Y + tempValue * entityInfo.SinAngle - wallInfo.Y;
            distanceToPoint = tempValue / Screen.Setting.Tile;

            return new Vector2f(hitX, hitY);
        }

        private TextureWallSide DetermineWallSide()
        {
            if (entityInfo.Y >= wallInfo.Top && entityInfo.Y <= wallInfo.Bottom)
            {
                if (entityInfo.CosAngle > 0 && entityInfo.X <= wallInfo.Right)
                    return TextureWallSide.Right;
                else if (entityInfo.CosAngle < 0 && entityInfo.X >= wallInfo.Left)
                    return TextureWallSide.Left;
            }

            if (entityInfo.X >= wallInfo.Left && entityInfo.X <= wallInfo.Right)
            {
                if (entityInfo.SinAngle > 0 && entityInfo.Y <= wallInfo.Bottom)
                    return TextureWallSide.Bottom;
                else if (entityInfo.SinAngle < 0 && entityInfo.Y >= wallInfo.Top)
                    return TextureWallSide.Top;
            }

            return TextureWallSide.Error;
        }
        private void SetTextureWall(ref TextureWallSide oldWallTexture, TextureWallSide newWallTexture)
        {
            if (oldWallTexture == TextureWallSide.Error)
                oldWallTexture = newWallTexture;
        }
        private TextureWallSide RedefiningWallSides(TextureWallSide wallDetermine)
        {
            switch (wallDetermine)
            {
                case TextureWallSide.LeftCorner:
                    return TextureWallSide.Left;
                case TextureWallSide.RightCorner:
                    return TextureWallSide.Right;
                case TextureWallSide.TopCorner:
                    return TextureWallSide.Top;
                case TextureWallSide.BottomCorner:
                    return TextureWallSide.Bottom;
                default:
                    return wallDetermine;
            }
        }
        public HitPoint DetermineWallAllSides(TexturedWall wall)
        {
            TextureWallSide wallDetermine = TextureWallSide.Error;
            wallDetermine = DetermineWallSide();

            float deltaX = wallInfo.X - entityInfo.X;
            float deltaY = wallInfo.Y - entityInfo.Y;
            distanceToWall = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (wallDetermine == TextureWallSide.Top || wallDetermine == TextureWallSide.Left)
                distanceToWall -= -(Screen.Setting.Tile * 3);


            Vector2f cornerHit = CalculateTextureHitPoint();
            if (cornerHit.X > cornerHit.Y)
            {
                cornerHit.X /= 100;
                if (cornerHit.X < cornerHit.Y)
                {
                    cornerHit.Y /= 100;
                    cornerHit.X = 0;
                    SetTextureWall(ref wallDetermine, TextureWallSide.LeftCorner);
                }
                else
                    SetTextureWall(ref wallDetermine, TextureWallSide.BottomCorner);
            }
            else
            {
                cornerHit.Y /= 100;
                if (cornerHit.X > cornerHit.Y)
                {
                    cornerHit.X /= 100;
                    cornerHit.Y = 0;
                    SetTextureWall(ref wallDetermine, TextureWallSide.TopCorner);
                }
                else
                    SetTextureWall(ref wallDetermine, TextureWallSide.RightCorner);
            }

            TextureWallSide textureWallDetermine = RedefiningWallSides(wallDetermine);
            return new HitPoint(cornerHit, distanceToPoint, distanceToWall, wallDetermine, textureWallDetermine);
        }


        public void RefreshData(Entity entity, TexturedWall wall)
        {
            entityInfo.RefreshData(entity);
            wallInfo.RefreshData(wall);
        }

        public void RefreshData(Entity entity, double angle, TexturedWall wall)
        {
            entityInfo.RefreshData(entity, angle);
            wallInfo.RefreshData(wall);
        }
        public void RefreshData(EntityInfo newEntityInfo, WallInfo newWallInfo)
        {
            entityInfo = newEntityInfo;
            wallInfo = newWallInfo;
        }

        public void RefreshData(Entity entity)
        {
            entityInfo.RefreshData(entity);
        }

        public void RefreshData(Entity entity, double angle)
        {
            entityInfo.RefreshData(entity, angle);
        }
    }
}
