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
using ObstacleLib.SpriteLib;
using System.Numerics;
using ObstacleLib.Texture;
using ObstacleLib;

namespace RenderLib
{
    internal class RenderObject(RenderLib.Setting setting)
    {
        RectangleShape block = new RectangleShape();
        float calcCooX(double ray, Screen screen)
        {
            return (float)ray * screen.setting.Scale;
        }

        private float normalize_Y_Position(Screen screen, double angleVertical, double addVariable = 0)
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
        private void calculationBlockScale(ref Screen screen, ref double projHeight)
        {
            float scaleX = (float)screen.setting.Scale;
            float scaleY = (float)projHeight;

            block.Size = new Vector2f(scaleX, scaleY);
        }
        private void calculationBlockPosition(ref Screen screen, ref int ray, ref double angleVertical)
        {
            float positionX = calcCooX(ray, screen);
            float positionY = normalize_Y_Position(screen, angleVertical, setting.ProjHeight / 2);

            block.Position = new Vector2f(positionX, positionY);
        }

        public void renderVertex(ref Screen screen, Color color, int ray, double projHeight, double depth, double angleVertical)
        {
            screen.vertexArray.Clear();
            color = BlankWall.blackoutColor(color, depth);

            calculationBlockScale(ref screen, ref projHeight);
            calculationBlockPosition(ref screen, ref ray, ref angleVertical);

            block.FillColor = color;

            screen.Window.Draw(block);
        }
        #endregion

        #region Render_Rectangle_With_Texture
        private void renderCameraRotation(ref Screen screen, ref Entity entity, ref double angleVertical)
        {
            if (entity.getEntityVerticalA() > 0)
            {
                angleVertical = (float)(screen.setting.HalfHeight / (1 + 1 * entity.getEntityVerticalA()));
            }
            else if (entity.getEntityVerticalA() < 0)
            {
                angleVertical = (float)(screen.setting.HalfHeight * (1 + 1 * -entity.getEntityVerticalA()));
            }
            else
            {
                angleVertical = screen.setting.HalfHeight;
            }
        }

        private void calculationTextureScale(ref Screen screen, TextureObstacle texture, ref SFML.Graphics.Sprite obstacle, ref int textureScale)
        {
            if (texture.Texture is null)
                return;

            float scaleX = (float)textureScale / texture.TextureWidth;
            float scaleY = (float)setting.ProjHeight / texture.TextureHeight;
            obstacle.Scale = new Vector2f(scaleX, scaleY);
        }

        private void calculationTexturePosition(ref Screen screen, ref SFML.Graphics.Sprite obstacle, ref int ray, ref double angleVertical)
        {
            float positionX = calcCooX(ray, screen);
            float positionY = (float)((angleVertical) - setting.ProjHeight / 2);

            obstacle.Position = new Vector2f(positionX, positionY);
        }

        public void renderObstacle(ref Screen screen, ref Entity entity, TextureObstacle texture, int ray, double angleVertical)
        {
            if (texture.Texture is null)
                return;

            int textureScale = (int)(texture.TextureWidth / screen.setting.Tile);
            IntRect textureRect = new IntRect((int)setting.Offset * textureScale, 0, screen.setting.Tile, (int)texture.TextureHeight);

            SFML.Graphics.Sprite obstacle = new SFML.Graphics.Sprite(texture.Texture, textureRect);

            TextureObstacle.blackoutTexture(ref obstacle, setting.Depth);
            calculationTextureScale(ref screen, texture, ref obstacle, ref textureScale);
            renderCameraRotation(ref screen, ref entity, ref angleVertical);
            calculationTexturePosition(ref screen, ref obstacle, ref ray, ref angleVertical);

            screen.Window.Draw(obstacle);
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

        private void definingDesiredSprite(ObstacleLib.SpriteLib.Sprite sprite, double spriteAngle)
        {
            double spriteDegreeAngle = spriteAngle * (180.0 / Math.PI);

            if (spriteDegreeAngle < 0)
                spriteDegreeAngle += 360;

            int totalDirections = sprite.Textures.Count;
            if (totalDirections == 0) return;

            double sectorSize = 360.0 / totalDirections;

            int textureIndex = (int)(spriteDegreeAngle / sectorSize) % totalDirections;
            sprite.CurrentTextureIndex = (totalDirections - 1 - textureIndex + totalDirections) % totalDirections;
        }
        private double calculationAngularDistance(ObstacleLib.SpriteLib.Sprite sprite, Entity player)
        {
            double dx = sprite.X - player.getEntityX();
            double dy = sprite.Y - player.getEntityY();
            sprite.Distance = Math.Sqrt(dx * dx + dy * dy);

            double spriteAngle = Math.Atan2(dy, dx);

            return spriteAngle;     
        }
        public void renderSprites(Screen screen, Entity player, List<ObstacleLib.SpriteLib.Sprite> Sprites)
        {
            foreach (var sprite in Sprites)
            {
                double spriteAngle = calculationAngularDistance(sprite, player);
                sprite.Angle = calculationSpriteAngle(player.getEntityA(), spriteAngle);

                definingDesiredSprite(sprite, spriteAngle);

                if (sprite.Angle < player.EntityFov / 2)
                {
                    int sprite_X_Position = (int)((screen.ScreenWidth / 2) * (1 + sprite.Angle / (player.EntityFov / 2)));
                    int spriteHeight = (int)((sprite.SizeMultiplier * screen.ScreenHeight / sprite.Distance));

                    if (spriteHeight > 0 && spriteHeight < screen.ScreenHeight)
                        drawSprite(screen, player.getEntityVerticalA(), 
                            sprite.Textures[sprite.CurrentTextureIndex].Texture,
                            sprite_X_Position, spriteHeight);
                }
            }
        }
        private void drawSprite(Screen screen, double verticalAngle, Texture texture, int x, int height)
        {
            float y = normalize_Y_Position(screen, verticalAngle);

            SFML.Graphics.Sprite sfmlSprite = new SFML.Graphics.Sprite(texture);
            sfmlSprite.Position = new Vector2f(x, y);
            sfmlSprite.Scale = new Vector2f((float)height / texture.Size.X, (float)height / texture.Size.Y);

            screen.Window.Draw(sfmlSprite);
        }

        #endregion
    }
}
