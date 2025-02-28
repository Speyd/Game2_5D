using SFML.Graphics;
using EntityLib;
using ScreenLib;
using SFML.System;
using Render.RenderInterface;
using HitBoxLib.PositionObject;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.Data.HitBoxObject;
using Render.RenderAlgorithm;
using Render.Map;


namespace ObstacleLib;
public abstract class Obstacle : IRenderable, IMiniMapRenderable, IHitBoxProcessor
{
    public Action<Obstacle, double, double>? OnPositionChanged;
    
    public HitBox HitBox { get; set; } = new HitBox();
    public Coordinate X { get; init; }
    public Coordinate Y { get; init; }
    public Coordinate Z { get; init; }


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

    public void SetShifts(double shifts)
    {
        ShiftCubedX = shifts;
        ShiftCubedY = shifts;
    }
    public void SetShifts(double shiftsX, double shiftsY)
    {
        ShiftCubedX = shiftsX;
        ShiftCubedY = shiftsY;
    }
    #endregion


    //----------------------Map Setting-----------------
    public SFML.Graphics.Color ColorInMap { get; set; }
    /// <summary>Positioning will be from the center of the object and not from the top corner</summary>
    public virtual bool IsOffsetMap { get; set; } = false;


    //-------------------Collision Setting--------------------
    /// <summary>The passability of an object through the current object</summary>
    public bool IsPassability { get; set; }
    /// <summary>Possibility to add an object to the same cell where the current object is located</summary>
    public virtual bool IsSingleAddable { get; init; } = true;



    public Obstacle(double x, double y, SFML.Graphics.Color colorInMap, bool isPassability)
    {
        ColorInMap = colorInMap;
        IsPassability = isPassability;

        X = new Coordinate(CoordinatePlane.X, HitBox);
        Y = new Coordinate(CoordinatePlane.Y, HitBox);
        Z = new Coordinate(CoordinatePlane.Z, HitBox);
    }



    public abstract void Render(Result result, Entity entity);
    public abstract void UpdateAdditionalInformation(double x, double y);

    public abstract  double GetZCoordinate();
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
    public abstract Vector2f GetPositionOnScreen(Result result, Entity entity);


    public abstract void FillingColorShape(RectangleShape rectangleShape, float OutlineThickness = 1);
    public abstract void FillingTextureShape(RectangleShape rectangleShape);


    public abstract float CoordinatesOffsetMap(float baseOffset);
    public abstract Vector2f ConversionToMapCoordinates(float mapTile);


    public virtual float WorldToScreenSideY(double side, double distance, double verticalAngle, double angleObject)
    {
        distance /= Screen.Setting.Tile;
        distance *= Math.Cos(angleObject);
        distance  = Math.Max(distance, 0.4);
        return WorldToScreenY(verticalAngle) - (float)(side / distance);
    }

    public virtual HitboxObjectInfo GetHitboxObjectInfo()
    {
        HitboxObjectInfo hitboxObjectInfo = new HitboxObjectInfo();
        hitboxObjectInfo.position = new Vector2f((float)X.Axis, (float)Y.Axis);
        hitboxObjectInfo.body = HitBox.MainHitBox;
        hitboxObjectInfo.worldToScreenY = WorldToScreenSideY;
        hitboxObjectInfo.worldToScreenX = WorldToScreenX;

        return hitboxObjectInfo;
    }
}
