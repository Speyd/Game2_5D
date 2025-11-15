using SFML.Graphics;
using ScreenLib;
using SFML.System;
using ObstacleLib.SpriteLib.Render;
using TextureLib.Textures;
using ProtoRender.RenderInterface;
using System.Collections.Concurrent;
using ProtoRender.RenderAlgorithm;
using DataPipes;
using AnimationLib;
using ProtoRender.Object;
using ProtoRender.Map;
using AnimationLib.Core;
using TextureLib.Loader;
using AnimationLib.Core.Elements;


namespace ObstacleLib.SpriteLib;
public class SpriteObstacle : Obstacle, ISelfRenderable, IAnimatable
{

    //-------------------List Sprites Render------------------
    public static ConcurrentDictionary<IMap, List<SpriteObstacle>> SpritesToRender { get; private set; } = new();
    private static readonly object _lock = new();

    //-------------------Obstacle------------------
    public override IMap? Map
    {
        get => _map;
        set
        {
            if (_map is not null && _map != value)
            {
                _map.DeleteObstacle(this);
                OnPositionChanged -= _map.UpdateCoordinatesObstacle;
                RemoveObstacle(this);
                AddObstacleToRenderList();
            }

            _map = value;
        }
    }

    //---------------------------Textures------------------------------
    public Animator Animation { get; set; } = new();
    public SFML.Graphics.Sprite RenderSprite { get; set; } = new SFML.Graphics.Sprite();


    //--------------------------Setting-----------------------------

    /// <summary>
    /// Global minimum distance constraint shared by all sprites.
    /// </summary>
    public static double GlobalMinDistance { get; set; } = 100;

    /// <summary>
    /// Individual minimum distance constraint, which can be set separately for each sprite.
    /// </summary>
    public double? PersonalMinDistance { get; set; } = null;


    private float scale = 1;
    /// <summary>Texture scale</summary>
    public float Scale
    {
        get => scale;
        set => scale = value == 0 ? 1 : value;
    }
    /// <summary>Size scale in MiniMap</summary>
    public override float SizeScale { get; set; } = 2;
    /// <summary>Position scale in MiniMap</summary>
    public override float PositionScale { get; set; } = 4;

    protected bool IsAdded { get; set; } = false;
    public bool IsRenderable { get; set; } = true;

    //---------------------Render Parameters----------------------
    /// <summary>Angle relative to this object and the observer</summary>
    public double AngleToObserver { get; set; } = 0;


    private double _distance = 0;
    /// <summary>
    /// Actual (world) distance to the object.
    /// Setting this value also updates <see cref="ViewDistance"/>.
    /// </summary>
    public double Distance 
    {
        get => _distance;
        set
        {
            _distance = value;
            ViewDistance = value * Math.Cos(AngleToObserver);
        }
    }
    /// <summary>
    /// Distance projected onto the observer’s view direction
    /// (used for perspective correction and sprite scaling).
    /// </summary>
    public double ViewDistance { get; protected set; }


    #region Constructor
    public SpriteObstacle(TextureWrapper texture)
     : this(new List<TextureWrapper> { texture }) { }

    public SpriteObstacle(List<TextureWrapper> textures, ImageLoadOptions? options = null)
        : base(SFML.Graphics.Color.White, false)
    {
        Animation = new Animator(textures, null, null, options);
    }

    public SpriteObstacle(string path, ImageLoadOptions? options = null)
        : this(new List<string> { path }, options) { }

    public SpriteObstacle(List<string> paths, ImageLoadOptions? options = null)
        : base(SFML.Graphics.Color.White, false)
    {
        Animation = new Animator(options, paths.ToArray());
    }

    public SpriteObstacle(AnimationClip animationClip, ImageLoadOptions? options = null)
        : base(SFML.Graphics.Color.White, false)
    {
        Animation = new Animator(animationClip, null, null, options);
    }
    public SpriteObstacle(Frame frame, ImageLoadOptions? options = null)
        : base(SFML.Graphics.Color.White, false)
    {
        Animation = new Animator(frame, null, null, options);
    }
    public SpriteObstacle(Animator animator, ImageLoadOptions? options = null)
        : base(SFML.Graphics.Color.White, false)
    {
        Animation = options is not null && options.CreateNew ? new Animator(animator, options) : animator;
    }

    public SpriteObstacle(SpriteObstacle spriteObstacle, ImageLoadOptions? options = null)
        : base(spriteObstacle)
    {
        IsRenderable = spriteObstacle.IsRenderable;
        IsAdded = spriteObstacle.IsAdded;

        Scale = spriteObstacle.Scale;
        AngleToObserver = spriteObstacle.AngleToObserver;
        Distance = spriteObstacle.Distance;

        Animation = options is not null && options.CreateNew ? new Animator(spriteObstacle.Animation, options) : spriteObstacle.Animation;
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
        else if (TextureInMiniMap is null && Animation.CurrentFrame?.CountElements > 0)
        {
            TextureInMiniMap = Animation.CurrentFrame.GetElement(0);
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
        if (IsAdded || !IsRenderable || Map is null)
            return;

        var list = SpritesToRender.GetOrAdd(Map, _ => new List<SpriteObstacle>());

        lock (list)
        {
            if (list.Contains(this))
                return;

            list.Add(this);
            list.Sort((a, b) => b.Distance.CompareTo(a.Distance));
            IsAdded = true;
        }
    }
    public static void AddObstacleToRenderList(SpriteObstacle obj)
    {
        if (obj.Map is null || obj.IsAdded || !obj.IsRenderable)
            return;

        var list = SpritesToRender.GetOrAdd(obj.Map, _ => new List<SpriteObstacle>());

        lock (list)
        {
            if (list.Contains(obj))
                return;

            list.Add(obj);
            list.Sort((a, b) => b.Distance.CompareTo(a.Distance));
            obj.IsAdded = true;
        }
    }

    public static void RemoveObstacle(SpriteObstacle obj)
    {
        if (obj.Map is null)
            return;

        if (SpritesToRender.TryGetValue(obj.Map, out var list))
        {
            lock (list)
            {
                list.Remove(obj);
                if (list.Count == 0)
                    SpritesToRender.TryRemove(obj.Map, out _);
            }
        }
    }

    public static void RenderSelfDrawableList(Result result, IUnit unit)
    {
        foreach(var sprites in SpritesToRender)
        {
            var map = sprites.Key;
            if (sprites.Key.ActiveAnchors.Count == 0 && unit.Map != map)
                continue;

            List<SpriteObstacle> spritesCopy;
            lock (_lock)
            {
                spritesCopy = sprites.Value;
            }

            for (int i = spritesCopy.Count - 1; i >= 0; i--)
            {
                var sprite = spritesCopy[i];
                bool skipRendering = sprite == unit || (unit.Map != map && map.ActiveAnchors.Count > 0);

                if (skipRendering)
                {
                    sprite.Animation.Update(0);
                    continue;
                }

                sprite.Render(result, unit);
            }
        }
    }
    #endregion

    #region ITextureProvider
    public override TextureWrapper? GetUsedTexture(IUnit? observer = null)
    {
       return Animation.CurrentFrame?.GetElement(0);
    }
    #endregion

    public override IObject GetCopy()
    {
        return new SpriteObstacle(this);
    }
    public override IObject GetDeepCopy()
    {
        return new SpriteObstacle(this, new ImageLoadOptions() { CreateNew = true });
    }
    public virtual double GetDisplayAngle(double spriteAngle)
    {
        return spriteAngle;
    }

    private bool IsVisibleToUnit(IUnit unit, out double spriteAngle)
    {
        Vector2f pos = new Vector2f((float)X.Axis, (float)Y.Axis);
        spriteAngle = MathUtils.CalculateAngleToTarget(pos, unit.OriginPosition);
        Distance = MathUtils.CalculateDistance(pos, unit.OriginPosition);

        if (Distance > unit.MaxRenderTile)
            return false;

        AngleToObserver = MathUtils.NormalizeAngleDifference(unit.Angle, spriteAngle);
        return Math.Abs(AngleToObserver) <= unit.Fov;
    }
    public override void Render(Result result, IUnit unit)
    {
        if (!IsVisibleToUnit(unit, out double spriteAngle))
        {
            Animation.Update(0);
            return;
        }

        Distance = Math.Max(Distance, PersonalMinDistance ?? GlobalMinDistance);
        Animation.Update((float)GetDisplayAngle(spriteAngle));
        float height = (float)(Screen.ScreenHeight / ViewDistance) * Scale;

        RenderOperation.DrawSprite(this, unit, height);
    }
}
