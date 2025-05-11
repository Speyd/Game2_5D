using SFML.Graphics;
using ScreenLib;
using SFML.System;
using ObstacleLib.SpriteLib.Render;
using TextureLib;
using ObstacleLib.SpriteLib.Add;
using ProtoRender.RenderInterface;
using System.Collections.Concurrent;
using ProtoRender.RenderAlgorithm;
using DataPipes;
using AnimationLib;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.PositionObject;
using ProtoRender.Object;

namespace ObstacleLib.SpriteLib;
public class SpriteObstacle : Obstacle, ISelfRenderable
{

    //-------------------List Sprites Render------------------
    public static List<SpriteObstacle> SpritesToRender { get; private set; } = new List<SpriteObstacle>();
    private static readonly object _lock = new();

    private bool IsAdded { get; set; } = false;
    public bool IsRenderable { get; set; } = true;

    //---------------------------Textures------------------------------
    public AnimationState Animation { get; set; } = new();
    public SFML.Graphics.Sprite RenderSprite { get; set; } = new SFML.Graphics.Sprite();


    //--------------------------Setting-----------------------------

    private float scale = 1;
    /// <summary>Texture scale</summary>
    public float Scale
    {
        get => scale * Screen.ScreenRatio;
        set => scale = value == 0 ? 1 : value;
    }
    /// <summary>Size scale</summary>
    public override float SizeScale { get; set; } = 2;
    /// <summary>Position scale in MiniMap</summary>
    public override float PositionScale { get; set; } = 4;
    //---------------------Render Parameters----------------------
    /// <summary>Angle relative to this object and the observer</summary>
    public double AngleToObserver { get; set; }
    public double Distance { get; set; }


    #region Constructor
    public SpriteObstacle(List<TextureObstacle> textures, bool isPassability = false)
        : base(0, 0, SFML.Graphics.Color.White, isPassability)
    {
        Adder.AddTextures(this, textures);
    }
    public SpriteObstacle(TextureObstacle texture, bool isPassability = false)
       : base(0, 0, SFML.Graphics.Color.White, isPassability)
    {
        Adder.AddTexture(this, texture);
    }
    public SpriteObstacle(string path, bool isDirectory, bool folderAccounting = false, bool isPassability = false)
       : base(0, 0, SFML.Graphics.Color.White, isPassability)
    {
        Adder.AddTextureFromFolder(this, path, isDirectory, folderAccounting);
    }
    public SpriteObstacle(List<string> paths, bool isPassability = false)
       : base(0, 0, SFML.Graphics.Color.White, isPassability)
    {
        Adder.AddTextures(this, paths);
    }
    /// <summary>Constructor class AnimationState</summary>
    /// <param name="spriteObstacle">Object of SpriteObstacle</param>
    /// <param name="updateTexture">true - create new texture, false - load texture</param>
    public SpriteObstacle(SpriteObstacle spriteObstacle, bool updateTexture = true)
    : base(0, 0, SFML.Graphics.Color.Black, false)
    {
        HitBox = new HitBox(spriteObstacle.HitBox);

        X.UpdateInfo(spriteObstacle.X, HitBox);
        Y.UpdateInfo(spriteObstacle.Y, HitBox);
        Z.UpdateInfo(spriteObstacle.Z, HitBox);

        ColorInMap = spriteObstacle.ColorInMap;
        if(updateTexture)
            TextureInMiniMap = spriteObstacle.TextureInMiniMap is not null ? new TextureObstacle(spriteObstacle.TextureInMiniMap) : null;
        else
            TextureInMiniMap = spriteObstacle.TextureInMiniMap;

        IsPassability = spriteObstacle.IsPassability;
        IsSingleAddable = spriteObstacle.IsSingleAddable;
        IsRenderable = spriteObstacle.IsRenderable;
        IsAdded = spriteObstacle.IsAdded;

        SizeScale = spriteObstacle.SizeScale;
        PositionScale = spriteObstacle.PositionScale;

        Scale = spriteObstacle.Scale;
        AngleToObserver = spriteObstacle.AngleToObserver;
        Distance = spriteObstacle.Distance;

        Animation = new AnimationState(spriteObstacle.Animation);
    }
    #endregion

    #region MapAdder_Implementation
    public override void HandleObjectAddition(double x, double y, bool resetHitBoxSide = false)
    {
        X.Axis = x;
        Y.Axis = y;

        ShiftCubedX = ShiftCubedX;
        ShiftCubedY = ShiftCubedY;
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
        else if (TextureInMiniMap is null && Animation.AmountFrame > 0)
        {
            TextureInMiniMap = Animation.GetFrame(0);
            rectangleShape.Texture = TextureInMiniMap?.Texture;
        }
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
        Distance = result.Depth;

        float height = (float)(Screen.ScreenHeight / Distance * Scale);

        var coo = RenderOperation.GetPositionOnScreen(this, unit, height);
        return new CoordinateOnScreen(coo.X, coo.Y);
    }
    #endregion

    #region ISelfDrawable_Implementation
    public void ProcessForRendering(ConcurrentDictionary<Type, bool> uniqueSelfDrawableTypes, ref bool hasNewTypes)
    {
        var type = this.GetType();
        if (uniqueSelfDrawableTypes.TryAdd(type, true))
            hasNewTypes = true;

        this.AddObstacleToRenderList();
    }
    public void AddObstacleToRenderList()
    {
        if (IsAdded == true || IsRenderable == false)
            return;
        else if (SpritesToRender.Contains(this))
            return;

        AddObstacle(this);
    }
    public static void AddObstacle(SpriteObstacle obj)
    {
        lock (_lock)
        {
            SpritesToRender.Add(obj);
            SpritesToRender = SpritesToRender
            .OrderByDescending(sprite => sprite.Distance)
            .ToList();

            obj.IsAdded = true;
        }
    }
    public static void RemoveObstacle(SpriteObstacle obj)
    {
        lock (_lock)
        {
            SpritesToRender.Remove(obj);
        }
    }

    public static void RenderSelfDrawableList(Result result, IUnit unit)
    {
        if(SpritesToRender.Count == 0) 
            return;

        foreach (var sprite in SpritesToRender)
        {
            if (sprite == unit)
                continue;

            sprite.Render(result, unit);
        }
    }
    #endregion

    public override IObject GetCopy()
    {
        return new SpriteObstacle(this);
    }
    public override void Render(Result result, IUnit unit)
    {
        Vector2f pos = new Vector2f((float)X.Axis, (float)Y.Axis);
        double spriteAngle = MathUtils.CalculateAngleToTarget(pos, unit.OriginPosition);
        Distance = MathUtils.CalculateDistance(pos, unit.OriginPosition);

        if (Distance > unit.MaxRenderTile)
            return;

        AngleToObserver = MathUtils.NormalizeAngleDifference(unit.Angle, spriteAngle);

        if (Math.Abs(AngleToObserver) <= unit.Fov)
        {
            AnimationManager.DefiningDesiredSprite(Animation, spriteAngle);

            Distance *= Math.Cos(AngleToObserver);
            Distance = Math.Max(Distance, 0.1);
            float height = (float)(Screen.ScreenHeight / Distance * Scale);

            RenderOperation.DrawSprite(this, unit, height);
        }
        
    }
}
