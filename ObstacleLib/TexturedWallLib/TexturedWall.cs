using SFML.Graphics;
using ScreenLib;
using SFML.System;
using TextureLib.Textures;
using ObstacleLib.TexturedWallLib.Render;
using HitBoxLib.PositionObject;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.Segment.SignsTypeSide;
using ProtoRender.RenderAlgorithm;
using ProtoRender.Object;
using ProtoRender.RenderInterface;
using RayTracingLib.Detection;
using TextureLib.Textures.Pair;
using EffectLib.EffectCore;
using TextureLib.Loader.ImageProcessing;

namespace ObstacleLib.TexturedWallLib;
public class TexturedWall : Obstacle, IWall, IDrawable
{
    //----------------------Textures--------------------------
    public MultiSideTexture MultiSide { get; internal set; }
    public TexturedPair? CurrentRenderTexture { get; set; } = null;

    //----------------------Setting---------------------
    /// <summary>Current wall level in world wall level</summary>
    public int LvlWall { get; internal set; } = IWall.minLvlWall;


    #region Constructor
    private static SFML.Graphics.Color GetDefaultWallColor() => SFML.Graphics.Color.Red;
    private string GetFirstTextureOrThrow()
    {
        return MultiSide.UniqueTexture.GetFirstValue()?.Base.PathTexture
               ?? throw new Exception("Error loading Texture (TexturedWall)");
    }
    private void InitializeWall()
    {
        UpdateBaseHeightHitBox();
        Z.Axis = (LvlWall - 1) * Screen.Setting.HalfTile;
    }

    public TexturedWall()
       : base(GetDefaultWallColor(), false)
    {
        MultiSide = new MultiSideTexture();
        InitializeWall();
    }
    public TexturedWall(ImageLoadOptions? options = null, bool createNewTexture = true, params string[] texturePaths)
       : base(GetDefaultWallColor(), false)
    {
        if (texturePaths.Length == 0)
            throw new ArgumentException("At least one texture path must be provided.");

        MultiSide = new MultiSideTexture(options, createNewTexture, texturePaths);
        TextureInMiniMap = new TextureWrapper(GetFirstTextureOrThrow(), true);

        InitializeWall();
    }
    public TexturedWall(List<(ObjectSide, string)> textures, ImageLoadOptions? options = null, bool createNewTexture = true)
        : base(GetDefaultWallColor(), false)
    {
        if (textures == null || textures.Count == 0)
            throw new ArgumentException("Texture list cannot be empty.");

        MultiSide = new MultiSideTexture(textures, options, createNewTexture);
        TextureInMiniMap = new TextureWrapper(GetFirstTextureOrThrow(), true);

        InitializeWall();
    }
    public TexturedWall(Dictionary<ObjectSide, string> textures, ImageLoadOptions? options = null, bool createNewTexture = true)
        : base(GetDefaultWallColor(), false)
    {
        if (textures == null || textures.Count == 0)
            throw new ArgumentException("Texture list cannot be empty.");

        MultiSide = new MultiSideTexture(textures, options, createNewTexture);
        TextureInMiniMap = new TextureWrapper(GetFirstTextureOrThrow(), true);

        InitializeWall();
    }
    public TexturedWall(List<(ObjectSide, TextureWrapper)> textures, ImageLoadOptions? options = null, bool createNewTexture = true)
        : base(GetDefaultWallColor(), false)
    {
        if (textures == null || textures.Count == 0)
            throw new ArgumentException("Texture list cannot be empty.");

        MultiSide = new MultiSideTexture(textures, options, createNewTexture);
        TextureInMiniMap = new TextureWrapper(GetFirstTextureOrThrow(), true);

        InitializeWall();
    }
    public TexturedWall(TexturedWall texturedWall, ImageLoadOptions? options = null, bool createNewTexture = true)
        : base(texturedWall)
    {
        MultiSide = new MultiSideTexture(texturedWall.MultiSide, options, createNewTexture);
        CurrentRenderTexture = null;

        LvlWall = texturedWall.LvlWall;
    }
    

    public TexturedWall(ImageLoadOptions? options = null, HashSet<ObjectSide>? sharedSides = null, params string[] texturePaths)
       : base(GetDefaultWallColor(), false)
    {
        if (texturePaths.Length == 0)
            throw new ArgumentException("At least one texture path must be provided.");

        MultiSide = new MultiSideTexture(sharedSides ?? new(), options, texturePaths);
        TextureInMiniMap = new TextureWrapper(GetFirstTextureOrThrow(), true);

        InitializeWall();
    }
    public TexturedWall(List<(ObjectSide, string)> textures, ImageLoadOptions? options = null, HashSet<ObjectSide>? sharedSides = null)
        : base(GetDefaultWallColor(), false)
    {
        if (textures == null || textures.Count == 0)
            throw new ArgumentException("Texture list cannot be empty.");

        MultiSide = new MultiSideTexture(textures, sharedSides ?? new(), options);
        TextureInMiniMap = new TextureWrapper(GetFirstTextureOrThrow(), true);

        InitializeWall();
    }
    public TexturedWall(Dictionary<ObjectSide, string> textures, ImageLoadOptions? options = null, HashSet<ObjectSide>? sharedSides = null)
        : base(GetDefaultWallColor(), false)
    {
        if (textures == null || textures.Count == 0)
            throw new ArgumentException("Texture list cannot be empty.");

        MultiSide = new MultiSideTexture(textures, sharedSides ?? new(), options);
        TextureInMiniMap = new TextureWrapper(GetFirstTextureOrThrow(), true);

        InitializeWall();
    }
    public TexturedWall(List<(ObjectSide, TextureWrapper)> textures, ImageLoadOptions? options = null, HashSet<ObjectSide>? sharedSides = null)
        : base(GetDefaultWallColor(), false)
    {
        if (textures == null || textures.Count == 0)
            throw new ArgumentException("Texture list cannot be empty.");

        MultiSide = new MultiSideTexture(textures, sharedSides ?? new(), options);
        TextureInMiniMap = new TextureWrapper(GetFirstTextureOrThrow(), true);

        InitializeWall();
    }
    public TexturedWall(TexturedWall texturedWall, bool isPassability = false, ImageLoadOptions? options = null, HashSet<ObjectSide>? sharedSides = null)
        : base(texturedWall)
    {
        MultiSide = new MultiSideTexture(texturedWall.MultiSide, sharedSides ?? new(), options);
        CurrentRenderTexture = null;

        LvlWall = texturedWall.LvlWall;
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
        if (TextureInMiniMap is not null && TextureInMiniMap.IsLoaded)
            rectangleShape.Texture = TextureInMiniMap.Texture;
        else
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
    public void SetLevelWall(double currentZ, double baseOffset, int lvl)
    {
        LvlWall = lvl;
        HitBox.MainHitBox[CoordinatePlane.Z, SideSize.Smaller]?.SetOffset(baseOffset);
        HitBox.MainHitBox[CoordinatePlane.Z, SideSize.Larger]?.SetOffset(baseOffset);

        Z.Axis = currentZ + (lvl - 1) * baseOffset;
    }
    #endregion

    #region IDrawable
    public float CalculateTextureX(Vector2f UV, ObjectSide side)
    {
        CurrentRenderTexture = MultiSide[side];
        if (CurrentRenderTexture is null)
            throw new Exception("CurrentRenderTexture is null (GetTextureCoordinate)");

        float textureX = UV.X > UV.Y ? UV.X : UV.Y;
        textureX *= CurrentRenderTexture.Base.Width / Screen.Setting.Scale;

        return textureX;
    }      
    public float BringingToStandardHeight(float heightObj)
    {
        if (CurrentRenderTexture is null)
            throw new Exception("CurrentRenderTexture is null(BringingToStandardHeight)");

        heightObj *= TextureWrapper.DifferenceHeight(CurrentRenderTexture.Base.Height);
        return heightObj;
    }
    public float BringingToStandardWidth(float widthtObj)
    {
        if (CurrentRenderTexture is null)
            throw new Exception("CurrentRenderTexture is null(BringingToStandardWidth)");

        widthtObj *= TextureWrapper.DifferenceWidth(CurrentRenderTexture.Base.Width);
        return widthtObj;
    }
    public float GetAveragedMult(float baseMult)
    {
        if (CurrentRenderTexture is null)
            throw new Exception("CurrentRenderTexture is null(GetAveragedMult)");


        float newMult = baseMult * Screen.ScreenRatio;
        newMult *= (float)TextureWrapper.BaseHeight / CurrentRenderTexture.Base.Height;

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

    #region ITextureProvider
    public override TextureWrapper? GetUsedTexture(IUnit? observer = null)
    {
        if (observer is null || MultiSide.UniqueTexture.Count == 0)
            return TextureInMiniMap;

        return MultiSide[RayDetectionX.DetermineObjectSides(this, observer)]?.Base;
    }
    #endregion

    public override IObject GetCopy()
    {
        return new TexturedWall(this, true);
    }
    public override IObject GetDeepCopy()
    {
        return new TexturedWall(this, false);
    }

    public override void Render(Result result, IUnit unit)
    {
        RenderInternal(result, unit, RenderOperation.SelectCurrentRenderTexture(this, result, unit));
    }
    public void RenderMultiWall(Result result, IUnit unit, ObjectSide objectSide)
    {
        RenderInternal(result, unit, MultiSide[objectSide]);
    }


    private void RenderInternal(Result result, IUnit unit, TexturedPair? currentRenderTexture)
    {
        TextureWrapper texture;
        if (currentRenderTexture is null || !currentRenderTexture.Base.IsLoaded)
            texture = TextureWrapper.Placeholder;
        else
            texture = currentRenderTexture.Base;

        IntRect textureRect = TextureWrapper.SetIntegerRectangle((int)result.Offset, Screen.Setting.Tile, texture);
        CoordinateOnScreen position = GetPositionOnScreen(result, unit);
        Vector2f scale = RenderOperation.CalculationTextureScale(result, texture);

        if (IsOffScreen(result, position, textureRect.Height, scale))
            return;

        VertexArray vertexArray = new VertexArray(PrimitiveType.Quads, 4);

        SFML.Graphics.Color effectColor = EffectUtils.ApplyEffect(Effect, BaseEffectColor, (float)result.Depth / Screen.Setting.Tile) ?? SFML.Graphics.Color.White;

        Vector2f topLeftTexCoords = new Vector2f(textureRect.Left, textureRect.Top);
        Vector2f topRightTexCoords = new Vector2f(textureRect.Left + textureRect.Width, textureRect.Top);
        Vector2f bottomRightTexCoords = new Vector2f(textureRect.Left + textureRect.Width, textureRect.Top + textureRect.Height);
        Vector2f bottomLeftTexCoords = new Vector2f(textureRect.Left, textureRect.Top + textureRect.Height);

        Vector2f topLeftPosition = new Vector2f(position.X, position.Top);
        Vector2f topRightPosition = new Vector2f(position.X + scale.X * textureRect.Width, position.Top);
        Vector2f bottomRightPosition = new Vector2f(position.X + scale.X * textureRect.Width, position.Top + scale.Y * textureRect.Height);
        Vector2f bottomLeftPosition = new Vector2f(position.X, position.Top + scale.Y * textureRect.Height);

        vertexArray[0] = new Vertex(topLeftPosition, effectColor, topLeftTexCoords);
        vertexArray[1] = new Vertex(topRightPosition, effectColor, topRightTexCoords);
        vertexArray[2] = new Vertex(bottomRightPosition, effectColor, bottomRightTexCoords);
        vertexArray[3] = new Vertex(bottomLeftPosition, effectColor, bottomLeftTexCoords);

        var renderTexture = currentRenderTexture is null || !currentRenderTexture.Base.IsLoaded || currentRenderTexture.Mod.Texture is null
            ? TextureWrapper.Placeholder.Texture
            : currentRenderTexture.Mod.Texture;
        RenderStates renderStates = new RenderStates(renderTexture);

        result.Depth += (LvlWall + 1) * 0.01;
        ZBuffer.AddToZBuffer(vertexArray, result.Depth, renderStates);
    }
}
