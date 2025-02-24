using EntityLib;
using ScreenLib;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using System.Numerics;
using ObstacleLib.BlankWallLib.Render;
using HitBoxLib;
using HitBoxLib.PositionObject;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.Segment.SignsTypeSide;
using EffectLib;
using Render.RenderAlgorithm;
using Render.Object;


namespace ObstacleLib.BlankWallLib;
public class BlankWall : Obstacle, IWall
{
    //--------------------Color For Render------------------
    public Color StandartColorFilling { get; set; } //Without BlackoutObstacle
   // public Color ColorFilling { get; set; }

    //-------------------Setting--------------------
    public override bool IsSingleAddable { get; init; } = true;

    #region Constructor
    public BlankWall(Color color, bool isPassability = false)
        : base(0, 0, color, isPassability)
    {
        StandartColorFilling = color;

        UpdateBaseHeightHitBox();
        Z.Axis = 0;
    }

    public BlankWall(byte r, byte g, byte b, bool isPassability = false)
        : base(0, 0, new Color(r, g, b), isPassability)
    {
        StandartColorFilling = new Color(r, g, b);

        UpdateBaseHeightHitBox();
        Z.Axis = 0;
    }
    #endregion


    #region MapAdder_Implementation
    private void UpdateBaseHeightHitBox()
    {
        HitBox.MainHitBox[CoordinatePlane.Z, SideSize.Smaller]?.SetOffset(Screen.Setting.HalfTile * IWall.baseMultHeightOnScreen);
        HitBox.MainHitBox[CoordinatePlane.Z, SideSize.Larger]?.SetOffset(Screen.Setting.HalfTile * IWall.baseMultHeightOnScreen);
    }
    public override void UpdateAdditionalInformation(double x, double y)
    {
        HitBox.MainHitBox[CoordinatePlane.X, SideSize.Smaller]?.SetOffset(0);
        HitBox.MainHitBox[CoordinatePlane.X, SideSize.Larger]?.SetOffset(Screen.Setting.Tile);
        HitBox.MainHitBox[CoordinatePlane.Y, SideSize.Smaller]?.SetOffset(0);
        HitBox.MainHitBox[CoordinatePlane.Y, SideSize.Larger]?.SetOffset(Screen.Setting.Tile);

        X.Axis = x;
        Y.Axis = y;
    }
    #endregion

    #region IMiniMapRenderable_Implementation
    public override void FillingColorShape(RectangleShape rectangleShape, float OutlineThickness = 1)
    {
        rectangleShape.OutlineThickness = OutlineThickness;
        rectangleShape.FillColor = ColorInMap;
    }
    public override void FillingTextureShape(RectangleShape rectangleShape)
    {
        rectangleShape.OutlineThickness = 1;
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
    public override Vector2f GetPositionOnScreen(Result result, Entity entity)
    {
        float positionX = WorldToScreenX(result.Ray);
        float positionY = WorldToScreenY(entity.VerticalAngle, (float)result.ProjHeight / 2);

        return new Vector2f(positionX, positionY);
    }

    //public override SFML.Graphics.Color BlackoutObstacle(double depth)
    //{
    //    byte darkened = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

    //    byte red = (byte)Math.Min(StandartColorFilling.R * darkened / 255, 255);
    //    byte green = (byte)Math.Min(StandartColorFilling.G * darkened / 255, 255);
    //    byte blue = (byte)Math.Min(StandartColorFilling.B * darkened / 255, 255);

    //    return new Color(red, green, blue);
    //}
    #endregion

    #region IWall_Implementation
    public override double GetZCoordinate() => Z.Axis;
    #endregion
    public void ProcessForRendering(List<InfoObject> infoObject, double coordinate, double depth, double maxDepth)
    {
        if (depth < maxDepth)
            infoObject.Add(new InfoObject(depth, coordinate, this));
    }
    public override void Render(Result result, Entity entity)
    {
        Color ColorFilling = VisualEffectHelper.VisualEffect.TransformationColor(StandartColorFilling, result.Depth);

        VertexArray renderWall = new VertexArray(PrimitiveType.Quads, 4);
        RenderOperation.UpdateVertices(
            this, renderWall,
            RenderOperation.CalculationBlockScale(result),
            GetPositionOnScreen(result, entity),
            ColorFilling
            );

        ZBuffer.AddToZBuffer(renderWall, result.Depth);
    }
}
