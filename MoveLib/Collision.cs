using ScreenLib;
using SFML.System;
using ProtoRender.Object;


namespace MoveLib.Move;
/// <summary>
/// Provides collision detection logic for units and obstacles on the map.
/// </summary>
public static class Collision
{
    private static int _radiusCheckTouch = Screen.Setting.Tile;

    /// <summary>
    /// Gets or sets the tile-based radius used to check nearby obstacles for collision.
    /// </summary>
    public static int RadiusCheckTouch
    {
        get => _radiusCheckTouch;
        set => _radiusCheckTouch = value * Screen.Setting.Tile;
    }

    /// <summary>
    /// Checks if there is any collision around the unit at the specified next position.
    /// Iterates over nearby obstacles within a radius and returns true if a collision is detected.
    /// </summary>
    /// <param name="subject">The IObject to check collisions for.</param>
    /// <param name="nextX">The candidate next X position of the unit.</param>
    /// <param name="nextY">The candidate next Y position of the unit.</param>
    /// <param name="ignoreList">List of objects to ignore in collision checks.</param>
    /// <param name="ignorePassability">If true, ignores obstacle passability during collision checks.</param>
    /// <returns>True if a collision is detected; otherwise false.</returns>
    private static bool FindFirstCollision(IObject subject, double nextX, double nextY, List<IObject> ignoreList, bool ignorePassability = false)
    {
        if (subject.Map is null)
            return false;

        var (cellX, cellY) = Screen.Mapping(subject.X.Axis, subject.Y.Axis);

        int minX = cellX - RadiusCheckTouch;
        int maxX = cellX + RadiusCheckTouch;
        int minY = cellY - RadiusCheckTouch;
        int maxY = cellY + RadiusCheckTouch;

        for (int x = minX; x <= maxX; x += Screen.Setting.Tile)
        {
            for (int y = minY; y <= maxY; y += Screen.Setting.Tile)
            {
                if (!subject.Map.Obstacles.TryGetValue((x, y), out var value))
                    continue;

                foreach (var obstacle in value.Keys)
                {
                    if (obstacle == subject || ignoreList.Contains(obstacle))
                        continue;

                    if (!obstacle.IgnoreCollisonMainBox)
                    {
                        if (CollisionHelper.IsCollidingAtPosition(subject, obstacle, obstacle.HitBox.MainHitBox, nextX, nextY, ignorePassability))
                            return true;
                    }
                    foreach (var box in obstacle.HitBox.SegmentedHitbox)
                    {
                        if (CollisionHelper.IsCollidingAtPosition(subject, obstacle, box, nextX, nextY, ignorePassability))
                            return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Finds the first obstacle the unit will collide with at the specified next position, and returns the collision point.
    /// </summary>
    /// <param name="subject">The unit to check collisions for.</param>
    /// <param name="nextX">The candidate next X position of the unit.</param>
    /// <param name="nextY">The candidate next Y position of the unit.</param>
    /// <param name="ignoreList">List of units to ignore in collision checks.</param>
    /// <param name="ignorePassability">If true, ignores obstacle passability during collision checks.</param>
    /// <returns>A tuple containing the colliding object and the collision point, or (null, null) if no collision.</returns>
    private static (IObject?, Vector3f?) GetFirstCollision(IObject subject, double nextX, double nextY, List<IUnit> ignoreList, bool ignorePassability = false)
    {
        if (subject.Map is null)
            return (null, null);

        var (cellX, cellY) = Screen.Mapping(subject.X.Axis, subject.Y.Axis);

        int minX = cellX - RadiusCheckTouch;
        int maxX = cellX + RadiusCheckTouch;
        int minY = cellY - RadiusCheckTouch;
        int maxY = cellY + RadiusCheckTouch;

        for (int x = minX; x <= maxX; x += Screen.Setting.Tile)
        {
            for (int y = minY; y <= maxY; y += Screen.Setting.Tile)
            {
                if (!subject.Map.Obstacles.TryGetValue((x, y), out var value))
                    continue;

                foreach (var obstacle in value.Keys)
                {
                    if (obstacle == subject || ignoreList.Contains(obstacle))
                        continue;

                    if (!obstacle.IgnoreCollisonMainBox)
                    {
                        var point = CollisionHelper.GetCollisionPointAt(subject, obstacle, obstacle.HitBox.MainHitBox, nextX, nextY, ignorePassability);
                        if (point != null)
                            return (obstacle, point);
                    }
                    foreach(var box in obstacle.HitBox.SegmentedHitbox)
                    {
                        var pointSegment = CollisionHelper.GetCollisionPointAt(subject, obstacle, box, nextX, nextY, ignorePassability);
                        if (pointSegment != null)
                            return (obstacle, pointSegment);
                    }
                }
            }
        }

        return (null, null);
    }
   
    
    /// <summary>
    /// Attempts to move the unit by the specified deltas and checks if a collision occurs with obstacles.
    /// </summary>
    /// <param name="subject">The IObject to move.</param>
    /// <param name="nextX">Delta movement in the X direction.</param>
    /// <param name="nextY">Delta movement in the Y direction.</param>
    /// <param name="ignoreList">List of objects to ignore during collision detection.</param>
    /// <param name="ignorePassability">If true, ignores obstacle passability.</param>
    /// <returns>True if a collision occurred; otherwise, false.</returns>
    public static bool IsCollisionObject(IObject subject, double nextX, double nextY, List<IObject> ignoreList, bool ignorePassability = false)
    {
        float minDistFromWall = subject is IMovable movable ? movable.MinDistanceFromWall / 2 : 0;
        double offsetX = minDistFromWall * Math.Sign(nextX);
        double offsetY = minDistFromWall * Math.Sign(nextY);

        bool collisionX = nextX != 0 && FindFirstCollision(subject, subject.X.Axis + nextX + offsetX, subject.Y.Axis, ignoreList, ignorePassability);
        bool collisionY = nextY != 0 && FindFirstCollision(subject, subject.X.Axis, subject.Y.Axis + nextY + offsetY, ignoreList, ignorePassability);

        if (nextX != 0 && !collisionX)
            subject.X.Axis += nextX;

        if (nextY != 0 && !collisionY)
            subject.Y.Axis += nextY;

        return collisionX || collisionY;
    }

    /// <summary>
    /// Attempts to move the unit by the specified deltas and returns the first obstacle and collision point encountered.
    /// </summary>
    /// <param name="subject">The IObject to move.</param>
    /// <param name="nextX">Delta movement in the X direction.</param>
    /// <param name="nextY">Delta movement in the Y direction.</param>
    /// <param name="ignoreList">List of units to ignore during collision detection.</param>
    /// <param name="ignorePassability">If true, ignores obstacle passability.</param>
    /// <returns>
    /// Tuple containing the colliding object and collision point if collision occurs; otherwise, (null, null).
    /// </returns>
    public static (IObject? Obj, Vector3f? Coordinate) GetCollisionObject(IObject subject, double nextX, double nextY, List<IUnit> ignoreList, bool ignorePassability = false)
    {
        float minDistFromWall = subject is IMovable movable ? movable.MinDistanceFromWall / 2 : 0;
        double offsetX = minDistFromWall * Math.Sign(nextX);
        double offsetY = minDistFromWall * Math.Sign(nextY);

        var collisionX = GetFirstCollision(subject, subject.X.Axis + nextX + offsetX, subject.Y.Axis, ignoreList, ignorePassability);
        var collisionY = GetFirstCollision(subject, subject.X.Axis, subject.Y.Axis + nextY + offsetY, ignoreList, ignorePassability);

        if (nextX != 0 && collisionX.Item1 == null)
            subject.X.Axis += nextX;

        if (nextY != 0 && collisionY.Item1 == null)
            subject.Y.Axis += nextY;

        return collisionX.Item1 != null ? collisionX :
               collisionY.Item1 != null ? collisionY :
               (null, null);
    }
}
