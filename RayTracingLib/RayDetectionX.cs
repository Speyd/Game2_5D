using ScreenLib;
using SFML.System;
using TextureLib.Textures.Pair;
using HitBoxLib.PositionObject;
using HitBoxLib.Segment.SignsTypeSide;
using ProtoRender.RenderAlgorithm;
using ProtoRender.Object;


namespace RayTracingLib.Detection;
/// <summary>
/// Static class responsible for detecting the sides of objects hit by rays. It calculates 
/// the intersection points and determines the side of the object hit by the ray. This 
/// includes the corner and main sides of the object, and calculates the distance from 
/// the ray's origin to the hit point.
/// </summary>
static public class RayDetectionX
{
    /// <summary>
    /// Determines the corner side of an object based on the ray's direction and position.
    /// It calculates the intersection with the object's corners and returns the coordinates
    /// of the hit point and the distance from the ray's origin.
    /// </summary>
    /// <param name="obj">The object being hit by the ray.</param>
    /// <param name="unit">The unit from which the ray is originating.</param>
    /// <param name="cosAngle">The cosine of the ray's angle.</param>
    /// <param name="sinAngle">The sine of the ray's angle.</param>
    /// <param name="minX">The minimum X coordinate of the object.</param>
    /// <param name="maxX">The maximum X coordinate of the object.</param>
    /// <param name="maxY">The maximum Y coordinate of the object.</param>
    /// <param name="minY">The minimum Y coordinate of the object.</param>
    /// <returns>A tuple containing the hit point coordinates and the distance to the hit point.</returns>
    static private (Vector2f, float) DetermineCornerWallSide(IObject obj,
        IUnit unit, float cosAngle, float sinAngle,
        float minX, float maxX, float maxY, float minY)
    {
        float tempValue = float.MaxValue;

        if (cosAngle != 0)
        {
            float tVerticalLeft = (float)(minX - unit.X.Axis) / cosAngle;
            if (tVerticalLeft >= 0)
            {
                float hitYLeft = (float)unit.Y.Axis + tVerticalLeft * sinAngle;
                if (hitYLeft >= minY && hitYLeft <= maxY)
                    tempValue = tVerticalLeft;
            }

            float tVerticalRight = (float)(maxX - unit.X.Axis) / cosAngle;
            if (tVerticalRight >= 0)
            {
                float hitYRight = (float)unit.Y.Axis + tVerticalRight * sinAngle;
                if (hitYRight >= minY && hitYRight <= maxY)
                    tempValue = Math.Min(tempValue, tVerticalRight);
            }
        }
        if (sinAngle != 0)
        {
            float tHorizontalTop = (float)(minY - unit.Y.Axis) / sinAngle;
            if (tHorizontalTop >= 0)
            {
                float hitXTop = (float)unit.X.Axis + tHorizontalTop * cosAngle;
                if (hitXTop >= minX && hitXTop <= maxX)
                    tempValue = Math.Min(tempValue, tHorizontalTop);
            }

            float tHorizontalBottom = (float)(maxY - unit.Y.Axis) / sinAngle;
            if (tHorizontalBottom >= 0)
            {
                float hitXBottom = (float)unit.X.Axis + tHorizontalBottom * cosAngle;
                if (hitXBottom >= minX && hitXBottom <= maxX)
                    tempValue = Math.Min(tempValue, tHorizontalBottom);
            }
        }

        if (tempValue == float.MaxValue)
            return (new Vector2f(-1, -1), 0);

        float hitX = (float)unit.X.Axis + tempValue * cosAngle - (float)obj.X.Axis;
        float hitY = (float)unit.Y.Axis + tempValue * sinAngle - (float)obj.Y.Axis;

        return (new Vector2f(hitX, hitY), tempValue);
    }

    /// <summary>
    /// Determines the main side (top, bottom, left, right) of the object hit by the ray.
    /// </summary>
    /// <param name="unit">The unit from which the ray is originating.</param>
    /// <param name="cosAngle">The cosine of the ray's angle.</param>
    /// <param name="sinAngle">The sine of the ray's angle.</param>
    /// <param name="minX">The minimum X coordinate of the object.</param>
    /// <param name="maxX">The maximum X coordinate of the object.</param>
    /// <param name="maxY">The maximum Y coordinate of the object.</param>
    /// <param name="minY">The minimum Y coordinate of the object.</param>
    /// <returns>The side of the object that the ray hit.</returns>
    static private ObjectSide DetermineMainSide(IUnit unit, float cosAngle, float sinAngle,
        float minX, float maxX, float maxY, float minY)
    {
        if (unit.Y.Axis >= minY && unit.Y.Axis <= maxY)
        {
            if (cosAngle > 0 && unit.X.Axis <= maxX)
                return ObjectSide.Right;
            else if (cosAngle < 0 && unit.X.Axis >= minX)
                return ObjectSide.Left;
        }

        if (unit.X.Axis >= minX && unit.X.Axis <= maxX)
        {
            if (sinAngle > 0 && unit.Y.Axis <= maxY)
                return ObjectSide.Bottom;
            else if (sinAngle < 0 && unit.Y.Axis >= minY)
                return ObjectSide.Top;
        }

        return ObjectSide.Error;
    }

    /// <summary>
    /// Sets the wall texture side based on the wall texture status.
    /// </summary>
    /// <param name="oldWallTexture">The current wall texture.</param>
    /// <param name="newWallTexture">The new wall texture to set.</param>
    static private void SetTextureWall(ref ObjectSide oldWallTexture, ObjectSide newWallTexture)
    {
        if (oldWallTexture == ObjectSide.Error)
            oldWallTexture = newWallTexture;
    }

    /// <summary>
    /// Redefines the wall side in case it is a corner.
    /// </summary>
    /// <param name="wallDetermine">The current wall side.</param>
    /// <returns>The redefined wall side.</returns>
    static private ObjectSide RedefiningWallSides(ObjectSide wallDetermine)
    {
        switch (wallDetermine)
        {
            case ObjectSide.LeftCorner:
                return ObjectSide.Left;
            case ObjectSide.RightCorner:
                return ObjectSide.Right;
            case ObjectSide.TopCorner:
                return ObjectSide.Top;
            case ObjectSide.BottomCorner:
                return ObjectSide.Bottom;
            default:
                return wallDetermine;
        }
    }

    /// <summary>
    /// Calculates the distance from the unit to the wall based on the object's position and the hit side.
    /// </summary>
    /// <param name="obstacle">The object being hit.</param>
    /// <param name="unit">The unit from which the ray is originating.</param>
    /// <param name="distanceToWall">The calculated distance to the wall.</param>
    /// <param name="wallDetermine">The side of the wall being hit.</param>
    static private void CalculateDistanceToWall(IObject obstacle, IUnit unit, ref float distanceToWall, ObjectSide wallDetermine)
    {
        distanceToWall = (float)DataPipes.MathUtils.CalculateDistance(obstacle.X.Axis, obstacle.Y.Axis, unit.X.Axis, unit.Y.Axis);

        if (wallDetermine == ObjectSide.Top || wallDetermine == ObjectSide.Left)
            distanceToWall -= -(Screen.Setting.Tile * 3);
    }

    /// <summary>
    /// Calculates the hit point of the ray and sets the corresponding texture wall side.
    /// </summary>
    /// <param name="cornerHit">The coordinates of the corner hit by the ray.</param>
    /// <param name="distanceToPoint">The distance from the ray's origin to the hit point.</param>
    /// <param name="distanceToWall">The distance from the ray's origin to the wall.</param>
    /// <param name="wallDetermine">The side of the wall being hit.</param>
    /// <returns>A HitPoint object representing the hit point with additional information.</returns>
    static private HitPoint CalculateHitPoint(Vector2f cornerHit, float distanceToPoint, float distanceToWall, ObjectSide wallDetermine)
    {
        if (cornerHit.X > cornerHit.Y)
        {
            cornerHit.X /= Screen.Setting.Tile;
            if (cornerHit.X < cornerHit.Y)
            {
                cornerHit.Y /= Screen.Setting.Tile;
                cornerHit.X = 0;
                SetTextureWall(ref wallDetermine, ObjectSide.LeftCorner);
            }
            else
                SetTextureWall(ref wallDetermine, ObjectSide.BottomCorner);
        }
        else
        {
            cornerHit.Y /= Screen.Setting.Tile;
            if (cornerHit.X > cornerHit.Y)
            {
                cornerHit.X /= Screen.Setting.Tile;
                cornerHit.Y = 0;
                SetTextureWall(ref wallDetermine, ObjectSide.TopCorner);
            }
            else
                SetTextureWall(ref wallDetermine, ObjectSide.RightCorner);
        }

        ObjectSide textureWallDetermine = RedefiningWallSides(wallDetermine);
        return new HitPoint(cornerHit, distanceToPoint, distanceToWall, wallDetermine, textureWallDetermine);
    }

    /// <summary>
    /// Determines the hit side of the object based on the ray's direction and the object's position.
    /// </summary>
    /// <param name="obj">The object being hit by the ray.</param>
    /// <param name="unit">The unit from which the ray is originating.</param>
    /// <returns>A HitPoint object representing the hit point and the corresponding wall sides.</returns>
    static public HitPoint DetermineHitObjectSides(IObject obj, IUnit unit)
    {
        float distanceToPoint = 0;
        float distanceToWall = 0;

        float cosAngle = unit.Direction.X;
        float sinAngle = unit.Direction.Y;

        var mainHitBox = obj.HitBox.MainHitBox;
        float minX = (float)(mainHitBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0f);
        float maxX = (float)(mainHitBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0f);
        float minY = (float)(mainHitBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0f);
        float maxY = (float)(mainHitBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0f);

        ObjectSide wallDetermine = ObjectSide.Error;
        wallDetermine = DetermineMainSide(unit, cosAngle, sinAngle, minX, maxX, maxY, minY);

        CalculateDistanceToWall(obj, unit, ref distanceToWall, wallDetermine);

        (Vector2f cornerHit, distanceToPoint) = DetermineCornerWallSide(obj, unit, cosAngle, sinAngle, minX, maxX, maxY, minY);
        return CalculateHitPoint(cornerHit, distanceToPoint, distanceToWall, wallDetermine);
    }

    /// <summary>
    /// Determines the wall side of the object hit by the ray.
    /// </summary>
    /// <param name="obj">The object being hit by the ray.</param>
    /// <param name="unit">The unit from which the ray is originating.</param>
    /// <param name="cosAngle">The cosine of the ray's angle.</param>
    /// <param name="sinAngle">The sine of the ray's angle.</param>
    /// <returns>The side of the object hit by the ray.</returns>
    static private ObjectSide Determine(IObject obj, IUnit unit, float cosAngle, float sinAngle)
    {
        var mainHitBox = obj.HitBox.MainHitBox;

        float minX = (float)(mainHitBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0f);
        float maxX = (float)(mainHitBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0f);
        float maxY = (float)(mainHitBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0f);
        float minY = (float)(mainHitBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0f);

        ObjectSide wallDetermine = ObjectSide.Error;
        wallDetermine = DetermineMainSide(unit, cosAngle, sinAngle, minX, maxX, maxY, minY);
        if (wallDetermine != ObjectSide.Error)
            return wallDetermine;

        Vector2f cornerHit = DetermineCornerWallSide(obj, unit, cosAngle, sinAngle, minX, maxX, maxY, minY).Item1;
        if (cornerHit.X > cornerHit.Y)
        {
            cornerHit.X /= Screen.Setting.Tile;
            if (cornerHit.X < cornerHit.Y)
                return ObjectSide.Left;
            else
                return ObjectSide.Bottom;
        }
        else
        {
            cornerHit.Y /= Screen.Setting.Tile;
            if (cornerHit.X > cornerHit.Y)
                return ObjectSide.Top;
            else
                return ObjectSide.Right;
        }
    }

    /// <summary>
    /// Determines the side of the object hit by the ray, with optional result calculations.
    /// </summary>
    /// <param name="obj">The object being hit by the ray.</param>
    /// <param name="unit">The unit from which the ray is originating.</param>
    /// <param name="result">An optional result to override the unit's direction.</param>
    /// <returns>The side of the object hit by the ray.</returns>
    static public ObjectSide DetermineObjectSides(IObject obj, IUnit unit, Result? result = null)
    {
        float cosAngle = (float)(result?.CosCarAngle ?? unit.Direction.X);
        float sinAngle = (float)(result?.SinCarAngle ?? unit.Direction.Y);

        return Determine(obj, unit, cosAngle, sinAngle);
    }

    /// <summary>
    /// Determines the side of the object hit by the ray, based on the unit's current direction.
    /// </summary>
    /// <param name="obj">The object being hit by the ray.</param>
    /// <param name="unit">The unit from which the ray is originating.</param>
    /// <returns>The side of the object hit by the ray.</returns>
    static public ObjectSide DetermineObjectSides(IObject obj, IUnit unit)
    {
        return Determine(obj, unit, unit.Direction.X, unit.Direction.Y);
    }
}

