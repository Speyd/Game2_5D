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
using ObstacleLib.BlankWallLib.Render;
using Render;

namespace ObstacleLib.BlankWallLib
{
    public class BlankWall : Obstacle, IWall
    {
        //--------------------Color For Render------------------
        public Color StandartColorFilling { get; set; } //Without BlackoutObstacle
        public Color ColorFilling { get; set; }


        //-------------------------Render Operation-----------------------
        private RenderOpertion RenderOperation = new RenderOpertion();


        //-------------------Setting--------------------
        public override bool IsSingleAddable { get; init; } = true;

        #region Constructor
        public BlankWall(Color color, bool isPassability = false)

            : base(0, 0, 'B', color, isPassability)
        {
            StandartColorFilling = color;
        }

        public BlankWall(byte r, byte g, byte b, bool isPassability = false)

            : base(0, 0, 'B', new Color(r, g, b), isPassability)
        {
            StandartColorFilling = new Color(r, g, b);
        }
        #endregion

        #region IRenderable_Implementation
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

        #region IWall_Implementation
        public float CalcCooX(double ray)
        {
            return (float)ray * Screen.Setting.Scale;
        }
        #endregion

        public override void UpdateAdditionalInformation(double x, double y)
        {
            X = x;
            Y = y;

            Left = X;
            Right = X + Screen.Setting.Tile;
            Top = Y;
            Bottom = Y + Screen.Setting.Tile;
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
