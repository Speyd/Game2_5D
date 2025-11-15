using SFML.Graphics;
using ScreenLib;
using SFML.System;
using ProtoRender.RenderInterface;
using HitBoxLib.PositionObject;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.Data.HitBoxObject;
using ProtoRender.RenderAlgorithm;
using ProtoRender.Map;
using TextureLib.Textures;
using ProtoRender.Object;
using EffectLib.EffectCore;


namespace ObstacleLib;
public abstract class Obstacle : IObject, IEffectUser
{
    /// <summary>
    /// Globally unique identifier for the object.
    /// </summary>
    public Guid UUID { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Event invoked whenever the object's position changes.
    /// Subscribers can use this to update coordinates, collision, or other map-related logic.
    /// </summary>
    public Action<IObject>? OnPositionChanged { get; set; }
    protected IMap? _map = null;

    /// <summary>
    /// Gets or sets the map to which this object belongs.
    /// When the map is changed, the object is removed from the previous map
    /// and unsubscribed from its position update events. Setting a new map
    /// does not automatically add the object to the new map.
    /// </summary>
    public virtual IMap? Map 
    {
        get => _map;
        set
        {
            if (_map is not null && _map != value)
            {
                _map.DeleteObstacle(this);
               OnPositionChanged -= _map.UpdateCoordinatesObstacle;
            }

            _map = value;
        }
    }

    /// <summary>
    /// Gets the X coordinate of the object in world space.
    /// </summary>
    public virtual Coordinate X { get; init; }

    /// <summary>
    /// Gets the Y coordinate of the object in world space.
    /// </summary>
    public virtual Coordinate Y { get; init; }
    
    /// <summary>
    /// Gets the Z coordinate of the object in world space.
    /// </summary>
    public virtual Coordinate Z { get; init; }

    /// <summary>
    /// Indicates whether the object is currently moving along any axis (X, Y, or Z).
    /// Returns <c>true</c> if any of the coordinates are moving; otherwise, <c>false</c>.
    /// </summary>
    public bool IsMoving => X.IsMoving || Y.IsMoving || Z.IsMoving;

    /// <summary>
    /// Gets or sets the column index of the map cell that contains the object,
    /// calculated using the map's tile size.
    /// </summary>
    public int CellX { get; set; }

    /// <summary>
    /// Gets or sets the row index of the map cell that contains the object,
    /// calculated using the map's tile size.
    /// </summary>
    public int CellY { get; set; }


    private HitBox _hitBox = new HitBox();
    /// <summary>
    /// Gets or sets the hitbox associated with this object.
    /// Setting the hitbox also updates the hitbox reference in the X, Y, and Z coordinates
    /// so that coordinate changes correctly update the object's collision boundaries.
    /// </summary>
    public virtual HitBox HitBox 
    {
        get => _hitBox;
        set
        {
            _hitBox = value;
            X.SetHitBox(value);
            Y.SetHitBox(value);
            Z.SetHitBox(value);
        }
    } 


    //--------------------Shift-------------------------
    #region Shift
    private double shiftCubedX = 0;
    /// <summary>Offset of an object along the current map cell(On the X axis)</summary>
    public double ShiftCubedX
    {
        get => shiftCubedX;
        set
        {
            shiftCubedX = value < 0 ? 1 : value > 99 ? 99 : value;
            X.Axis = Screen.Mapping(X.Axis, Screen.Setting.Tile) + shiftCubedX;
        }
    }

    private double shiftCubedY = 0;
    /// <summary>Offset of an object along the current map cell(On the Y axis)</summary>
    public double ShiftCubedY
    {
        get => shiftCubedY;
        set
        {
            shiftCubedY = value < 0 ? 1 : value > 99 ? 99 : value;
            Y.Axis = Screen.Mapping(Y.Axis, Screen.Setting.Tile) + shiftCubedY;
        }
    }

    public static readonly SFML.Graphics.Color BaseEffectColor = new SFML.Graphics.Color(200, 200, 200);
    public virtual IEffect? Effect { get; set; } = null;

    public virtual void SetShifts(double shifts)
    {
        ShiftCubedX = shifts;
        ShiftCubedY = shifts;
    }
    public virtual void SetShifts(double shiftsX, double shiftsY)
    {
        ShiftCubedX = shiftsX;
        ShiftCubedY = shiftsY;
    }
    #endregion


    //------------------Map Setting-----------------

    /// <summary>
    /// Gets or sets the color of the object as displayed on the minimap.
    /// </summary>
    public virtual SFML.Graphics.Color ColorInMap { get; set; }

    /// <summary>
    /// Gets or sets the texture of the object for the minimap representation.
    /// Can be null if a plain color is used instead.
    /// </summary>
    public virtual TextureWrapper? TextureInMiniMap { get; set; }

    /// <summary>
    /// Gets or sets the scale factor for the size of the object on the minimap.
    /// 1.0 means original size, values greater or smaller scale the object proportionally.
    /// </summary>
    public virtual float SizeScale { get; set; } = 1;

    /// <summary>
    /// Gets or sets the scale factor for the position of the object on the minimap.
    /// Adjusts how the object's coordinates map to the minimap.
    /// </summary>
    public virtual float PositionScale { get; set; } = 1;


    //-------------------Collision Setting--------------------
    /// <summary>The passability of an object through the current object</summary>
    public virtual bool IsPassability { get; set; } = false;
    /// <summary>
    /// If true, ignores collisions with objects marked as MainBox (primary bounding boxes),
    /// allowing them to be bypassed during collision checks.
    ///</summary>
    public virtual bool IgnoreCollisonMainBox { get; set; } = false;
    /// <summary>Possibility to add an object to the same cell where the current object is located</summary>
    public virtual bool IsSingleAddable { get; set; } = false;



    public Obstacle(SFML.Graphics.Color colorInMap, bool isPassability)
    {
        ColorInMap = colorInMap;
        IsPassability = isPassability;

        X = new Coordinate(CoordinatePlane.X, HitBox);
        X.AfterMoveAxis = UpdateCoordinate;

        Y = new Coordinate(CoordinatePlane.Y, HitBox);
        Y.AfterMoveAxis = UpdateCoordinate;

        Z = new Coordinate(CoordinatePlane.Z, HitBox);
    }
    public Obstacle(Obstacle obstacle)
        :this(obstacle.ColorInMap, obstacle.IsPassability)
    {
        HitBox = new HitBox(obstacle.HitBox);

        X.UpdateInfo(obstacle.X, HitBox);
        Y.UpdateInfo(obstacle.Y, HitBox);
        Z.UpdateInfo(obstacle.Z, HitBox);

        shiftCubedX = obstacle.ShiftCubedX;
        shiftCubedY = obstacle.shiftCubedY;

        ColorInMap = obstacle.ColorInMap;
        if(obstacle.TextureInMiniMap is not null)
            TextureInMiniMap = new TextureWrapper(obstacle.TextureInMiniMap.PathTexture, true); ;

        IsPassability = obstacle.IsPassability;
        IsSingleAddable = obstacle.IsSingleAddable;
        IgnoreCollisonMainBox = obstacle.IgnoreCollisonMainBox;

        SizeScale = obstacle.SizeScale;
        PositionScale = obstacle.PositionScale;
        Effect = obstacle.Effect;
    }

    private void UpdateCoordinate()
    {
        OnPositionChanged?.Invoke(this);
    }
    public abstract void Render(Result result, IUnit unit);
    public abstract IObject GetCopy();
    public abstract IObject GetDeepCopy();


    #region IMapAdder
    public abstract void HandleObjectAddition(double x, double y, bool resetHitBoxSide);
    #endregion

    #region IRenderable
    public virtual float WorldToScreenY(double angleVertical, float addVariable = 0)
    {
        if (angleVertical <= 0)
            return (float)(Screen.Setting.HalfHeight - Screen.Setting.HalfHeight * angleVertical - addVariable);
        else
        {
            angleVertical += 1;

            return (float)(Screen.Setting.HalfHeight / angleVertical - addVariable);
        }
    }
    public virtual float WorldToScreenX(double Angle, double DeltaAngle)
    {
        int delta_rays = (int)(Angle / DeltaAngle);
        int current_ray = Screen.Setting.CenterRay + delta_rays;


        return current_ray;
    }
    public virtual float WorldToScreenX(int ray)
    {
        return (float)ray * Screen.Setting.Scale;
    }
    public abstract CoordinateOnScreen GetPositionOnScreen(Result result, IUnit unit);
    #endregion

    #region IMiniMapRenderable
    public abstract void FillingColorShape(RectangleShape rectangleShape, float OutlineThickness = 1);
    public abstract void FillingTextureShape(RectangleShape rectangleShape);
    public abstract Vector2f ConversionToMapCoordinates(Vector2f mapTile);
    public virtual Vector2f CoordinatesOffsetMap(Vector2f baseOffset) => baseOffset / PositionScale;
    public virtual Vector2f SizeOffsetMap(Vector2f baseOffset) => baseOffset / SizeScale;
    #endregion

    #region IHitBoxProcessor
    public virtual float WorldToScreenSideY(double side, double distance, double verticalAngle, double angleObject)
    {
        distance /= Screen.Setting.Tile;
        distance *= Math.Cos(angleObject);
        distance  = Math.Max(distance, IHitBoxProcessor.minDistance);
        return WorldToScreenY(verticalAngle) - (float)(side / distance);
    }
    public virtual RenderInfo GetRenderHitBoxInfo()
    {
        RenderInfo hitboxObjectInfo = new RenderInfo();
        hitboxObjectInfo.position = new Vector3f((float)X.Axis, (float)Y.Axis, (float)Z.Axis);
        hitboxObjectInfo.hitBox = HitBox;
        hitboxObjectInfo.worldToScreenY = WorldToScreenSideY;
        hitboxObjectInfo.worldToScreenX = WorldToScreenX;

        return hitboxObjectInfo;
    }
    #endregion

    #region ITextureProvider
    public abstract TextureWrapper? GetUsedTexture(IUnit? observer = null);
    #endregion
}
