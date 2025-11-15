using ScreenLib;
using SFML.System;
using ProtoRender.Object;
using HitBoxLib.PositionObject;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.Segment.SignsTypeSide;
using MoveLib.Move.Result;
using ProtoRender.Physics;
using DataPipes;


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


    #region Detection 
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
    public static (bool Result, CollisionAxis Axis) IsCollisionNear(IObject subject, double nextX, double nextY, List<IObject> ignoreList, bool ignorePassability = false)
    {
        if (subject.Map is null)
            return default;

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
                        var resultMain = CollisionHelper.IsCollidingAtPosition(subject, obstacle, obstacle.HitBox.MainHitBox, nextX, nextY, ignorePassability);
                        if (resultMain.Result)
                            return (true, resultMain.Axis);
                    }
                    foreach (var box in obstacle.HitBox.SegmentedHitbox)
                    {
                        var resultSegment = CollisionHelper.IsCollidingAtPosition(subject, obstacle, box, nextX, nextY, ignorePassability);
                        if (resultSegment.Result)
                            return (true, resultSegment.Axis);
                    }
                }
            }
        }

        return default;
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
    public static CollisionResult GetCollisionNear(IObject subject, double nextX, double nextY, List<IObject> ignoreList, bool ignorePassability = false)
    {
        if (subject.Map is null)
            return default;

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
                        if (point.Coordinate is not null)
                            return new(obstacle, point.Coordinate, point.Axis);
                    }
                    foreach (var box in obstacle.HitBox.SegmentedHitbox)
                    {
                        var pointSegment = CollisionHelper.GetCollisionPointAt(subject, obstacle, box, nextX, nextY, ignorePassability);
                        if (pointSegment.Coordinate is not null)
                            return new(obstacle, pointSegment.Coordinate, pointSegment.Axis);
                    }
                }
            }
        }

        return default;
    }

    /// <summary>
    /// Shortcut for <see cref="FindSurfaceHeight(SurfaceCheckMode, IObject, Box, double, List{IObject})"/> 
    /// that uses the object's current Z position.
    /// </summary>
    public static double FindSurfaceHeight(SurfaceCheckMode mode, IObject subject, Box subjectBox, List<IObject> ignoreList)
    {
        return FindSurfaceHeight(mode, subject, subjectBox, subject.Z.Axis, ignoreList);
    }

    /// <summary>
    /// Finds the nearest supporting surface (ground or ceiling) relative to the given object.
    /// The method returns the Z coordinate of the found surface.  
    /// If no suitable surface is found, it returns a default value:
    /// <list type="bullet">
    /// <item><c>double.MinValue</c> when <see cref="SurfaceCheckMode.Ground"/> is used.</item>
    /// <item><c>double.MaxValue</c> when <see cref="SurfaceCheckMode.Ceiling"/> is used.</item>
    /// </list>
    /// </summary>
    /// <param name="mode">
    /// The check mode:
    /// <list type="bullet">
    /// <item><see cref="SurfaceCheckMode.Ground"/> – returns the highest surface below the object.</item>
    /// <item><see cref="SurfaceCheckMode.Ceiling"/> – returns the lowest surface above the object.</item>
    /// </list>
    /// </param>
    /// <param name="subject">The object for which the surface check is performed.</param>
    /// <param name="subjectBox">The bounding box of the object used for collision checks.</param>
    /// <param name="newZ">
    /// Optional Z position override. If different from the object's current Z, 
    /// the bounding box is recalculated at this height.
    /// </param>
    /// <param name="ignoreList">A list of objects to exclude from the check.</param>
    /// <returns>
    /// The Z coordinate of the found surface, or the default value (<c>double.MinValue</c> or <c>double.MaxValue</c>)
    /// if no ground or ceiling was found, depending on the mode.
    /// </returns>
    public static double FindSurfaceHeight(SurfaceCheckMode mode, IObject subject, Box subjectBox, double newZ, List<IObject> ignoreList)
    {
        var defaultValue = mode == SurfaceCheckMode.Ground ? double.MinValue : double.MaxValue;
        if (subject.Map is null || subject is null)
            return defaultValue;

        ignoreList ??= new List<IObject>();


        double surfaceHeight = defaultValue;
        var (cellX, cellY) = Screen.Mapping(subject.X.Axis, subject.Y.Axis);

        int minX = cellX - RadiusCheckTouch;
        int maxX = cellX + RadiusCheckTouch;
        int minY = cellY - RadiusCheckTouch;
        int maxY = cellY + RadiusCheckTouch;

        var (subMinX, subMaxX) = CollisionHelper.GetBounds(subjectBox, CoordinatePlane.X);
        var (subMinY, subMaxY) = CollisionHelper.GetBounds(subjectBox, CoordinatePlane.Y);

        var (subMinZ, subMaxZ) = CollisionHelper.GetBounds(subjectBox, CoordinatePlane.Z);
        if (newZ != subject.Z.Axis)
        {
            subMinZ = newZ + (subjectBox[CoordinatePlane.Z, SideSize.Smaller]?.Offset ?? 0);
            subMaxZ = newZ + (subjectBox[CoordinatePlane.Z, SideSize.Larger]?.Offset ?? 0);
        }

        for (int x = minX; x <= maxX; x += Screen.Setting.Tile)
        {
            for (int y = minY; y <= maxY; y += Screen.Setting.Tile)
            {
                if (subject.Map.Obstacles is null ||
                    !subject.Map.Obstacles.TryGetValue((x, y), out var obstacles) ||
                    obstacles is null)
                {
                    continue;
                }

                foreach (var obstacle in obstacles.Keys)
                {
                    if (obstacle is null || obstacle == subject || ignoreList.Contains(obstacle))
                        continue;

                    var targetBox = obstacle.HitBox.MainHitBox;
                    if (targetBox is null)
                        continue;

                    var (tgtMinX, tgtMaxX) = CollisionHelper.GetBounds(targetBox, CoordinatePlane.X);
                    var (tgtMinY, tgtMaxY) = CollisionHelper.GetBounds(targetBox, CoordinatePlane.Y);
                    var (tgtMinZ, tgtMaxZ) = CollisionHelper.GetBounds(targetBox, CoordinatePlane.Z);

                    bool intersectsXY =
                        subMaxX >= tgtMinX &&
                        subMinX <= tgtMaxX &&
                        subMaxY >= tgtMinY &&
                        subMinY <= tgtMaxY;

                    if (!intersectsXY)
                        continue;

                    switch (mode)
                    {
                        case SurfaceCheckMode.Ground:
                            if (subMinZ >= tgtMaxZ && tgtMaxZ > surfaceHeight)
                                surfaceHeight = tgtMaxZ;
                            break;
                        case SurfaceCheckMode.Ceiling:
                            if (subMaxZ >= tgtMinZ && subMaxZ <= tgtMaxZ && surfaceHeight > tgtMinZ)
                                surfaceHeight = tgtMinZ;
                            break;
                    }
                }
            }
        }

        return surfaceHeight;
    }

           


    #endregion

    #region Handle
    /// <summary>
    /// Resolves potential collisions when moving the subject object,
    /// adjusting its position to avoid overlaps with the target or
    /// moving beyond the safe point.
    /// </summary>
    /// <remarks>
    /// Algorithm:
    /// 1. Considers the minimum distance from walls for movable objects.
    /// 2. Calculates potential new X and Y positions.
    /// 3. Checks for collisions using <c>IsCollisionNear</c>.
    /// 4. If a collision is detected or movement exceeds safe limits,
    ///    the position is reverted.
    /// </remarks>
    public static void ResolvePositionCollision(
    IObject subject,
    IObject target,
    Vector3f safePoint,
    double deltaX,
    double deltaY)
    {
        float minDistFromWall = subject is IMovable movable ? movable.MinDistanceFromWall / 2 : 0;

        double targetX = subject.X.Axis + deltaX + (minDistFromWall * Math.Sign(deltaX));
        double targetY = subject.Y.Axis + deltaY + (minDistFromWall * Math.Sign(deltaY));


        double originalX = subject.X.Axis;
        double originalY = subject.Y.Axis;
        var closest = new Vector2f(
            Math.Clamp((float)target.X.Axis,
                (float)(subject.HitBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0f),
                (float)(subject.HitBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0f)),
            Math.Clamp((float)target.Y.Axis,
                (float)(subject.HitBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0f),
                (float)(subject.HitBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0f))
        );


        subject.X.Axis += deltaX;
        if (IsCollisionNear(subject, subject.X.Axis, subject.Y.Axis, new List<IObject>()).Result)
        {
            subject.X.Axis = originalX;
        }
        else if ((deltaX > 0 && subject.X.Axis < safePoint.X - closest.X) ||
                  (deltaX < 0 && subject.X.Axis > safePoint.X + closest.X))
        {
            subject.X.Axis = originalX;
        }

        subject.Y.Axis += deltaY;
        if (IsCollisionNear(subject, subject.X.Axis, subject.Y.Axis, new List<IObject>()).Result)
        {
            subject.Y.Axis = originalY;
        }
        else if ((deltaY > 0 && subject.Y.Axis < safePoint.Y - closest.Y) ||
                 (deltaY < 0 && subject.Y.Axis > safePoint.Y + closest.Y))
        {
            subject.Y.Axis = originalY;
        }
    }
    #endregion

    #region Collision

    /// <summary>
    /// Attempts to move the unit by the specified deltas and checks if a collision occurs with obstacles.
    /// </summary>
    /// <param name="subject">The IObject to move.</param>
    /// <param name="deltaX">Delta movement in the X direction.</param>
    /// <param name="deltaY">Delta movement in the Y direction.</param>
    /// <param name="ignoreList">List of objects to ignore during collision detection.</param>
    /// <param name="ignorePassability">If true, ignores obstacle passability.</param>
    /// <returns>True if a collision occurred; otherwise, false.</returns>
    public static CollisionResult TryMoveWithCollision(IObject subject, double deltaX, double deltaY, List<IObject> ignoreList, bool ignorePassability = false)
    {
        if (deltaX == 0 && deltaY == 0)
            return default;
        if (subject is IPhysicsObject physics && physics.ControlState is not ControlState.Normal)
            return default;

        IMovable? movable = subject as IMovable;
        float minDistFromWall = movable?.MinDistanceFromWall / 2 ?? 0;

        double dx = deltaX + (minDistFromWall * Math.Sign(deltaX));
        double dy = deltaY + (minDistFromWall * Math.Sign(deltaY));
        double targetX = subject.X.Axis + dx;
        double targetY = subject.Y.Axis + dy;

        UpdateMoveDirection(movable, dx, dy);

        var collisionResultX = GetCollisionNear(subject, targetX, subject.Y.Axis, ignoreList, ignorePassability);
        var collisionResultY = GetCollisionNear(subject, subject.X.Axis, targetY, ignoreList, ignorePassability);

        if (collisionResultX.CollisionObject is null)
            subject.X.Axis += deltaX;
        if (collisionResultY.CollisionObject is null)
            subject.Y.Axis += deltaY;

        var result = collisionResultX.CollisionObject is not null ? collisionResultX : collisionResultY;
        var axis = CombineCollisionAxis(collisionResultX.Axis, collisionResultY.Axis);

        return new (result.CollisionObject, result.CollisionCoordinate, axis);
    }

    private static void UpdateMoveDirection(IMovable? movable, double dx, double dy)
    {
        if (movable is null) return;

        double length = Math.Sqrt(dx * dx + dy * dy);
        movable.MoveDirection = length > 0
            ? new Vector2f((float)(dx / length), (float)(dy / length))
            : new Vector2f();
    }
    private static CollisionAxis CombineCollisionAxis(CollisionAxis axisX, CollisionAxis axisY) =>
    new CollisionAxis(
        axisX.X || axisY.X,
        axisX.Y || axisY.Y,
        axisX.Z || axisY.Z
    );


    /// <summary>
    /// Attempts to move the unit by the specified deltas and checks if a collision occurs with obstacles.
    /// </summary>
    /// <param name="subject">The IObject to move.</param>
    /// <param name="deltaX">Delta movement in the X direction.</param>
    /// <param name="deltaY">Delta movement in the Y direction.</param>
    /// <param name="ignoreList">List of objects to ignore during collision detection.</param>
    /// <param name="ignorePassability">If true, ignores obstacle passability.</param>
    /// <returns>True if a collision occurred; otherwise, false.</returns>
    public static bool WillCollide(IObject subject, double deltaX, double deltaY, List<IObject> ignoreList, bool ignorePassability = false)
    {
        if (deltaX == 0 && deltaY == 0)
            return false;

        float minDistFromWall = subject is IMovable movable ? movable.MinDistanceFromWall / 2 : 0;

        double targetX = subject.X.Axis + deltaX + (minDistFromWall * Math.Sign(deltaX));
        double targetY = subject.Y.Axis + deltaY + (minDistFromWall * Math.Sign(deltaY));

        var collisionResult = GetCollisionNear(subject, targetX, targetY, ignoreList, ignorePassability);

        return !(collisionResult.CollisionObject is null);
    }

    /// <summary>
    /// Attempts to move the unit by the specified deltas and returns the first obstacle and collision point encountered.
    /// </summary>
    /// <param name="subject">The IObject to move.</param>
    /// <param name="deltaX">Delta movement in the X direction.</param>
    /// <param name="deltaY">Delta movement in the Y direction.</param>
    /// <param name="ignoreList">List of units to ignore during collision detection.</param>
    /// <param name="ignorePassability">If true, ignores obstacle passability.</param>
    /// <returns>
    /// Tuple containing the colliding object and collision point if collision occurs; otherwise, (null, null).
    /// </returns>
    public static CollisionResult GetCollision(IObject subject, double deltaX, double deltaY, List<IObject> ignoreList, bool ignorePassability = false)
    {
        if (deltaX == 0 && deltaY == 0)
            return default;

        float minDistFromWall = subject is IMovable movable ? movable.MinDistanceFromWall / 2 : 0;

        double targetX = subject.X.Axis + deltaX + (minDistFromWall * Math.Sign(deltaX));
        double targetY = subject.Y.Axis + deltaY + (minDistFromWall * Math.Sign(deltaY));

        var collisionResult = GetCollisionNear(subject, targetX, targetY, ignoreList, ignorePassability);
        return collisionResult;
    }
    
    #endregion
}