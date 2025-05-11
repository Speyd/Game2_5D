using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;
using ScreenLib.SettingScreen;
using SFML.System;
using SFML.Window;
using HitBoxLib.PositionObject;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.Data.Observer;
using ProtoRender.Object;
using HitBoxLib.Operations;
using ProtoRender.RenderAlgorithm;
using TextureLib;
using SFML.Graphics;
using HitBoxLib.Data.HitBoxObject;


namespace EntityLib;
/// <summary>Entity</summary>
//public class Entity: IUnit
//{
//    public bool IsPassability { get; set; } = false;
//    public bool IsSingleAddable { get; set; } = false;

//    public float SizeScale {  get; set; }
//    public float  PositionScale { get; set; }
//    public SFML.Graphics.Color ColorInMap { get; set; }
//    public TextureObstacle TextureInMiniMap { get; set; }

//    /// <summary> Entity position with Screen.Setting.Tile multiplier </summary>
//    public Vector2f Position
//    {
//        get
//        {
//            return new Vector2f((float)X.Axis, (float)Y.Axis) / Screen.Setting.Tile;
//        }
//    }

//    /// <summary> Entity position</summary>
//    public Vector2f OriginPosition
//    {
//        get
//        {
//            return new Vector2f((float)X.Axis, (float)Y.Axis);
//        }
//    }

//    //-----------------Fov-----------------
//    private double _fov;
//    /// <summary>Fov Entity</summary>
//    public double Fov 
//    { 
//        get => _fov;
//        set
//        {
//            _fov = value;
//            HalfFov = value / 2;
//        }
//    }
//    /// <summary>Half Fov Entity</summary>
//    public double HalfFov { get; private set; }


//    //---------------------Ray Setting----------------------
//    /// <summary>Maximum rendering distance(depends on Screen.Setting.Tile)</summary>
//    public double MaxRenderTile { get; set; }

//    //---------------------Render Setting----------------------
//    /// <summary>This is the angle between adjacent rays in the rendering system</summary>
//    public double DeltaAngle { get; private set; }
//    /// <summary>Projected height(depends on the distance between the Entity and the object)</summary>
//    public double ProjCoeff { get; private set; }


//    //---------------------Coordinates----------------------
//    /// <summary>Hit Box Entity</summary>
//    public HitBox HitBox { get; init; }
//    /// <summary>X axis coordinate</summary>
//    public Coordinate X { get; init; }
//    /// <summary>Y axis coordinate</summary>
//    public Coordinate Y { get; init; }
//    /// <summary>Z axis coordinate</summary>
//    public Coordinate Z { get; init; }


//    //-----------------------Angle-----------------------
//    /// <summary>X - Cos(Angle); Y - Sin(Angle)</summary>
//    public Vector2f Direction { get; protected set; }
//    /// <summary>X - -Sin(Angle); Y - Cos(Angle)</summary>
//    public Vector2f Plane { get; protected set; }

//    private double _angle;
//    /// <summary>Angle Entity(Horizontal axis)</summary>
//    public double Angle
//    {
//        get => _angle;
//        set
//        {
//            if (_angle != value)
//            {
//                _angle = value;
//                float cos = (float)Math.Cos(_angle);
//                float sin = (float)Math.Sin(_angle);
//                Direction = new Vector2f(cos, sin);
//                Plane = new Vector2f(-sin, cos);
//            }
//        }
//    }
//    /// <summary>Vertical Angle Entity(Vertical axis)</summary>
//    public double VerticalAngle { get; set; }
//    public Action<IObject, double, double>? OnPositionChanged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
//    int ProtoRender.Object.IObserver.MaxRenderTile { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }



//    /// <summary>Entity сlass constructor</summary>
//    public Entity(double x = 0, double y = 0, double z = 0, double fov = Math.PI / 3)
//    {
//        Fov = fov;
//        HalfFov = (float)Fov / 2;

//        HitBox = new HitBox();
//        X = new Coordinate(CoordinatePlane.X, x, HitBox);
//        Y = new Coordinate(CoordinatePlane.Y, y, HitBox);
//        Z = new Coordinate(CoordinatePlane.Z, z, HitBox);
//        Z.Axis = 50;

//        Angle = 0;
//        VerticalAngle = 0;

//        EntitySettingChangesFun();
//        Screen.WidthChangesFun += EntitySettingChangesFun;
//    }
//    public Entity(Entity entity)
//    {
//        Fov = entity.Fov;
//        HalfFov = entity.HalfFov;
//        HitBox = new HitBox(entity.HitBox);
//        X = new Coordinate(entity.X);
//        Y = new Coordinate(entity.Y);
//        Z = new Coordinate(entity.Z);

//        Angle = entity.Angle;
//        VerticalAngle = entity.VerticalAngle;
//        SizeScale = entity.SizeScale;
//        PositionScale = entity.PositionScale;
//        ColorInMap = entity.ColorInMap;
//        TextureInMiniMap = entity.TextureInMiniMap;

//        EntitySettingChangesFun();
//        Screen.WidthChangesFun += EntitySettingChangesFun;
//    }
//    public void Render(Result result, IUnit unit)
//    {

//    }


//    public IObject GetCopy() => new Entity(this);
//    private void EntitySettingChangesFun()
//    {
//        float dist = Screen.Setting.AmountRays / (2 * (float)Math.Tan(HalfFov));
//        ProjCoeff = dist * Screen.Setting.Tile;

//        DeltaAngle = (float)Fov / Screen.Setting.AmountRays;
//    }
//    /// <summary>Returns basic information about the observer</summary>
//    public ObserverInfo GetObserverInfo()
//    {
//        ObserverInfo observerInfo = new ObserverInfo();
//        observerInfo.fov = Fov;
//        observerInfo.verticalAngle = VerticalAngle;
//        observerInfo.angle = Angle;
//        observerInfo.deltaAngle = DeltaAngle;
//        observerInfo.position = new Vector3f((float)X.Axis, (float)Y.Axis, (float)Z.Axis);

//        return observerInfo;
//    }
//    /// <summary>Calculating the next step position</summary>
//    public (double nextX, double nextY) CalculateNextPosition(double deltaX, double deltaY)
//    {
//        double nextX = X.Axis + deltaX;
//        double nextY = Y.Axis + deltaY;
//        return (nextX, nextY);
//    }

//    public float WorldToScreenY(double angleVertical, float addVariable = 0)
//    {
//        throw new NotImplementedException();
//    }

//    public float WorldToScreenX(double Angle, double DeltaAngle)
//    {
//        throw new NotImplementedException();
//    }

//    public float WorldToScreenX(int ray)
//    {
//        throw new NotImplementedException();
//    }

//    public Vector2f GetPositionOnScreen(Result result, ProtoRender.Object.IObserver observer)
//    {
//        throw new NotImplementedException();
//    }

//    public void FillingColorShape(RectangleShape rectangleShape, float OutlineThickness = 1)
//    {
//        throw new NotImplementedException();
//    }

//    public void FillingTextureShape(RectangleShape rectangleShape)
//    {
//        throw new NotImplementedException();
//    }

//    public float SizeOffsetMap(float baseOffset)
//    {
//        throw new NotImplementedException();
//    }

//    public float CoordinatesOffsetMap(float baseOffset)
//    {
//        throw new NotImplementedException();
//    }

//    public Vector2f ConversionToMapCoordinates(float mapTile)
//    {
//        throw new NotImplementedException();
//    }

//    public float WorldToScreenSideY(double side, double distance, double verticalAngle, double angleObject)
//    {
//        throw new NotImplementedException();
//    }

//    public RenderInfo GetRenderHitBoxInfo()
//    {
//        throw new NotImplementedException();
//    }

//    public void HandleObjectAddition(double x, double y, bool resetHitBoxSide)
//    {
//        throw new NotImplementedException();
//    }

//    public void ObserverSettingChangesFun()
//    {
//        throw new NotImplementedException();
//    }
//}
