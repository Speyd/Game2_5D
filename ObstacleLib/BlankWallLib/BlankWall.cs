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
using ProtoRender.RenderAlgorithm;
using ProtoRender.Object;
using ObstacleLib.SpriteLib;
using TextureLib;
using ProtoRender.RenderInterface;


namespace ObstacleLib.BlankWallLib;
public class BlankWall : Obstacle, IWall
{
    //--------------------Color For Render------------------
    public Color StandartColorFilling { get; set; }

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
    public BlankWall(BlankWall blankWall)
       : base(0, 0, SFML.Graphics.Color.Black, false)
    {
        HitBox = new HitBox(blankWall.HitBox);

        X.UpdateInfo(blankWall.X, HitBox);
        Y.UpdateInfo(blankWall.Y, HitBox);
        Z.UpdateInfo(blankWall.Z, HitBox);

        ColorInMap = blankWall.ColorInMap;
        StandartColorFilling = blankWall.StandartColorFilling;
        TextureInMiniMap = blankWall.TextureInMiniMap is not null ? new TextureObstacle(blankWall.TextureInMiniMap) : null;

        IsPassability = blankWall.IsPassability;
        IsSingleAddable = blankWall.IsSingleAddable;

        SizeScale = blankWall.SizeScale;
        PositionScale = blankWall.PositionScale;
    }
    #endregion


    #region MapAdder_Implementation
    private void UpdateBaseHeightHitBox()
    {
        HitBox.MainHitBox[CoordinatePlane.Z, SideSize.Smaller]?.SetOffset(Screen.Setting.HalfVerticalTile);
        HitBox.MainHitBox[CoordinatePlane.Z, SideSize.Larger]?.SetOffset(Screen.Setting.HalfVerticalTile);
    }
    public override void HandleObjectAddition(double x, double y, bool resetHitBoxSide = true)
    {
        if (resetHitBoxSide)
        {
            HitBox.MainHitBox[CoordinatePlane.X, SideSize.Smaller]?.SetOffset(0);
            HitBox.MainHitBox[CoordinatePlane.X, SideSize.Larger]?.SetOffset(Screen.Setting.Tile);
            HitBox.MainHitBox[CoordinatePlane.Y, SideSize.Smaller]?.SetOffset(0);
            HitBox.MainHitBox[CoordinatePlane.Y, SideSize.Larger]?.SetOffset(Screen.Setting.Tile);
        }

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
        if (TextureInMiniMap is not null)
        {
            rectangleShape.Texture = TextureInMiniMap.Texture;
            return;
        }

        rectangleShape.OutlineThickness = 1;
        rectangleShape.FillColor = ColorInMap;
    }

    public override Vector2f ConversionToMapCoordinates(float mapTile)
    {
        float x = (float)X.Axis / Screen.Setting.Tile * mapTile;
        float y = (float)Y.Axis / Screen.Setting.Tile * mapTile;

        return new Vector2f(x, y);
    }
    #endregion

    #region IRenderable_Implementation
    public override CoordinateOnScreen GetPositionOnScreen(Result result, IUnit unit)
    {
        float positionX = WorldToScreenX(result.Ray);
        float screenCenterY = (float)WorldToScreenY(unit.VerticalAngle);
        float verticalShift = (float)((Z.Axis * HitBoxLib.Operations.Render.MultHeight - unit.Z.Axis) / (result.Depth / Screen.Setting.Tile));

        float top = screenCenterY - (float)(result.ProjHeight / 2) - verticalShift;
        float bottom = screenCenterY + (float)(result.ProjHeight / 2) - verticalShift;

        return new CoordinateOnScreen(positionX, bottom, top);
    }
    public void ProcessForRendering(List<InfoObject> infoObject, double coordinate, double depth, double maxDepth)
    {
        if (depth < maxDepth)
            infoObject.Add(new InfoObject(depth, coordinate, this));
    }
    #endregion

    #region IWall_Implementation
    public bool IsOffScreen(Result result, CoordinateOnScreen position, int heightTexture, Vector2f scale)
    {
        if (result.PositionPreviousObject is not null && position.Top > result.PositionPreviousObject.Value.Top && position.Bottom < result.PositionPreviousObject.Value.Bottom)
            return true;
        if (position.Top + scale.Y * heightTexture < 0)
            return true;
        if (position.Top > Screen.ScreenHeight)
            return true;

        return false;
    }
    #endregion

    public override IObject GetCopy()
    {
        return new BlankWall(this);
    }
    public override void Render(Result result, IUnit unit)
    {
        Vector2f scale = RenderOperation.CalculationScale(result);
        CoordinateOnScreen position = GetPositionOnScreen(result, unit);

        if (IsOffScreen(result, position, 1, scale))
            return;

        Color ColorFilling = VisualEffectHelper.VisualEffect.TransformationColor(StandartColorFilling, result.Depth);
        VertexArray renderWall = new VertexArray(PrimitiveType.Quads, 4);
        RenderOperation.UpdateVertices(renderWall, scale, position, ColorFilling);

        ZBuffer.AddToZBuffer(renderWall, result.Depth);
    }
}
