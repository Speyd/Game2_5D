using EntityLib;
using ObstacleLib;
using ObstacleLib.Render;
using ScreenLib;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Render;
using Render.ResultAlgorithm;
using SFML.System;

namespace MapLib.Obstacles.Diversity_Obstacle
{
    public class BlankWall : Obstacle, IWall
    {

        public Color ColorFilling { get; set; }
        public RectangleShape Wall { get; private set; } = new RectangleShape();

        public BlankWall(double x, double y,
            char symbol, Color colorInMap,
            Color color, bool isPassability = false)

            : base(x, y, symbol, colorInMap, isPassability)
        {
            ColorFilling = color;
        }

        public BlankWall(double x, double y, char symbol,
            byte r, byte g, byte b,
            Color colorInMap, bool isPassability = false)

            : base(x, y, symbol, colorInMap, isPassability)
        {
            ColorFilling = new Color(r, g, b);
        }


        public override void blackoutObstacle(double depth)
        {
            byte darkened = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

            byte red = (byte)Math.Min(ColorFilling.R * darkened / 255, 255);
            byte green = (byte)Math.Min(ColorFilling.G * darkened / 255, 255);
            byte blue = (byte)Math.Min(ColorFilling.B * darkened / 255, 255);

            Wall.FillColor = new Color(red, green, blue);
        }
        public override void fillingMiniMapShape(RectangleShape rectangleShape)
        {
            rectangleShape.OutlineThickness = 1;
            rectangleShape.FillColor = ColorInMap;
        }

        #region Render

        #region RenderOperation
        public override float normalizePositionY(Screen screen, double angleVertical, float addVariable = 0)
        {
            if (angleVertical <= 0)
                return (float)(screen.Setting.HalfHeight - (screen.Setting.HalfHeight * angleVertical) - addVariable);
            else
            {
                angleVertical += 1;

                return (float)(screen.Setting.HalfHeight / angleVertical - addVariable);
            }
        }
        public float calcCooX(double ray, Screen screen)
        {
            return (float) ray * screen.Setting.Scale;
        }
        #endregion

        private void calculationBlockScale(ref Screen screen, Result result)
        {
            float scaleX = screen.Setting.Scale;
            float scaleY = (float)result.ProjHeight;

            Wall.Size = new Vector2f(scaleX, scaleY);
        }
        private void calculationBlockPosition(ref Screen screen, Result result, ref double angleVertical)
        {
            float positionX = calcCooX(result.Ray, screen);
            float positionY = normalizePositionY(screen, angleVertical, (float)result.ProjHeight / 2);

            Wall.Position = new Vector2f(positionX, positionY);
        }

        public override void render(Screen screen, Result result, Entity entity)
        {
            screen.vertexArray.Clear();

            calculationBlockScale(ref screen, result);
            calculationBlockPosition(ref screen, result, ref entity.getEntityVerticalA());

            blackoutObstacle(result.Depth);

            screen.Window.Draw(Wall);
        }
        #endregion
    }
}
