using SFML.Graphics;
using ScreenLib;
using SFML.System;
using ObstacleLib.SpriteLib.Render;
using TextureLib.Textures;
using ObstacleLib.SpriteLib.Add;
using ProtoRender.RenderInterface;
using System.Collections.Concurrent;
using ProtoRender.RenderAlgorithm;
using DataPipes;
using AnimationLib;
using TextureLib.Loader.ImageProcessing;
using ProtoRender.Object;


namespace ObstacleLib.SpriteLib;
public class SpriteObstacle : Obstacle, ISelfRenderable
{

    //-------------------List Sprites Render------------------
    public static List<SpriteObstacle> SpritesToRender { get; private set; } = new List<SpriteObstacle>();
    private static readonly object _lock = new();
    private static readonly object _lockAdd = new();


    //---------------------------Textures------------------------------
    public AnimationState Animation { get; set; } = new();
    public SFML.Graphics.Sprite RenderSprite { get; set; } = new SFML.Graphics.Sprite();


    //--------------------------Setting-----------------------------

    private float scale = 1;
    /// <summary>Texture scale</summary>
    public float Scale
    {
        get => scale;
        set => scale = value == 0 ? 1 : value;
    }
    /// <summary>Size scale</summary>
    public override float SizeScale { get; set; } = 2;
    /// <summary>Position scale in MiniMap</summary>
    public override float PositionScale { get; set; } = 4;

    private bool IsAdded { get; set; } = false;
    public bool IsRenderable { get; set; } = true;

    //---------------------Render Parameters----------------------
    /// <summary>Angle relative to this object and the observer</summary>
    public double AngleToObserver { get; set; }
    public double Distance { get; set; }


    #region Constructor
    public SpriteObstacle(List<TextureWrapper> textures)
        : base(SFML.Graphics.Color.White, false)
    {
        Adder.AddTextures(this, textures);
    }
    public SpriteObstacle(TextureWrapper texture)
       : base(SFML.Graphics.Color.White, false)
    {
        Adder.AddTexture(this, texture);
    }
    public SpriteObstacle(List<string> paths, ImageLoadOptions? options = null)
       : base(SFML.Graphics.Color.White, false)
    {
        Animation.LoadOptions = options ?? new();
        _ = Adder.AddTexturesAsync(this, paths);
    }
    public SpriteObstacle(string path, ImageLoadOptions? options = null)
       : base(SFML.Graphics.Color.White, false)
    {
        Animation.LoadOptions = options ?? new();
        _ = Adder.AddTextureAsync(this, path);
    }
    public SpriteObstacle(AnimationState animationState)
          : base(SFML.Graphics.Color.White, false)
    {
        Animation = new AnimationState(animationState);
    }
    /// <summary>Constructor class AnimationState</summary>
    /// <param name="spriteObstacle">Object of SpriteObstacle</param>
    /// <param name="createNewTexture">true - create new texture, false - load created texture</param>
    public SpriteObstacle(SpriteObstacle spriteObstacle, bool createNewTexture = true, bool loadAsync = false)
        : base(spriteObstacle)
    {      
        IsRenderable = spriteObstacle.IsRenderable;
        IsAdded = spriteObstacle.IsAdded;

        Scale = spriteObstacle.Scale;
        AngleToObserver = spriteObstacle.AngleToObserver;
        Distance = spriteObstacle.Distance;

        Animation = createNewTexture ? new AnimationState(spriteObstacle.Animation, loadAsync) : spriteObstacle.Animation;
    }
    #endregion

    #region MapAdder
    public override void HandleObjectAddition(double x, double y, bool resetHitBoxSide = false)
    {
        X.Axis = x;
        Y.Axis = y;

        ShiftCubedX = ShiftCubedX;
        ShiftCubedY = ShiftCubedY;
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
            rectangleShape.Texture = TextureInMiniMap.Texture;
        else if (TextureInMiniMap is null && Animation.CountFrame > 0)
        {
            TextureInMiniMap = Animation.GetFrame(0);
            rectangleShape.Texture = TextureInMiniMap?.Texture;
        }
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
        Distance = result.Depth;

        float height = (float)(Screen.ScreenHeight / Distance * Scale);

        var coo = RenderOperation.GetPositionOnScreen(this, unit, height);
        return new CoordinateOnScreen(coo.X, coo.Y);
    }
    #endregion

    #region ISelfDrawable
    public void ProcessForRendering(ConcurrentDictionary<Type, bool> uniqueSelfDrawableTypes, ref bool hasNewTypes)
    {
        var type = this.GetType();
        if (uniqueSelfDrawableTypes.TryAdd(type, true))
            hasNewTypes = true;

        this.AddObstacleToRenderList();
    }
    public void AddObstacleToRenderList()
    {
        lock (_lock)
        {
            if (IsAdded == true || IsRenderable == false)
                return;
            else if (SpritesToRender.Contains(this))
                return;

            AddObstacle(this);
        }
    }
    public static void AddObstacle(SpriteObstacle obj)
    {
        lock (_lockAdd)
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
            {
                if(sprite.Animation.IsAnimation)
                    AnimationManager.DefiningDesiredSprite(sprite.Animation, 0);

                continue;
            }

            sprite.Render(result, unit);
        }
    }
    #endregion

    #region ITextureProvider
    public override TextureWrapper? GetUsedTexture(IUnit? observer = null)
    {
       return Animation.GetFrame(0);
    }
    #endregion

    public override IObject GetCopy()
    {
        return new SpriteObstacle(this);
    }
    public override IObject GetDeepCopy()
    {
        return new SpriteObstacle(this, false);
    }
    public virtual double GetDisplayAngle(double spriteAngle)
    {
        return spriteAngle;
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
            AnimationManager.DefiningDesiredSprite(Animation, GetDisplayAngle(spriteAngle));
            Distance *= Math.Cos(AngleToObserver);
            Distance = Math.Max(Distance, 0.1);

            float height = (float)(Screen.ScreenHeight / Distance) * Scale;

            RenderOperation.DrawSprite(this, unit, height);
        }       
        else if (Animation.IsAnimation)
            AnimationManager.DefiningDesiredSprite(Animation, 0);
    }
}
