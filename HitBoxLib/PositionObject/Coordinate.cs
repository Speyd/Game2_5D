using HitBoxLib.HitBoxSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HitBoxLib.PositionObject;
public class Coordinate
{
    private double _axis = 0;
    /// <summary>Axis coordinate</summary>
    public double Axis
    {
        get => _axis;
        set => SetAxis(value, _hitBox);
    }
    /// <summary>Determining which axis an object belongs to</summary>
    public CoordinatePlane CoordinatePlane { get; set; }
    private HitBox _hitBox;

    public Coordinate(CoordinatePlane coordinatePlane, HitBox hitBox)
    {
        CoordinatePlane = coordinatePlane;
        _hitBox = hitBox;
    }
    public Coordinate(CoordinatePlane coordinatePlane, double axis, HitBox hitBox)
    {
        CoordinatePlane = coordinatePlane;
        _hitBox = hitBox;

        Axis = axis;
    }
    public Coordinate(Coordinate coordinate)
    {
        CoordinatePlane = coordinate.CoordinatePlane;
        _axis = coordinate._axis;
        _hitBox = new HitBox(coordinate._hitBox);
    }
    public Coordinate(Coordinate coordinate, HitBox hitBox)
    {
        CoordinatePlane = coordinate.CoordinatePlane;
        _axis = coordinate._axis;
        _hitBox = hitBox;
    }

    public void SetAxis(double value, HitBox hitBox)
    {
        _axis = value;
        hitBox.SetSide(this);
    }
}
