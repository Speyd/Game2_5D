using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using ScreenLib.SettingScreen;
using static System.Formats.Asn1.AsnWriter;
using ScreenLib;
using EntityLib;
using System.IO;
using EntityLib.Player;
using ObstacleLib.ItemObstacle;
using System.Numerics;
using ObstacleLib.Render.Texture;
using ObstacleLib;
using System.Reflection.Metadata;

namespace RenderLib
{
    internal class RenderObject(RenderLib.Setting setting)
    {
        RectangleShape block = new RectangleShape();
        float calcCooX(double ray, Screen screen)
        {
            return (float)ray * screen.setting.Scale;
        }

        private float normalize_Y_Position(Screen screen,  double angleVertical, double addVariable = 0)
        {
            if (angleVertical <= 0)
                return (float)(screen.setting.HalfHeight - (screen.setting.HalfHeight * angleVertical) - addVariable);
            else
            {
                angleVertical += 1;

                return (float)(screen.setting.HalfHeight / angleVertical - addVariable);
            }
        }

        #region Render_Rectangle_Without_Texture
        private void calculationBlockScale(ref Screen screen, BlankWall wall)
        {
            float scaleX = screen.setting.Scale;
            float scaleY = (float)setting.ProjHeight;

            wall.Wall.Size = new Vector2f(scaleX, scaleY);
        }
        private void calculationBlockPosition(ref Screen screen, BlankWall wall, ref int ray, ref double angleVertical)
        {
            float positionX = calcCooX(ray, screen);
            float positionY = normalize_Y_Position(screen, angleVertical, setting.ProjHeight / 2);

            wall.Wall.Position = new Vector2f(positionX, positionY);
        }

        public void renderVertex(ref Screen screen, BlankWall wall, int ray, double angleVertical)
        {
            screen.vertexArray.Clear();

            calculationBlockScale(ref screen, wall);
            calculationBlockPosition(ref screen, wall, ref ray, ref angleVertical);

            wall.blackoutObstacle(setting.Depth);

            screen.Window.Draw(wall.Wall);
        }
        #endregion



        #region Render_Rectangle_With_Texture
        #region Calculation
        private void calculationCameraRotation(ref Screen screen, ref double angleVertical)
        {
            if (angleVertical <= 0)
            {
                angleVertical = (float)(screen.setting.HalfHeight * (1 + 1 * -angleVertical));
            }
            else
            {
                angleVertical = (float)(screen.setting.HalfHeight / (1 + 1 * angleVertical));
            }
        }
        private void calculationTextureScale(TexturedWall wall)
        {
            if (wall.Texture is null)
                return;

            float scaleX = (float)wall.Texture.TextureScale / wall.Texture.TextureWidth;
            float scaleY = (float)setting.ProjHeight / wall.Texture.TextureHeight;
            wall.SpriteObst.Scale = new Vector2f(scaleX, scaleY);
        }
        private void calculationTexturePosition(ref Screen screen, TexturedWall wall,
            int ray, double angleVertical)
        {
            float positionX = calcCooX(ray, screen);
            float positionY = (float)((angleVertical) - setting.ProjHeight / 2);

            wall.SpriteObst.Position = new Vector2f(positionX, positionY);
        }

        #endregion

        public void renderObstacle(ref Screen screen, TexturedWall wall,
            int ray, double angleVertical)
        {
            if (wall.Texture.Texture is null)
                return;

            IntRect textureRect = TextureObstacle.setOffset((int)setting.Offset, screen.setting.Tile, wall.Texture);

            wall.SpriteObst = new SFML.Graphics.Sprite(wall.Texture.Texture, textureRect);
            wall.blackoutObstacle(setting.Depth);

            calculationTextureScale(wall);
            calculationCameraRotation(ref screen, ref angleVertical);
            calculationTexturePosition(ref screen, wall, ray, angleVertical);

            screen.Window.Draw(wall.SpriteObst);
        }
        #endregion



        #region Render_Sprite
        private double calculationSpriteAngle(double playerAngle, double spriteAngle) 
        {
            double angleDifference = spriteAngle - playerAngle;
            if (angleDifference > Math.PI) 
                angleDifference -= 2 * Math.PI;
            if (angleDifference < -Math.PI)
                angleDifference += 2 * Math.PI;

            return angleDifference;
        }
        private void definingDesiredSprite(ObstacleLib.ItemObstacle.Sprite sprite, double spriteAngle)
        {
            double spriteDegreeAngle = spriteAngle * (180.0 / Math.PI);

            if (spriteDegreeAngle < 0)
                spriteDegreeAngle += 360;

            int totalDirections = sprite.Textures.Count;
            if (totalDirections == 0) return;

            double sectorSize = 360.0 / totalDirections;

            int textureIndex = (int)(spriteDegreeAngle / sectorSize) % totalDirections;
            sprite.CurrentTexture = sprite.Textures[(totalDirections - 1 - textureIndex + totalDirections) % totalDirections];
        }
        private double calculationAngularDistance(ObstacleLib.ItemObstacle.Sprite sprite, Entity player)
        {
            double dx = sprite.WallX - player.getEntityX();
            double dy = sprite.WallY - player.getEntityY();
            sprite.Distance = Math.Sqrt(dx * dx + dy * dy);

            double spriteAngle = Math.Atan2(dy, dx);

            return spriteAngle;     
        }
        public void renderSprites(ref Screen screen, Entity player)
        {
           
            foreach (var sprite in ObstacleLib.ItemObstacle.Sprite.spritesToRender)
            {
                double spriteAngle = calculationAngularDistance(sprite, player);
                if (sprite.Distance > player.MaxDistance)
                    continue;

                sprite.Angle = calculationSpriteAngle(player.getEntityA(), spriteAngle);

                if (sprite.Angle < player.EntityFov / 2)
                {
                    definingDesiredSprite(sprite, spriteAngle);

                    int sprite_X_Position = (int)((screen.ScreenWidth / 2) * (1 + sprite.Angle / (player.EntityFov / 2)));

                    double safeDistance = Math.Max(sprite.Distance, 0.1);
                    int spriteHeight = (int)(screen.ScreenHeight / safeDistance * sprite.ScaleMultSprite);

                    if (spriteHeight > 0)
                        drawSprite(screen, sprite, player.getEntityVerticalA(), sprite_X_Position, spriteHeight);
                }
            }
        }

        private void drawSprite(Screen screen, ObstacleLib.ItemObstacle.Sprite sprite, double verticalAngle, int x, int height)
        {
            float scaledHeight = sprite.SpriteObst.GetGlobalBounds().Height;
            float y = normalize_Y_Position(screen, verticalAngle, scaledHeight / 2);

            sprite.SpriteObst = new SFML.Graphics.Sprite(sprite.CurrentTexture.Texture);
            sprite.blackoutObstacle(sprite.Distance);

            sprite.SpriteObst.Position = new Vector2f(x, y);

            sprite.SpriteObst.Scale = new Vector2f
                (
                (float)height / sprite.CurrentTexture.TextureWidth, 
                (float)height / sprite.CurrentTexture.TextureHeight
                );

            screen.Window.Draw(sprite.SpriteObst);
        }

        #endregion
    }
}
