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
using Render.RenderInterface;
using HitBoxLib;

namespace ObstacleLib.BlankWallLib
{
    public class BlankWall : Obstacle, IWall
    {
        //--------------------Color For Render------------------
        public Color StandartColorFilling { get; set; } //Without BlackoutObstacle
        public Color ColorFilling { get; set; }

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


         #region IMiniMapRenderable_Implementation
        public override void FillingShape(RectangleShape rectangleShape, float OutlineThickness = 1)
        {
            rectangleShape.OutlineThickness = OutlineThickness;
            rectangleShape.FillColor = ColorInMap;
        }
        public override float CoordinatesOffsetMap(float baseOffset) => baseOffset;
        public override Vector2f ConversionToMapCoordinates(float mapTile)
        {
            float x = (float)X.Axis / Screen.Setting.Tile * mapTile;
            float y = (float)Y.Axis / Screen.Setting.Tile * mapTile;

            return new Vector2f(x, y);
        }
        #endregion

        #region IRenderable_Implementation
        public override Vector2f GetCoordintePositionOnScreen(Result result, Entity entity)
        {
            float positionX = GetRayScreenX(result.Ray);
            float positionY = NormalizeYPosition(entity.VerticalAngle, (float)result.ProjHeight / 2);

            return new Vector2f(positionX, positionY);
        }

        public override void BlackoutObstacle(double depth)
        {
            byte darkened = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

            byte red = (byte)Math.Min(StandartColorFilling.R * darkened / 255, 255);
            byte green = (byte)Math.Min(StandartColorFilling.G * darkened / 255, 255);
            byte blue = (byte)Math.Min(StandartColorFilling.B * darkened / 255, 255);

            ColorFilling = new Color(red, green, blue);
        }
        public override float NormalizeYPosition(double angleVertical, float addVariable = 0)
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
        public override double GetZCoordinate() => Z.Axis;

        public float GetRayScreenX(double ray)
        {
            return (float)ray * Screen.Setting.Scale;
        }
        #endregion
        public override void UpdateAdditionalInformation(double x, double y)
        {
            HitBox[HitBoxSideType.Right]?.SetOffset(0);
            HitBox[HitBoxSideType.Bottom]?.SetOffset(0);
            HitBox[HitBoxSideType.DownSide]?.SetOffset(Screen.Setting.Tile);
            HitBox[HitBoxSideType.UpSide]?.SetOffset(Screen.Setting.Tile);

            X.Axis = x;
            Y.Axis = y;
        }

        public void ProcessForRendering(List<InfoObject> infoObject, double coordinate, double depth, double maxDepth)
        {
            if (depth < maxDepth)
                infoObject.Add(new InfoObject(depth, coordinate, this));
        }
        public override void Render(Result result, Entity entity)
        {
            BlackoutObstacle(result.Depth);

            VertexArray renderWall = new VertexArray(PrimitiveType.Quads, 4);
            RenderOperation.UpdateVertices(
                this, renderWall,
                RenderOperation.CalculationBlockScale(result),
                GetCoordintePositionOnScreen(result, entity)
                );

            ZBuffer.AddToZBuffer(renderWall, result.Depth);
        }

    }
}
