using FpsLib;
using ProtoRender.Map;
using ProtoRender.Object;

namespace MoveLib.Move;
/// <summary>
/// Provides functionality to move game units on the map with respect to direction,
/// movement speed, and collision detection.
/// </summary>
public static class MovePositions
{
    /// <summary>
    /// Attempts to move the unit in a given direction with collision detection,
    /// based on its speed and current facing direction.
    /// </summary>
    /// <param name="map">The game map containing obstacles.</param>
    /// <param name="unit">The unit to move.</param>
    /// <param name="directionX">
    /// The horizontal component of the input direction (e.g., -1 for left, 1 for right).
    /// </param>
    /// <param name="directionY">
    /// The vertical component of the input direction (e.g., -1 for backward, 1 for forward).
    /// </param>
    public static void Move(IMap map, IUnit unit, double directionX, double directionY)
    {

        double speed = unit.MoveSpeed * FPS.GetDeltaTime();

        double cosAngle = unit.Direction.X;
        double sinAngle = unit.Direction.Y;

        double rx = cosAngle * directionX - sinAngle * directionY;
        double ry = sinAngle * directionX + cosAngle * directionY;

        rx *= speed;
        ry *= speed;

        lock (unit)
        {
            Collision.IsCollision(map, unit, rx, ry, unit.IgnoreCollisionObjects.Keys.ToList());
        }
    }
}
