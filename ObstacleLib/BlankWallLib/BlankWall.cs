using ScreenLib;
using SFML.Graphics;
using SFML.System;
using ObstacleLib.BlankWallLib.Render;
using HitBoxLib.PositionObject;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.Segment.SignsTypeSide;
using EffectLib;
using ProtoRender.RenderAlgorithm;
using ProtoRender.Object;
using TextureLib.Textures;
using ProtoRender.RenderInterface;
using ObstacleLib.SpriteLib;
using EffectLib.EffectCore;


namespace ObstacleLib.BlankWallLib;
public class BlankWall : Obstacle, IWall
{
    //--------------------Color For Render------------------
    public Color ColorFilling { get; set; }

    #region Constructor
    public BlankWall(Color color)
        : base(color, false)
    {
        ColorFilling = color;

        UpdateBaseHeightHitBox();
        Z.Axis = 0;
    }
    public BlankWall(byte r, byte g, byte b)
        : base(new Color(r, g, b), false)
    {
        ColorFilling = new Color(r, g, b);

        UpdateBaseHeightHitBox();
        Z.Axis = 0;
    }
    public BlankWall(BlankWall blankWall)
       : base(blankWall)
    {
        ColorFilling = blankWall.ColorFilling;
    }
    #endregion


    #region MapAdder
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

    #region IMiniMapRenderable
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

    public override Vector2f ConversionToMapCoordinates(Vector2f mapTile)
    {
        float x = (float)X.Axis / Screen.Setting.Tile * mapTile.X;
        float y = (float)Y.Axis / Screen.Setting.Tile * mapTile.Y;

        return new Vector2f(x, y);
    }
    #endregion

    #region IRenderable
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

    #region IWall
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

    #region ITextureProvider
    public override TextureWrapper? GetUsedTexture(IUnit? observer = null) => null;
    #endregion

    public override IObject GetCopy()
    {
        return new BlankWall(this);
    }
    public override IObject GetDeepCopy()
    {
        return new BlankWall(this);
    }
    public override void Render(Result result, IUnit unit)
    {
        Vector2f scale = RenderOperation.CalculationScale(result);
        CoordinateOnScreen position = GetPositionOnScreen(result, unit);

        if (IsOffScreen(result, position, 1, scale))
            return;

        SFML.Graphics.Color effectColor = EffectUtils.ApplyEffect(Effect, ColorFilling, (float)result.Depth / Screen.Setting.Tile) ?? SFML.Graphics.Color.White;

        VertexArray renderWall = new VertexArray(PrimitiveType.Quads, 4);
        RenderOperation.UpdateVertices(renderWall, scale, position, effectColor);

        ZBuffer.AddToZBuffer(renderWall, result.Depth);
    }
}
