using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp;
using System.Runtime.InteropServices;
using EntityLib;
using ScreenLib;
using EntityLib.Player;
using SFML.System;
using ObstacleLib.SpriteLib.Animation;
using ObstacleLib.SpriteLib.Render;
using TextureLib;
using ObstacleLib.SpriteLib.Add;
using System.Drawing;
using System.Numerics;
using ProtoRender.RenderInterface;
using System.Reflection.Metadata;
using System.Text;
using DataPipes.Pool;
using HitBoxLib;
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
    private bool IsAdded { get; set; } = false;


    //---------------------------Textures------------------------------
    public AnimationState Animation { get; init; } = new();
    public SFML.Graphics.Sprite RenderSprite { get; set; } = new SFML.Graphics.Sprite();


    //--------------------------Setting-----------------------------
    public override bool IsSingleAddable { get; init; } = false;

    private float scale = 1;
    /// <summary>Texture scale</summary>
    public float Scale
    {
        get => scale * Screen.ScreenRatio;
        set => scale = value == 0 ? 1 : value;
    }
    public override float SizeScale { get; set; } = 2;
    public override float PositionScale { get; set; } = 4;
    //---------------------Render Parameters----------------------
    /// <summary>Angle relative to this object and the observer</summary>
    public double Angle { get; set; }
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
    public SpriteObstacle(string path, bool isDirectory, bool isPassability = false, bool folderAccounting = false)
       : base(0, 0, SFML.Graphics.Color.White, isPassability)
    {
        if(isDirectory)
            Adder.AddTextureFromFolder(this, path, folderAccounting);
        else
            Adder.AddTexture(this, path);
    }
    public SpriteObstacle(List<string> paths, bool isPassability = false)
       : base(0, 0, SFML.Graphics.Color.White, isPassability)
    {
        Adder.AddTextures(this, paths);
    }
    public SpriteObstacle(SpriteObstacle spriteObstacle)
    : base(0, 0, SFML.Graphics.Color.Black, false)
    {
        HitBox = new HitBox(spriteObstacle.HitBox);

        X = new Coordinate(spriteObstacle.X, HitBox);
        Y = new Coordinate(spriteObstacle.Y, HitBox);
        Z = new Coordinate(spriteObstacle.Z, HitBox);

        ColorInMap = spriteObstacle.ColorInMap;
        TextureInMiniMap = spriteObstacle.TextureInMiniMap is not null ? new TextureObstacle(spriteObstacle.TextureInMiniMap) : null;

        IsPassability = spriteObstacle.IsPassability;
        IsSingleAddable = spriteObstacle.IsSingleAddable;

        SizeScale = spriteObstacle.SizeScale;
        PositionScale = spriteObstacle.PositionScale;

        Scale = spriteObstacle.Scale;
        Angle = spriteObstacle.Angle;
        Distance = spriteObstacle.Distance;

        Animation = spriteObstacle.Animation;
        IsAdded = spriteObstacle.IsAdded;
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
    public override Vector2f GetPositionOnScreen(Result result, Entity entity)
    {
        Distance = result.Depth;

        float height = (float)(Screen.ScreenHeight / Distance * Scale);
        return RenderOperation.GetPositionOnScreen(this, entity, height);
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
        if (IsAdded == true)
            return;
        else if (SpritesToRender.Contains(this))
            return;

        SpritesToRender.Add(this);
        SpritesToRender = SpritesToRender
            .OrderByDescending(sprite => sprite.Distance)
            .ToList();

        IsAdded = true;
    }
    public static void RenderSelfDrawableList(Result result, Entity entity)
    {
        if(SpritesToRender.Count == 0) 
            return;

        foreach (var sprite in SpritesToRender)
        {
            sprite.Render(result, entity);
        }
    }
    #endregion

    public override IObject GetCopy()
    {
        return new SpriteObstacle(this);
    }
    public override void Render(Result result, Entity entity)
    {
        Vector2f pos = new Vector2f((float)X.Axis, (float)Y.Axis);
        double spriteAngle = MathUtils.CalculateAngleToTarget(pos, entity.OriginPosition);
        Distance = MathUtils.CalculateDistance(pos, entity.OriginPosition);

        if (Distance > entity.MaxRenderTile)
            return;

        Angle = MathUtils.NormalizeAngleDifference(entity.Angle, spriteAngle);

        if (Math.Abs(Angle) <= entity.Fov)
        {
            AnimationManager.DefiningDesiredSprite(Animation, spriteAngle);

            Distance *= Math.Cos(Angle);
            Distance = Math.Max(Distance, 0.1);
            float height = (float)(Screen.ScreenHeight / Distance * Scale);

            RenderOperation.DrawSprite(this, entity, height);
        }
        
    }
}
