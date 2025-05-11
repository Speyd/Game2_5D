using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProtoRender.Object;
/// <summary>
/// Defines an entity that can move in the game world.
/// </summary>
public interface IMovable
{
    /// <summary>
    /// Current movement speed of the entity (units per second).
    /// </summary>
    public float MoveSpeed { get; set; }

    /// <summary>
    /// Rotation speed modifier based on frame rate, used to normalize mouse input.
    /// </summary>
    public double MoveSpeedAngel { get; set; }

    /// <summary>
    /// Temporary storage for vertical camera angle (pitch).
    /// </summary>
    public double TempVerticalAngle { get; set; }

    /// <summary>
    /// Temporary storage for horizontal camera angle (yaw).
    /// </summary>
    public double TempAngle { get; set; }

    /// <summary>
    /// Minimum allowed distance between the entity and nearby obstacles (collision padding).
    /// </summary>
    public float MinDistanceFromWall { get; set; }

    /// <summary>
    /// Mouse sensitivity multiplier used for rotating the camera or character.
    /// </summary>
    public float MouseSensitivity { get; set; }

    /// <summary>
    /// Indicates whether the mouse is locked (captured) within the game window.
    /// </summary>
    public bool IsMouseCaptured { get; set; }

    /// <summary>
    /// Minimum vertical camera angle (in radians). Prevents looking too far upward.
    /// </summary>
    public double MinVerticalAngle { get; set; }

    /// <summary>
    /// Maximum vertical camera angle (in radians). Prevents looking too far downward.
    /// </summary>
    public double MaxVerticalAngle { get; set; }
    /// <summary>
    /// Thread-safe collection of object to ignore during processing.
    /// </summary>
    ConcurrentDictionary<IObject, byte> IgnoreCollisionObjects { get; set; }
}
