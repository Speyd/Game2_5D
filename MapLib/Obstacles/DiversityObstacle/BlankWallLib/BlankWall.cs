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
using Render.InterfaceRender;
using Render.ZBufferRender;
using Render.ResultAlgorithm;
using SFML.System;
using System.Numerics;

namespace MapLib.Obstacles.DiversityObstacle.BlankWallLib
{
    public class BlankWall : Obstacle, IWall
    {

        public Color ColorFilling { get; set; }
        public Color CurrentColorFilling { get; set; }

        private RenderBlankWallOpertion renderOperation = new RenderBlankWallOpertion();

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


        #region IRenderableImplementation
        public override void BlackoutObstacle(double depth)
        {
            byte darkened = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

            byte red = (byte)Math.Min(ColorFilling.R * darkened / 255, 255);
            byte green = (byte)Math.Min(ColorFilling.G * darkened / 255, 255);
            byte blue = (byte)Math.Min(ColorFilling.B * darkened / 255, 255);

            CurrentColorFilling = new Color(red, green, blue);
        }
        public override void FillingMiniMapShape(RectangleShape rectangleShape)
        {
            rectangleShape.OutlineThickness = 1;
            rectangleShape.FillColor = ColorInMap;
        }
        public override float NormalizePositionY(Screen screen, double angleVertical, float addVariable = 0)
        {
            if (angleVertical <= 0)
                return (float)(screen.Setting.HalfHeight - screen.Setting.HalfHeight * angleVertical - addVariable);
            else
            {
                angleVertical += 1;

                return (float)(screen.Setting.HalfHeight / angleVertical - addVariable);
            }
        }
        #endregion

        public float CalcCooX(double ray, Screen screen)
        {
            return (float)ray * screen.Setting.Scale;
        }
        public override void Render(Screen screen, Result result, Entity entity)
        {
            BlackoutObstacle(result.Depth);

            VertexArray renderWall = new VertexArray(PrimitiveType.Quads, 4);
            renderOperation.UpdateVertices(
                this, renderWall,
                renderOperation.CalculationBlockScale(screen, result),
                renderOperation.CalculationBlockPosition(screen, this, result, entity)
                );

            ZBuffer.AddToZBuffer(renderWall, result.Depth);
        }
    }
}
