using ScreenLib;
using SFML.System;
using HitBoxLib.Segment.SignsTypeSide;
using HitBoxLib.PositionObject;
using ProtoRender.Object;
using ProtoRender.Map;
using HitBoxLib.HitBoxSegment;
using System;



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
    /// Checks for collision along the Z-axis between a unit and an obstacle.
    /// </summary>
    /// <param name="obstacleBox">HitBox of the obstacle.</param>
    /// <param name="unitBox">HitBox of the unit.</param>
    /// <returns>True if the Z ranges overlap, otherwise false.</returns>
    public static bool CheckZCollision(HitBox obstacleBox, HitBox unitBox)
    {
        double unitMinZ = unitBox[CoordinatePlane.Z, SideSize.Smaller]?.Side ?? 0;
        double unitMaxZ = unitBox[CoordinatePlane.Z, SideSize.Larger]?.Side ?? 0;
        double obsMinZ = obstacleBox[CoordinatePlane.Z, SideSize.Smaller]?.Side ?? 0;
        double obsMaxZ = obstacleBox[CoordinatePlane.Z, SideSize.Larger]?.Side ?? 0;
        return (unitMinZ <= obsMinZ && (unitMaxZ >= obsMaxZ || unitMaxZ <= obsMaxZ && unitMaxZ >= obsMinZ)) ||
               (unitMinZ >= obsMinZ && (unitMinZ <= obsMaxZ && (unitMaxZ >= obsMaxZ || unitMaxZ <= obsMaxZ && unitMaxZ >= obsMinZ)));
    }

    /// <summary>
    /// Determines whether the unit's hitbox collides with an obstacle's hitbox at the given next position.
    /// </summary>
    /// <returns>True if a collision occurs and the obstacle is impassable.</returns>
    public static bool IsCollidingWithObstacle(IObject obstacle, IUnit unit, double nextX, double nextY)
    {
        double originalX = unit.X.Axis;
        double originalY = unit.Y.Axis;

        unit.X.Axis = nextX;
        unit.Y.Axis = nextY;

        var obsBox = obstacle.HitBox;
        var unitBox = unit.HitBox;

        double unitMinX = unitBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0;
        double unitMaxX = unitBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0;
        double obsMinX = obsBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0;
        double obsMaxX = obsBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0;

        double unitMinY = unitBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0;
        double unitMaxY = unitBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0;
        double obsMinY = obsBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0;
        double obsMaxY = obsBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0;

        bool overlapX = unitMaxX >= obsMinX && unitMinX <= obsMaxX;
        bool overlapY = unitMaxY >= obsMinY && unitMinY <= obsMaxY;
        bool overlapZ = CheckZCollision(obsBox, unitBox);

        unit.X.Axis = originalX;
        unit.Y.Axis = originalY;

        return (overlapX && overlapY && overlapZ) && !obstacle.IsPassability;
    }

    /// <summary>
    /// Returns the 3D collision point if a collision occurs.
    /// </summary>
    /// <returns>The collision point or null if no collision.</returns>
    public static Vector3f? GetCollisionPoint(IObject obstacle, IUnit unit, double nextX, double nextY)
    {
        double originalX = unit.X.Axis;
        double originalY = unit.Y.Axis;

        unit.X.Axis = nextX;
        unit.Y.Axis = nextY;

        var unitBox = unit.HitBox;
        var obsBox = obstacle.HitBox;

        double unitMinX = unitBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0.0;
        double unitMaxX = unitBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0.0;
        double unitMinY = unitBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0.0;
        double unitMaxY = unitBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0.0;
        double unitMinZ = unitBox[CoordinatePlane.Z, SideSize.Smaller]?.Side ?? 0.0;
        double unitMaxZ = unitBox[CoordinatePlane.Z, SideSize.Larger]?.Side ?? 0.0;

        double obsMinX = obsBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0.0;
        double obsMaxX = obsBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0.0;
        double obsMinY = obsBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0.0;
        double obsMaxY = obsBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0.0;
        double obsMinZ = obsBox[CoordinatePlane.Z, SideSize.Smaller]?.Side ?? 0.0;
        double obsMaxZ = obsBox[CoordinatePlane.Z, SideSize.Larger]?.Side ?? 0.0;

        unit.X.Axis = originalX;
        unit.Y.Axis = originalY;

        bool overlapX = unitMaxX >= obsMinX && unitMinX <= obsMaxX;
        bool overlapY = unitMaxY >= obsMinY && unitMinY <= obsMaxY;
        bool overlapZ = CheckZCollision(obsBox, unitBox);

        if (overlapX && overlapY && overlapZ)
        {
            double intersectMinX = (unitMinX + obsMinX) / 2;
            double intersectMaxX = (unitMaxX + obsMaxX) / 2;

            double intersectMinY = (unitMinY + obsMinY) / 2;
            double intersectMaxY = (unitMaxY + obsMaxY) / 2;

            double intersectMinZ = (unitMinZ + obsMinZ) / 2;
            double intersectMaxZ = (unitMaxZ + obsMaxZ) / 2;

            double collisionX = (intersectMinX + intersectMaxX) / 2.0;
            double collisionY = (intersectMinY + intersectMaxY) / 2.0;
            double collisionZ = (intersectMinZ + intersectMaxZ) / 2.0;

            return new Vector3f((float)collisionX, (float)collisionY, (float)collisionZ);
        }

        return null;
    }

    /// <summary>
    /// Checks if there are any collisions around the given unit in the target direction.
    /// </summary>
    private static bool HasNearbyCollision(IMap map, IUnit unit, double nextX, double nextY, List<IObject> ignoreList)
    {
        var (cellX, cellY) = Screen.Mapping(unit.X.Axis, unit.Y.Axis);

        int minX = cellX - RadiusCheckTouch;
        int maxX = cellX + RadiusCheckTouch;
        int minY = cellY - RadiusCheckTouch;
        int maxY = cellY + RadiusCheckTouch;

        for (int x = minX; x <= maxX; x += Screen.Setting.Tile)
        {
            for (int y = minY; y <= maxY; y += Screen.Setting.Tile)
            {
                if (!map.Obstacles.ContainsKey((x, y)))
                    continue;

                foreach (var obstacle in map.Obstacles[(x, y)])
                {
                    if (obstacle == unit || ignoreList.Contains(obstacle))
                        continue;

                    if (IsCollidingWithObstacle(obstacle, unit, nextX, nextY))
                        return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Returns the first object the unit will collide with, and the point of collision.
    /// </summary>
    private static (IObject?, Vector3f?) GetFirstCollision(IMap map, IUnit unit, double nextX, double nextY, List<IUnit> ignoreList)
    {
        var (cellX, cellY) = Screen.Mapping(unit.X.Axis, unit.Y.Axis);

        int minX = cellX - RadiusCheckTouch;
        int maxX = cellX + RadiusCheckTouch;
        int minY = cellY - RadiusCheckTouch;
        int maxY = cellY + RadiusCheckTouch;

        for (int x = minX; x <= maxX; x += Screen.Setting.Tile)
        {
            for (int y = minY; y <= maxY; y += Screen.Setting.Tile)
            {
                if (!map.Obstacles.ContainsKey((x, y)))
                    continue;

                foreach (var obstacle in map.Obstacles[(x, y)])
                {
                    if (obstacle == unit || ignoreList.Contains(obstacle))
                        continue;

                    var point = GetCollisionPoint(obstacle, unit, nextX, nextY);
                    if (point != null)
                        return (obstacle, point);
                }
            }
        }

        return (null, null);
    }

    /// <summary>
    /// Attempts to move the unit and detects if a collision occurs.
    /// </summary>
    /// <returns>True if there was a collision; otherwise, false.</returns>
    public static bool IsCollision(IMap map, IUnit unit, double nextX, double nextY, List<IObject> ignoreList)
    {
        double offsetX = unit.MinDistanceFromWall / 2 * Math.Sign(nextX);
        double offsetY = unit.MinDistanceFromWall / 2 * Math.Sign(nextY);

        bool collisionX = nextX != 0 && HasNearbyCollision(map, unit, unit.X.Axis + nextX + offsetX, unit.Y.Axis, ignoreList);
        bool collisionY = nextY != 0 && HasNearbyCollision(map, unit, unit.X.Axis, unit.Y.Axis + nextY + offsetY, ignoreList);

        if (!collisionX)
            unit.X.Axis += nextX;

        if (!collisionY)
            unit.Y.Axis += nextY;

        return collisionX || collisionY;
    }

    /// <summary>
    /// Attempts to move the unit and returns the first object it collides with and the collision point.
    /// </summary>
    /// <returns>Tuple containing the colliding object and collision point if any.</returns>
    public static (IObject? Obj, Vector3f? Coordinate) GetCollisionDetails(IMap map, IUnit unit, double nextX, double nextY, List<IUnit> ignoreList)
    {
        double offsetX = unit.MinDistanceFromWall / 2 * Math.Sign(nextX);
        double offsetY = unit.MinDistanceFromWall / 2 * Math.Sign(nextY);

        var collisionX = GetFirstCollision(map, unit, unit.X.Axis + nextX + offsetX, unit.Y.Axis, ignoreList);
        var collisionY = GetFirstCollision(map, unit, unit.X.Axis, unit.Y.Axis + nextY + offsetY, ignoreList);

        if (nextX != 0 && collisionX.Item1 == null)
            unit.X.Axis += nextX;

        if (nextY != 0 && collisionY.Item1 == null)
            unit.Y.Axis += nextY;

        return collisionX.Item1 != null ? collisionX :
               collisionY.Item1 != null ? collisionY :
               (null, null);
    }
}
