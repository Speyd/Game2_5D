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

namespace RenderLib
{
    internal class RenderObject(RenderLib.Setting setting)
    {
        RectangleShape block = new RectangleShape();
        SFML.Graphics.Sprite sfmlSprite = new SFML.Graphics.Sprite(new Texture(@"D:\C++ проекты\Game2_5D\tn342.png"));
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
        public Color colorDefinition(byte r, byte g, byte b, double depth)
        {
            byte darkened = (byte)(255 / (1 + depth * depth * 0.00001));

            return new Color((byte)(darkened / 2), darkened, (byte)(darkened / 3));
        }

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

        public void renderVertex(ref Screen screen, int ray, double projHeight, double depth, double angleVertical)
        {
            screen.vertexArray.Clear();

            Color color = colorDefinition(30, 200, 30, depth);

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

        private void calculationTextureScale(ref Screen screen, ref SFML.Graphics.Sprite obstacle, ref int textureScale)
        {
            if (setting.Texture is null)
                return;

            float scaleX = (float)textureScale / setting.Texture.Size.X;
            float scaleY = (float)setting.ProjHeight / setting.Texture.Size.Y;
            obstacle.Scale = new Vector2f(scaleX, scaleY);
        }

        private void calculationTexturePosition(ref Screen screen, ref SFML.Graphics.Sprite obstacle, ref int ray, ref double angleVertical)
        {
            float positionX = calcCooX(ray, screen);
            float positionY = (float)((angleVertical) - setting.ProjHeight / 2);

            obstacle.Position = new Vector2f(positionX, positionY);
        }

        private void blackoutTexture(ref SFML.Graphics.Sprite obstacle)
        {
            byte darknessFactor = (byte)(255 / (1 + setting.Depth * setting.Depth * 0.00001));
            obstacle.Color = new Color(darknessFactor, darknessFactor, darknessFactor);
        }

        public void renderObstacle(ref Screen screen, ref Entity entity, int ray, double angleVertical)
        {
            if (setting.Texture is null)
                return;

            int textureScale = (int)(setting.Texture.Size.X / screen.setting.Tile);
            IntRect textureRect = new IntRect((int)setting.Offset * textureScale, 0, screen.setting.Tile, (int)setting.Texture.Size.Y);

            SFML.Graphics.Sprite obstacle = new SFML.Graphics.Sprite(setting.Texture, textureRect);

            blackoutTexture(ref obstacle);
            calculationTextureScale(ref screen, ref obstacle, ref textureScale);
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
        public void renderSprites(Screen screen, Entity player, List<ObstacleLib.SpriteLib.Sprite> Sprites)
        {
            foreach (var sprite in Sprites)
            {
                double dx = sprite.X - player.getEntityX();
                double dy = sprite.Y - player.getEntityY();
                sprite.Distance = Math.Sqrt(dx * dx + dy * dy);

                double spriteAngle = Math.Atan2(dy, dx);
                sprite.Angle = calculationSpriteAngle(player.getEntityA(), spriteAngle);

                
                if (sprite.Angle < player.EntityFov / 2)
                {
                    int spriteScreenX = (int)((screen.ScreenWidth / 2) * (1 + sprite.Angle / (player.EntityFov / 2)));

                    int originalSpriteHeight = 64;
                    int spriteHeight = (int)((originalSpriteHeight * screen.ScreenHeight / sprite.Distance));

                    if (spriteHeight > 0 && spriteHeight < screen.ScreenHeight)
                        drawSprite(screen, player,  sprite.Texture, spriteScreenX, spriteHeight);
                }
            }
        }
        private void drawSprite(Screen screen, Entity player, Texture texture, int x, int height)
        {
            float y = normalize_Y_Position(screen, player.getEntityVerticalA());

            sfmlSprite.Position = new Vector2f(x, (float)y);
            sfmlSprite.Scale = new Vector2f((float)height / texture.Size.X, (float)height / texture.Size.Y);
            screen.Window.Draw(sfmlSprite);
        }

        #endregion
    }
}
