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

namespace RenderLib
{
    internal class RenderObject(RenderLib.Setting setting)
    {
        Texture texture = new Texture(@"D:\C++ проекты\Game2_5D\Wall1.png");

        #region WithoutTexture
        float calcCooX(double ray, int Scale)
        {
            return (float)ray * Scale;
        }

        float calcCooHightY(int ScreenHeight, double projHeight)
        {
            return (float)(ScreenHeight / 2 + projHeight / 2);
        }

        float calcCooLowerY(int ScreenHeight, double projHeight)
        {
            return (float)(ScreenHeight / 2 - projHeight / 2);
        }

        public void renderVertex(ref Screen screen, double ray, double projHeight, double depth)
        {
           // Color color = colorDefinition(30, 200, 30, depth);

            //Vertex v1 = new Vertex(new Vector2f(calcCooX(ray, screen.setting.Scale), calcCooLowerY(screen.ScreenHeight, projHeight)), color);
            //Vertex v2 = new Vertex(new Vector2f(calcCooX(ray, screen.setting.Scale), calcCooHightY(screen.ScreenHeight, projHeight)), color);
            //Vertex v3 = new Vertex(new Vector2f(calcCooX(ray + 1, screen.setting.Scale), calcCooHightY(screen.ScreenHeight, projHeight)), color);
            //Vertex v4 = new Vertex(new Vector2f(calcCooX(ray + 1, screen.setting.Scale), calcCooLowerY(screen.ScreenHeight, projHeight)), color);

            //screen.vertexArray.Append(v1);
            //screen.vertexArray.Append(v2);
            //screen.vertexArray.Append(v3);
            //screen.vertexArray.Append(v4);
        }
        #endregion

        #region WithTexture
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

        private void calculationTextureScale(ref Screen screen, ref Sprite obstacle, ref int textureScale)
        {
            if (setting.Texture is null)
                return;

            float scaleX = (float)textureScale / setting.Texture.Size.X;
            float scaleY = (float)setting.ProjHeight / setting.Texture.Size.Y;
            obstacle.Scale = new Vector2f(scaleX, scaleY);
        }

        private void calculationTexturePosition(ref Screen screen, ref Sprite obstacle, ref int ray, ref double angleVertical)
        {
            obstacle.Position = new Vector2f(ray * screen.setting.Scale, (float)((angleVertical) - setting.ProjHeight / 2));
        }

        private void blackoutTexture(ref Sprite obstacle)
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

            Sprite obstacle = new Sprite(setting.Texture, textureRect);

            blackoutTexture(ref obstacle);
            calculationTextureScale(ref screen, ref obstacle, ref textureScale);
            renderCameraRotation(ref screen, ref entity, ref angleVertical);
            calculationTexturePosition(ref screen, ref obstacle, ref ray, ref angleVertical);

            screen.Window.Draw(obstacle);
        }
        #endregion
    }
}
