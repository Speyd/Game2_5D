using EntityLib;
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
        //--------------------Color For Render------------------
        public Color StandartColorFilling { get; set; } //Without BlackoutObstacle
        public Color ColorFilling { get; set; }



        //-------------------------Render Operation-----------------------
        private RenderBlankWallOpertion RenderOperation = new RenderBlankWallOpertion();



        public BlankWall(double x, double y, Color color, bool isPassability = false)

            : base(x, y, 'B', color, isPassability)
        {
            StandartColorFilling = color;
        }

        public BlankWall(double x, double y, byte r, byte g, byte b, bool isPassability = false)

            : base(x, y, 'B', new Color(r, g, b), isPassability)
        {
            StandartColorFilling = new Color(r, g, b);
        }




        #region IRenderableImplementation
        public override void BlackoutObstacle(double depth)
        {
            byte darkened = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

            byte red = (byte)Math.Min(StandartColorFilling.R * darkened / 255, 255);
            byte green = (byte)Math.Min(StandartColorFilling.G * darkened / 255, 255);
            byte blue = (byte)Math.Min(StandartColorFilling.B * darkened / 255, 255);

            ColorFilling = new Color(red, green, blue);
        }
        public override void FillingMiniMapShape(RectangleShape rectangleShape)
        {
            rectangleShape.OutlineThickness = 1;
            rectangleShape.FillColor = ColorInMap;
        }
        public override float NormalizePositionY(double angleVertical, float addVariable = 0)
        {
            if (angleVertical <= 0)
                return (float)(Screen.Setting.HalfHeight - Screen.Setting.HalfHeight * angleVertical - addVariable);
            else
            {
                angleVertical += 1;

                return (float)(Screen.Setting.HalfHeight / angleVertical - addVariable);
            }
        }
        #endregion
        //public override bool Collision(double X, double Y, double playerSide)
        //{
        //    bool isCollidingX = X > Left && X < Right;
        //    bool isCollidingY = Y > Top && Y < Bottom;

        //    return isCollidingX && isCollidingY && !IsPassability;
        //}
        //public override void ResetXSides(double value)
        //{
        //    Left = X;
        //    Right = X + Screen.Setting.Tile;
        //}
        //public override void ResetYSides(double value)
        //{
        //    Top = Y;
        //    Bottom = Y + Screen.Setting.Tile;
        //}
        //public override float Normalize_X_MiniMap()
        //{
        //    return (float)X / Screen.Setting.Tile;
        //}
        //public override float Normalize_Y_MiniMap()
        //{
        //    return (float)Y / Screen.Setting.Tile;
        //}
        public override void StandartSetSides()
        {
            Left = X;
            Right = X + Screen.Setting.Tile;
            Top = Y;
            Bottom = Y + Screen.Setting.Tile;
        }
        public float CalcCooX(double ray)
        {
            return (float)ray * Screen.Setting.Scale;
        }
        public override void Render(Result result, Entity entity)
        {
            BlackoutObstacle(result.Depth);

            VertexArray renderWall = new VertexArray(PrimitiveType.Quads, 4);
            RenderOperation.UpdateVertices(
                this, renderWall,
                RenderOperation.CalculationBlockScale(result),
                RenderOperation.CalculationBlockPosition(this, result, entity)
                );

            ZBuffer.AddToZBuffer(renderWall, result.Depth);
        }
    }
}
