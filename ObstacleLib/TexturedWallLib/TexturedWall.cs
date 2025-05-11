using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using ScreenLib;
using SFML.System;
using TextureLib;
using ObstacleLib.TexturedWallLib.Render;
using HitBoxLib.PositionObject;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.Segment.SignsTypeSide;
using EffectLib;
using ProtoRender.RenderAlgorithm;
using ProtoRender.Object;
using ObstacleLib.SpriteLib;
using DataPipes;
using ProtoRender.RenderInterface;
using ObstacleLib.BlankWallLib;


namespace ObstacleLib.TexturedWallLib;
public class TexturedWall : Obstacle, IWall, IDrawable
{
    //----------------------Textures--------------------------
    public MultiTexturedObject MultiTextured { get; init; }
    public TexturedPair? CurrentRenderTexture { get; set; } = null;

    //----------------------Setting---------------------
    /// <summary>Current wall level in world wall level</summary>
    public int LvlWall { get; private set; } = IWall.minLvlWall;



    #region Constructor
    private static SFML.Graphics.Color GetDefaultWallColor() => SFML.Graphics.Color.Red;
    private string GetFirstTextureOrThrow()
    {
        return MultiTextured.UniqueTexture.GetFirstValue()?.Base.PathTexture
               ?? throw new Exception("Error loading Texture (TexturedWall)");
    }
    private void InitializeWall()
    {
        UpdateBaseHeightHitBox();
        Z.Axis = (LvlWall - 1) * Screen.Setting.HalfTile;
    }

    public TexturedWall(params string[] texturePaths)
       : base(0, 0, GetDefaultWallColor(), false)
    {
        if (texturePaths.Length == 0)
            throw new ArgumentException("At least one texture path must be provided.");

        MultiTextured = new MultiTexturedObject(texturePaths);
        TextureInMiniMap = new TextureObstacle(GetFirstTextureOrThrow());

        InitializeWall();
    }
    public TexturedWall(bool isPassability = false, params string[] texturePaths)
       : base(0, 0, GetDefaultWallColor(), isPassability)
    {
        if (texturePaths.Length == 0)
            throw new ArgumentException("At least one texture path must be provided.");

        MultiTextured = new MultiTexturedObject(texturePaths);
        TextureInMiniMap = new TextureObstacle(GetFirstTextureOrThrow());

        InitializeWall();
    }
    public TexturedWall(List<(ObjectSide, string)> textures, bool isPassability = false)
        : base(0, 0, GetDefaultWallColor(), isPassability)
    {
        if (textures == null || textures.Count == 0)
            throw new ArgumentException("Texture list cannot be empty.");

        MultiTextured = new MultiTexturedObject(textures);
        TextureInMiniMap = new TextureObstacle(GetFirstTextureOrThrow());

        InitializeWall();
    }
    public TexturedWall(List<(ObjectSide, TextureObstacle)> textures, bool isPassability = false)
        : base(0, 0, GetDefaultWallColor(), isPassability)
    {
        if (textures == null || textures.Count == 0)
            throw new ArgumentException("Texture list cannot be empty.");

        MultiTextured = new MultiTexturedObject(textures);
        TextureInMiniMap = new TextureObstacle(GetFirstTextureOrThrow());

        InitializeWall();
    }

    public TexturedWall(TexturedWall texturedWall)
        : base(0, 0, SFML.Graphics.Color.Black, false)
    {
        HitBox = new HitBox(texturedWall.HitBox);

        X.UpdateInfo(texturedWall.X, HitBox);
        Y.UpdateInfo(texturedWall.Y, HitBox);
        Z.UpdateInfo(texturedWall.Z, HitBox);

        ColorInMap = texturedWall.ColorInMap;
        TextureInMiniMap = texturedWall.TextureInMiniMap is not null ? new TextureObstacle(texturedWall.TextureInMiniMap) : null;

        IsPassability = texturedWall.IsPassability;
        IsSingleAddable = texturedWall.IsSingleAddable;

        SizeScale = texturedWall.SizeScale;
        PositionScale = texturedWall.PositionScale;

        MultiTextured = new MultiTexturedObject(texturedWall.MultiTextured);
        CurrentRenderTexture = null;

        LvlWall = texturedWall.LvlWall;
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
            rectangleShape.Texture = TextureInMiniMap.Texture;
        else
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
    public void SetLevelWall(double currentZ, double baseOffset, int lvl)
    {
        LvlWall = lvl;
        HitBox.MainHitBox[CoordinatePlane.Z, SideSize.Smaller]?.SetOffset(baseOffset);
        HitBox.MainHitBox[CoordinatePlane.Z, SideSize.Larger]?.SetOffset(baseOffset);

        Z.Axis = currentZ + (lvl - 1) * baseOffset;
    }
    #endregion

    #region IDrawable_Implementation
    public float CalculateTextureX(Vector2f UV, ObjectSide side)
    {
        CurrentRenderTexture = MultiTextured[side];
        if (CurrentRenderTexture is null)
            throw new Exception("CurrentRenderTexture is null (GetTextureCoordinate)");

        float textureX = UV.X > UV.Y ? UV.X : UV.Y;
        textureX *= CurrentRenderTexture.Base.Width / Screen.Setting.Scale;

        return textureX - (float)Math.Pow(CurrentRenderTexture.Base.Height / TextureObstacle.BaseHeight, 4.5f);
    }      
    public float BringingToStandard(float heightObj)
    {
        if (CurrentRenderTexture is null)
            throw new Exception("CurrentRenderTexture is null(BringingToStandard)");

        heightObj *= TextureObstacle.DifferenceHeight(CurrentRenderTexture.Base.Height);
        return heightObj;
    }
    public float GetAveragedMult(float baseMult)
    {
        if (CurrentRenderTexture is null)
            throw new Exception("CurrentRenderTexture is null(GetAveragedMult)");


        float newMult = baseMult * Screen.ScreenRatio;
        newMult *= (float)TextureObstacle.BaseHeight / CurrentRenderTexture.Base.Height;

        return newMult;
    }
    public float CalculateTextureY(IUnit unit, float ProjHeight, float mult, float addCoordinates)
    {
        if (CurrentRenderTexture is null)
            throw new Exception("CurrentRenderTexture is null(GetAveragedMult)");

        float textureY = ProjHeight * (float)unit.VerticalAngle * mult;

        float heightDifference = (float)(Z.Axis - unit.Z.Axis);
        float textureMult = Screen.Setting.VerticalTile / (CurrentRenderTexture?.Base.Height ?? 1);
        return CurrentRenderTexture.Base.Height / 2 + textureY - addCoordinates + (heightDifference / textureMult);
    }
    public float CalculateTextureY(float verticalAngle, float ProjHeight, float mult, float addCoordinates)
    {
        if (CurrentRenderTexture is null)
            throw new Exception("CurrentRenderTexture is null(GetAveragedMult)");

        float textureY = ProjHeight * verticalAngle * mult;
        return CurrentRenderTexture.Base.Height / 2 + textureY - addCoordinates;
    }
    public bool IsInsideTexture(float textureX, float textureY)
    {
        if(CurrentRenderTexture is null)
            return true;

        var baseTexure = CurrentRenderTexture.Base;
        if (textureX < 0 || textureX > baseTexure.Width)
            return true;
        else if(textureY < 0 || textureY > baseTexure.Height)
            return true;

        return false;
    }

    public void DrawObject(Drawable drawObject)
    {
        if (CurrentRenderTexture is null)
            return;

        CurrentRenderTexture.Mod.Draw(drawObject);
        CurrentRenderTexture.Mod.Display();
    }
    public void DrawObjectAsync(Drawable drawObject)
    {
        ProtoRender.RenderAlgorithm.DrawingQueue.EnqueueDraw((DrawObject, drawObject));
    }
    #endregion

    public override IObject GetCopy()
    {
        return new TexturedWall(this);
    }

    public override void Render(Result result, IUnit unit)
    {
        RenderInternal(result, unit, RenderOperation.SelectCurrentRenderTexture(this, result, unit));
    }
    public void RenderMultiWall(Result result, IUnit unit, ObjectSide objectSide)
    {
        RenderInternal(result, unit, MultiTextured[objectSide]);
    }


    private void RenderInternal(Result result, IUnit unit, TexturedPair? currentRenderTexture)
    {
        if (currentRenderTexture is null)
            return;

        IntRect textureRect = TextureObstacle.SetIntegerRectangle((int)result.Offset, Screen.Setting.Tile, currentRenderTexture.Base);
        CoordinateOnScreen position = GetPositionOnScreen(result, unit);
        Vector2f scale = RenderOperation.CalculationTextureScale(result, currentRenderTexture);

        if (IsOffScreen(result, position, textureRect.Height, scale))
            return;

        VertexArray vertexArray = new VertexArray(PrimitiveType.Quads, 4);

        SFML.Graphics.Color blackoutColor = VisualEffectHelper.VisualEffect.TransformationColor(result.Depth);

        Vector2f topLeftTexCoords = new Vector2f(textureRect.Left, textureRect.Top);
        Vector2f topRightTexCoords = new Vector2f(textureRect.Left + textureRect.Width, textureRect.Top);
        Vector2f bottomRightTexCoords = new Vector2f(textureRect.Left + textureRect.Width, textureRect.Top + textureRect.Height);
        Vector2f bottomLeftTexCoords = new Vector2f(textureRect.Left, textureRect.Top + textureRect.Height);

        Vector2f topLeftPosition = new Vector2f(position.X, position.Top);
        Vector2f topRightPosition = new Vector2f(position.X + scale.X * textureRect.Width, position.Top);
        Vector2f bottomRightPosition = new Vector2f(position.X + scale.X * textureRect.Width, position.Top + scale.Y * textureRect.Height);
        Vector2f bottomLeftPosition = new Vector2f(position.X, position.Top + scale.Y * textureRect.Height);

        vertexArray[0] = new Vertex(topLeftPosition, blackoutColor, topLeftTexCoords);
        vertexArray[1] = new Vertex(topRightPosition, blackoutColor, topRightTexCoords);
        vertexArray[2] = new Vertex(bottomRightPosition, blackoutColor, bottomRightTexCoords);
        vertexArray[3] = new Vertex(bottomLeftPosition, blackoutColor, bottomLeftTexCoords);

        RenderStates renderStates = new RenderStates(currentRenderTexture.Mod.Texture);

        result.Depth += (LvlWall + 1) * 0.01;
        ZBuffer.AddToZBuffer(vertexArray, result.Depth, renderStates);
    }
}
