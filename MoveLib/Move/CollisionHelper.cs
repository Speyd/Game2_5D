using SFML.System;
using ProtoRender.Object;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.PositionObject;
using HitBoxLib.Segment.SignsTypeSide;
using TextureLib.Textures.Pair;
using MoveLib.Move.Result;


namespace MoveLib.Move;
/// <summary>
/// Provides utility methods for detecting and processing collisions between 3D hitboxes.
/// </summary>
public static class CollisionHelper
{
    /// <summary>
    /// Checks if two boxes overlap on the specified coordinate plane.
    /// </summary>
    public static bool CheckCollision(Box subjectBox, Box targetBox, CoordinatePlane plane)
    {
        var (subjectMin, subjectMax) = GetBounds(subjectBox, plane);
        var (targetMin, targetMax) = GetBounds(targetBox, plane);

        return IntervalsOverlap(subjectMin, subjectMax, targetMin, targetMax);
    }

    /// <summary>
    /// Checks if two 1D intervals overlap.
    /// </summary>
    public static bool CheckCollision(double subjectMin, double subjectMax, double targetMin, double targetMax)
    {
        return IntervalsOverlap(subjectMin, subjectMax, targetMin, targetMax);
    }

    /// <summary>
    /// Returns whether two numeric intervals overlap.
    /// </summary>
    public static bool IntervalsOverlap(double min1, double max1, double min2, double max2)
    {
        return max1 > min2 && min1 < max2;
    }

    /// <summary>
    /// Gets the min and max bounds of a box along a specified coordinate plane.
    /// </summary>
    public static (double Min, double Max) GetBounds(Box box, CoordinatePlane plane)
    {
        double min = box[plane, SideSize.Smaller]?.Side ?? 0.0;
        double max = box[plane, SideSize.Larger]?.Side ?? 0.0;
        return (min, max);
    }

    /// <summary>
    /// Calculates the average center point of two bounding boxes.
    /// </summary>
    public static Vector3f GetAverageCenter(Vector3f minA, Vector3f maxA, Vector3f minB, Vector3f maxB)
    {
        float x = (minA.X + maxA.X + minB.X + maxB.X) / 4f;
        float y = (minA.Y + maxA.Y + minB.Y + maxB.Y) / 4f;
        float z = (minA.Z + maxA.Z + minB.Z + maxB.Z) / 4f;
        return new Vector3f(x, y, z);
    }

    /// <summary>
    /// Determines if the subject's hitbox collides with the target's box.
    /// </summary>
    public static (bool Result, CollisionAxis Axis) IsCollisionHitBox(IObject subject, Box subjectBox, IObject target, Box targetBox, bool ignorePassability = false)
    {
        return TryGetCollision(subject, subjectBox, target, targetBox, ignorePassability, out _);
    }

    /// <summary>
    /// Returns the point of collision between two objects, or null if no collision occurred.
    /// </summary>
    public static (Vector3f? Coordinate, CollisionAxis Axis) GetCollisionPoint(IObject subject, Box subjectBox, IObject target, Box targetBox, bool ignorePassability = false)
    {
        var resultCollision = TryGetCollision(subject, subjectBox, target, targetBox, ignorePassability, out var data);
        return resultCollision.Result ? (data?.CollisionPoint, resultCollision.Axis) : (null, resultCollision.Axis);
    }


    /// <summary>
    /// Checks for collision and returns the side on which the collision occurred.
    /// </summary>
    public static (bool IsColliding, CollisionAxis Axis, ObjectSide CollisionSide) CheckCollisionWithSide(IObject subject, Box subjectBox, IObject target, Box targetBox, bool ignorePassability = false)
    {
        var resultCollision = TryGetCollision(subject, subjectBox, target, targetBox, ignorePassability, out var data);
        if (!resultCollision.Result)
            return (false, resultCollision.Axis, ObjectSide.Error);

        if (data is null)
            return (false, resultCollision.Axis, ObjectSide.Error);

        double deltaX = data.Value.CenterA.X - data.Value.CenterB.X;
        double deltaY = data.Value.CenterA.Y - data.Value.CenterB.Y;
        double overlapX = (data.Value.WidthSum / 2.0) - Math.Abs(deltaX);
        double overlapY = (data.Value.HeightSum / 2.0) - Math.Abs(deltaY);

        ObjectSide side = overlapX < overlapY
            ? (deltaX > 0 ? ObjectSide.Left : ObjectSide.Right)
            : (deltaY > 0 ? ObjectSide.Top : ObjectSide.Bottom);

        return (true, resultCollision.Axis, side);
    }

    /// <summary>
    /// Checks if the subject will collide with the target at a new position.
    /// </summary>
    public static (bool Result, CollisionAxis Axis) IsCollidingAtPosition(IObject subject, IObject target, Box targetBox, double nextX, double nextY, bool ignorePassability = false)
    {
        return SimulatePosition(subject, nextX, nextY, (Box subjectBox) =>
            IsCollisionHitBox(subject, subjectBox, target, targetBox, ignorePassability));
    }

    /// <summary>
    /// Gets the collision point between subject and target if subject is moved to a new position.
    /// </summary>
    public static (Vector3f? Coordinate, CollisionAxis Axis) GetCollisionPointAt(IObject subject, IObject target, Box targetBox, double nextX, double nextY, bool ignorePassability = false)
    {
        return SimulatePosition(subject, nextX, nextY, (Box subjectBox) =>
            GetCollisionPoint(subject, subjectBox, target, targetBox, ignorePassability));
    }

    // ----------------- Private Helpers -----------------

    /// <summary>
    /// Attempts to detect a collision between subject and target and provides detailed collision data if successful.
    /// </summary>
    private static (bool Result, CollisionAxis Axis) TryGetCollision(IObject subject, Box subjectBox, IObject target, Box targetBox, bool ignorePassability, out CollisionData? data)
    {
        data = null;

        if (subject?.HitBox == null || target?.HitBox == null)
            return default;


        var (subMinX, subMaxX) = GetBounds(subjectBox, CoordinatePlane.X);
        var (subMinY, subMaxY) = GetBounds(subjectBox, CoordinatePlane.Y);
        var (subMinZ, subMaxZ) = GetBounds(subjectBox, CoordinatePlane.Z);

        var (tgtMinX, tgtMaxX) = GetBounds(targetBox, CoordinatePlane.X);
        var (tgtMinY, tgtMaxY) = GetBounds(targetBox, CoordinatePlane.Y);
        var (tgtMinZ, tgtMaxZ) = GetBounds(targetBox, CoordinatePlane.Z);

        bool overlapX = IntervalsOverlap(subMinX, subMaxX, tgtMinX, tgtMaxX);
        bool overlapY = IntervalsOverlap(subMinY, subMaxY, tgtMinY, tgtMaxY);
        bool overlapZ = IntervalsOverlap(subMinZ, subMaxZ, tgtMinZ, tgtMaxZ);

        bool colliding = overlapX && overlapY && overlapZ;
        if (!colliding || (!ignorePassability && target.IsPassability))
            return (false, new (overlapX, overlapY, overlapZ));

        Vector3f minA = new((float)subMinX, (float)subMinY, (float)subMinZ);
        Vector3f maxA = new((float)subMaxX, (float)subMaxY, (float)subMaxZ);
        Vector3f minB = new((float)tgtMinX, (float)tgtMinY, (float)tgtMinZ);
        Vector3f maxB = new((float)tgtMaxX, (float)tgtMaxY, (float)tgtMaxZ);

        data = new CollisionData
        {
            CollisionPoint = GetAverageCenter(minA, maxA, minB, maxB),
            CenterA = new Vector3f((float)((subMinX + subMaxX) / 2), (float)((subMinY + subMaxY) / 2), 0),
            CenterB = new Vector3f((float)((tgtMinX + tgtMaxX) / 2), (float)((tgtMinY + tgtMaxY) / 2), 0),
            WidthSum = (subMaxX - subMinX) + (tgtMaxX - tgtMinX),
            HeightSum = (subMaxY - subMinY) + (tgtMaxY - tgtMinY)
        };

        return (true, new(overlapX, overlapY, overlapZ));
    }

    /// <summary>
    /// Simulates the subject being at a new position, performs a collision check, and restores the original position.
    /// </summary>
    private static T SimulatePosition<T>(IObject subject, double newX, double newY, Func<Box, T> collisionCheck)
    {
        Box temp = new Box(subject.HitBox.MainHitBox);
        CanMove(temp, newX, CoordinatePlane.X);
        CanMove(temp, newY, CoordinatePlane.Y);

        T result = collisionCheck(temp);

        return result;
    }

    private static void CanMove(Box box, double coord, CoordinatePlane coordinatePlane)
    {
        foreach (HitBoxSide item in box[coordinatePlane])
        {
            item.SetSide(coord);
        }
    }
}