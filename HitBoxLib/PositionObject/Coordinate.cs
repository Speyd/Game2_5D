using HitBoxLib.HitBoxSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HitBoxLib.PositionObject;
/// <summary>Coordinat object</summary>
public class Coordinate
{
    private double _axis = 0;
    /// <summary>Axis coordinate</summary>
    public double Axis
    {
        get => _axis;
        set => SetAxis(value, _hitBox);
    }
    /// <summary>Stores a list of methods that are called when the coordinate changes.</summary>
    public Action? AfterMoveAxis {  get; set; }
    /// <summary>
    /// Stores a list of methods that are called before the coordinate changes.
    /// </summary>
    public Action? BeforeMoveAxis { get; set; }
    /// <summary>
    /// Stores the previous value of the axis before the most recent change.
    /// </summary>
    public double? PreviousAxis { get; set; } = null;
    /// <summary>Axis coordinate in map</summary>
    public double AxisMap { get; private set; }
    /// <summary>Determining which axis an object belongs to</summary>
    public CoordinatePlane CoordinatePlane { get; set; }
    private HitBox _hitBox;

    /// <summary>
    /// Initializes a new instance of the <see cref="Coordinate"/> class with the specified coordinate plane and hitbox.
    /// </summary>
    /// <param name="coordinatePlane">The coordinate plane (e.g., X, Y, or Z).</param>
    /// <param name="hitBox">The hitbox associated with this coordinate.</param>
    public Coordinate(CoordinatePlane coordinatePlane, HitBox hitBox)
    {
        CoordinatePlane = coordinatePlane;
        _hitBox = hitBox;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Coordinate"/> class with the specified coordinate plane, axis value, and hitbox.
    /// </summary>
    /// <param name="coordinatePlane">The coordinate plane (e.g., X, Y, or Z).</param>
    /// <param name="axis">The value along the specified coordinate plane.</param>
    /// <param name="hitBox">The hitbox associated with this coordinate.</param>
    public Coordinate(CoordinatePlane coordinatePlane, double axis, HitBox hitBox)
    {
        CoordinatePlane = coordinatePlane;
        _hitBox = hitBox;

        Axis = axis;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Coordinate"/> class by copying another coordinate and its hitbox.
    /// </summary>
    /// <param name="coordinate">The coordinate to copy from.</param>
    public Coordinate(Coordinate coordinate)
    {
        CoordinatePlane = coordinate.CoordinatePlane;

        _hitBox = new HitBox(coordinate._hitBox);
        Axis = coordinate._axis;

        PreviousAxis = coordinate.PreviousAxis;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Coordinate"/> class by copying the coordinate plane and axis from another coordinate, but using a different hitbox.
    /// </summary>
    /// <param name="coordinate">The coordinate to copy plane and axis from.</param>
    /// <param name="hitBox">The hitbox to use for the new coordinate.</param>
    public Coordinate(Coordinate coordinate, HitBox hitBox)
    {
        CoordinatePlane = coordinate.CoordinatePlane;

        _hitBox = hitBox;
        Axis = coordinate._axis;

        PreviousAxis = coordinate.PreviousAxis;
    }
    /// <summary>
    /// Update info HitBox.
    /// </summary>
    /// <param name="coordinate">The coordinate to copy from.</param>
    public void UpdateInfo(Coordinate coordinate)
    {
        CoordinatePlane = coordinate.CoordinatePlane;

        _hitBox = new HitBox(coordinate._hitBox);
        Axis = coordinate._axis;

        PreviousAxis = coordinate.PreviousAxis;
    }
    /// <summary>
    /// Update info HitBox.
    /// </summary>
    /// <param name="coordinate">The coordinate to copy from.</param>
    public void UpdateInfo(Coordinate coordinate, HitBox hitBox)
    {
        CoordinatePlane = coordinate.CoordinatePlane;

        _hitBox = hitBox;
        Axis = coordinate._axis;

        PreviousAxis = coordinate.PreviousAxis;
    }

    /// <summary>Set axis</summary>
    public void SetAxis(double value, HitBox hitBox)
    {
        BeforeMoveAxis?.Invoke();

        PreviousAxis = PreviousAxis is null? value: _axis;
        _axis = value;
        AxisMap = ScreenLib.Screen.Mapping(value) / ScreenLib.Screen.Setting.Tile;

        hitBox.SetSide(this);
        AfterMoveAxis?.Invoke();
    }

    public void SetHitBox(HitBox hitBox)
    {
        _hitBox = hitBox;
        hitBox.SetSide(this);
    }
}
