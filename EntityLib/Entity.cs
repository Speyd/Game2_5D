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
using static System.Runtime.InteropServices.JavaScript.JSType;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.Data.Observer;


namespace EntityLib;
public class Entity: IObserver
{
    /// <summary> Entity position with Screen.Setting.Tile multiplier </summary>
    public Vector2f Position
    {
        get
        {
            return new Vector2f((float)X.Axis, (float)Y.Axis) / Screen.Setting.Tile;
        }
    }

    /// <summary> Entity position</summary>
    public Vector2f OriginPosition
    {
        get
        {
            return new Vector2f((float)X.Axis, (float)Y.Axis);
        }
    }

    //-----------------Fov-----------------
    private double _fov;
    public double Fov 
    { 
        get => _fov;
        set
        {
            _fov = value;
            HalfFov = value / 2;
        }
    }
    public double HalfFov { get; private set; }


    //---------------------Ray Setting----------------------
    /// <summary>Maximum rendering distance(depends on Screen.Setting.Tile)</summary>
    public double MaxRenderTile { get; set; }

    //---------------------Render Setting----------------------
    /// <summary>This is the angle between adjacent rays in the rendering system</summary>
    public double DeltaAngle { get; private set; }
    /// <summary>Projected height(depends on the distance between the Entity and the object)</summary>
    public double ProjCoeff { get; private set; }


    //---------------------Coordinates----------------------
    public HitBox HitBox { get; init; }
    public Coordinate Y { get; init; }
    public Coordinate X { get; init; }
    public Coordinate Z { get; init; }


    //-----------------------Angle-----------------------
    /// <summary>X - Cos(Angle); Y - Sin(Angle)</summary>
    public Vector2f Direction { get; private set; }
    /// <summary>X - -Sin(Angle); Y - Cos(Angle)</summary>
    public Vector2f Plane { get; private set; }

    private double _angle;
    /// <summary>Angle Entity(Horizontal axis)</summary>
    public double Angle
    {
        get => _angle;
        set
        {
            if (_angle != value)
            {
                _angle = value;
                float cos = (float)Math.Cos(_angle);
                float sin = (float)Math.Sin(_angle);
                Direction = new Vector2f(cos, sin);
                Plane = new Vector2f(-sin, cos);
            }
        }
    }
    /// <summary>Vertical Angle Entity(Vertical axis)</summary>
    public double VerticalAngle { get; set; }




    public Entity(Setting setting, double maxDistance,
        double fov = Math.PI / 3,
        double x = 0, double y = 0,
        double angle = 0, double verticalAngle = 0)
    {
        Fov = fov;
        HalfFov = (float)Fov / 2;

        HitBox = new HitBox();
        X = new Coordinate(CoordinatePlane.X, HitBox);
        Y = new Coordinate(CoordinatePlane.Y, HitBox);
        Z = new Coordinate(CoordinatePlane.Z, HitBox);
        Z.Axis = 50;

        Angle = angle;
        VerticalAngle = verticalAngle;

        EntitySettingChangesFun();
        Screen.WidthChangesFun += EntitySettingChangesFun;
    }


    private void EntitySettingChangesFun()
    {
        float dist = Screen.Setting.AmountRays / (2 * (float)Math.Tan(HalfFov));
        ProjCoeff = dist * Screen.Setting.Tile;

        DeltaAngle = (float)Fov / Screen.Setting.AmountRays;
    }
    public ObserverInfo GetObserverInfo()
    {
        ObserverInfo observerInfo = new ObserverInfo();
        observerInfo.fov = Fov;
        observerInfo.vertivalAngle = VerticalAngle;
        observerInfo.angle = Angle;
        observerInfo.deltaAngle = DeltaAngle;
        observerInfo.position = OriginPosition;

        return observerInfo;
    }
    public (double nextX, double nextY) CalculateNextPosition(double deltaX, double deltaY)
    {
        double nextX = X.Axis + deltaX;
        double nextY = Y.Axis + deltaY;
        return (nextX, nextY);
    }
}
