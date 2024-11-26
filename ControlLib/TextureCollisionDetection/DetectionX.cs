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
    internal class DetectionX(Screen screen, WallInfo wallInfo, EntityInfo entityInfo, DetectionActionInfo detectionInfo)
    {
        float tempValue = float.MaxValue;


        private Vector2f CalculateTextureHitPoint()
        {
            if (entityInfo.CosAngle != 0)
            {
                float tVerticalLeft = (wallInfo.Left - entityInfo.X) / entityInfo.CosAngle;
                float tVerticalRight = (wallInfo.Right - entityInfo.X) / entityInfo.CosAngle;

                float hitYLeft = entityInfo.Y + tVerticalLeft * entityInfo.SinAngle;
                float hitYRight = entityInfo.Y + tVerticalRight * entityInfo.SinAngle;

                if (hitYLeft >= wallInfo.Top && hitYLeft <= wallInfo.Bottom && tVerticalLeft >= 0)
                {
                    tempValue = tVerticalLeft;
                }

                if (hitYRight >= wallInfo.Top && hitYRight <= wallInfo.Bottom && tVerticalRight >= 0)
                {
                    tempValue = Math.Min(tempValue, tVerticalRight);
                }
            }

            if (entityInfo.SinAngle != 0)
            {
                float tHorizontalTop = (wallInfo.Top - entityInfo.Y) / entityInfo.SinAngle;
                float tHorizontalBottom = (wallInfo.Bottom - entityInfo.Y) / entityInfo.SinAngle;

                float hitXTop = entityInfo.X + tHorizontalTop * entityInfo.CosAngle;
                float hitXBottom = entityInfo.X + tHorizontalBottom * entityInfo.CosAngle;

                if (hitXTop >= wallInfo.Left && hitXTop <= wallInfo.Right && tHorizontalTop >= 0)
                {
                    tempValue = Math.Min(tempValue, tHorizontalTop);
                }

                if (hitXBottom >= wallInfo.Left && hitXBottom <= wallInfo.Right && tHorizontalBottom >= 0)
                {
                    tempValue = Math.Min(tempValue, tHorizontalBottom);
                }
            }


            if (tempValue == float.MaxValue)
            {
                return new Vector2f(-1, -1);
            }

            float hitX = (float)(entityInfo.X + tempValue * entityInfo.CosAngle) - (int)wallInfo.X;
            float hitY = (float)(entityInfo.Y + tempValue * entityInfo.SinAngle) - (int)wallInfo.Y;


            detectionInfo.DistanceToPoint = tempValue / screen.Setting.Tile;
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

        private void SetTextureWall(TextureWallSide wallTexture)
        {
            if (detectionInfo.WallDetermine == TextureWallSide.Error)
                detectionInfo.WallDetermine = wallTexture;
        }
        public void calculateTextureHitPoint(TexturedWall wall)
        {
            float deltaX = wallInfo.X - entityInfo.X;
            float deltaY = wallInfo.Y - entityInfo.Y;
            detectionInfo.DistanceToWall = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (detectionInfo.WallDetermine == TextureWallSide.Top || detectionInfo.WallDetermine == TextureWallSide.Left)
                detectionInfo.DistanceToWall -= -(screen.Setting.Tile * 3);

            Vector2f cornerHit = CalculateTextureHitPoint();

            detectionInfo.WallDetermine = DetermineWallSide();
            if (cornerHit.X > cornerHit.Y)
            {
                cornerHit.X /= 100;
                if (cornerHit.X < cornerHit.Y)
                {
                    cornerHit.Y /= 100;
                    cornerHit.X = 0;
                    SetTextureWall(TextureWallSide.LeftCorner);
                    detectionInfo.TextureWallDetermine = TextureWallSide.Left;
                }
                else
                {
                    SetTextureWall(TextureWallSide.BottomCorner);
                    detectionInfo.TextureWallDetermine = TextureWallSide.Bottom;
                }
            }
            else
            {
                cornerHit.Y /= 100;
                if (cornerHit.X > cornerHit.Y)
                {
                    cornerHit.X /= 100;
                    cornerHit.Y = 0;
                    SetTextureWall(TextureWallSide.TopCorner);
                    detectionInfo.TextureWallDetermine = TextureWallSide.Top;
                }
                else
                {
                    SetTextureWall(TextureWallSide.RightCorner);
                    detectionInfo.TextureWallDetermine = TextureWallSide.Right;
                }
            }

            wall.CurrentTexture = wall.RenderTextures.GetTexture(detectionInfo.TextureWallDetermine);
            detectionInfo.Texture_X_Coordinate = getTextureCoordinate(cornerHit, wall);
        }

        private float getTextureCoordinate(Vector2f cornerHit, TexturedWall wall)
        {
            float textureX = cornerHit.X > cornerHit.Y ? cornerHit.X : cornerHit.Y;
            textureX *= (wall.TextureObst.TextureWidth) / (screen.Setting.Scale);

            return textureX - (float)Math.Pow((wallInfo.TextureHeight / wallInfo.BaseTextureHeight), 4.5f);
        }
    }
}
