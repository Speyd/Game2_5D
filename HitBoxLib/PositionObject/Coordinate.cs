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



    private float stopTimer = 0f;
    /// <summary>
    /// The delay in seconds after which the movement is considered stopped.
    /// </summary>
    
    public float StopDelay { get; set; } = 0.1f;
    /// <summary>
    /// CancellationTokenSource used for stopping the movement watcher task.
    /// </summary>
    private CancellationTokenSource? stopCts = null;

    /// <summary>
    /// Interval in milliseconds at which the movement watcher checks if the object has stopped.
    /// </summary>
    public static int StopWatcherIntervalMs { get; set; } = 10;


    /// <summary>
    /// Indicates whether the coordinate is currently moving.
    /// </summary>
    public bool IsMoving { get; private set; } = false;

    /// <summary>
    /// Action invoked when movement starts.
    /// </summary>
    public Action? OnStartMove { get; set; }

    /// <summary>
    /// Action invoked when movement stops.
    /// </summary>
    public Action? OnStopMove { get; set; }



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
        StopDelay = coordinate.StopDelay;
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
        if (!IsMoving && PreviousAxis != value)
        {
            IsMoving = true;
            OnStartMove?.Invoke();
        }

        ResetWatcher();

        _axis = value;
        AxisMap = ScreenLib.Screen.Mapping(value) / ScreenLib.Screen.Setting.Tile;

        hitBox.SetSide(this);
        AfterMoveAxis?.Invoke();
    }

    private void ResetWatcher()
    {
        stopTimer = 0f;

        stopCts?.Cancel();
        stopCts = new CancellationTokenSource();
        var token = stopCts.Token;

        StartStopWatcher(token);
    }
    private void StartStopWatcher(CancellationToken token)
    {
        Task.Run(async () =>
        {
            while (!token.IsCancellationRequested && IsMoving)
            {
                await Task.Delay(StopWatcherIntervalMs, token);
                stopTimer += FpsLib.FPS.GetDeltaTime();

                if (stopTimer >= StopDelay)
                {
                    IsMoving = false;
                    OnStopMove?.Invoke();
                    break;
                }
            }
        }, token);
    }


    public void SetHitBox(HitBox hitBox)
    {
        _hitBox = hitBox;
        hitBox.SetSide(this);
    }
}
