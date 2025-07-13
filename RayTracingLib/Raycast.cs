using ScreenLib;
using HitBoxLib.PositionObject;
using SFML.System;
using HitBoxLib.Segment.SignsTypeSide;
using ProtoRender.Object;
using System.Collections.Concurrent;
using ProtoRender.Map;

namespace RayTracingLib;
/// <summary>
/// Checks for ray collision with objects
/// </summary>
public static class Raycast
{
    private const float EPSILON = 0.000001f;
    private static int _scanRadius = 1;

    /// <summary>Radius of collision check of ray with object</summary>
    public static int ScanRadius
    {
        get => _scanRadius;
        set => _scanRadius = Math.Max(0, value);
    }
    /// <summary>Percentage of step for calculating coordinates</summary>
    public static float PercentTile { get; set; } = 0.01f;
    /// <summary>Normalizes the coordinates of the hit, - if further from the observer, + if closer</summary>
    public static int CoordinatesMoving { get; set; } = 10;
    /// <summary>
    /// Finds the intersection points of the ray with the object's boundaries
    /// </summary>
    private static List<double> GetIntersectionParameter(IUnit unit, float minX, float maxX, float minY, float maxY)
    {
        List<double> tValues = new();
        var (dirX, dirY) = (unit.Direction.X, unit.Direction.Y);

        if (dirX != 0)
        {
            tValues.Add((minX - unit.X.Axis) / dirX);
            tValues.Add((maxX - unit.X.Axis) / dirX);
        }
        if (dirY != 0)
        {
            tValues.Add((minY - unit.Y.Axis) / dirY);
            tValues.Add((maxY - unit.Y.Axis) / dirY);
        }

        return tValues;
    }

    /// <summary>
    /// Checks if a ray intersects an object at a specific T value
    /// </summary>
    private static CollisionResult CheckIntersection(IObject obj, IUnit unit, double x, double y, Sides sides)
    {
        var mainBox = obj.HitBox.MainHitBox;
        Vector3f center = new Vector3f((float)x, (float)y, (float)obj.Z.Axis);
        var z = HitBoxLib.Operations.Collision.IsRayTouchesObjectZ(obj.GetRenderHitBoxInfo(), unit.GetObserverInfo(), mainBox);

        bool isTouch2D = HitBoxLib.Operations.Collision.IsRayTouchesObject(mainBox, x, y);
        return new CollisionResult(HitBoxLib.Operations.Collision.IsTouches3D(isTouch2D, z.result), new Vector3f((float)x, (float)y, (float)z.coordinate), sides);
    }

    private static Sides GetSideObject(IObject obj)
    {
        float minX = (float)(obj.HitBox.MainHitBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0);
        float maxX = (float)(obj.HitBox.MainHitBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0);
        float minY = (float)(obj.HitBox.MainHitBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0);
        float maxY = (float)(obj.HitBox.MainHitBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0);

        return new Sides(minX, maxX, minY, maxY);
    }
    /// <summary>
    /// Calculates the coordinates of the beam hitting the object
    /// </summary>
    public static CollisionResult GetCoordinateTouch(IMap map, IObject checkedObject, IUnit unit)
    {
        int mapWidth = map.Setting.MapTileWidth;
        int mapHeight = map.Setting.MapTileHeight;
        int tileSize = (int)(Screen.Setting.Tile * PercentTile);

        double dx = unit.Direction.X, dy = unit.Direction.Y;
        dx = Math.Abs(dx) < EPSILON ? (dx < 0 ? -EPSILON : EPSILON) : dx;
        dy = Math.Abs(dy) < EPSILON ? (dy < 0 ? -EPSILON : EPSILON) : dy;


        double startX = unit.X.Axis, startY = unit.Y.Axis;
        double gridX = startX;
        double gridY = startY;

        int stepX = dx > 0 ? tileSize : -tileSize;
        int stepY = dy > 0 ? tileSize : -tileSize;

        double tDeltaX = Math.Abs(tileSize / dx);
        double tDeltaY = Math.Abs(tileSize / dy);

        double tMaxX = (gridX + (dx > 0 ? tileSize : 0) - startX) / dx;
        double tMaxY = (gridY + (dy > 0 ? tileSize : 0) - startY) / dy;

        Sides sides = GetSideObject(checkedObject);
        while (true)
        {
            var result = CheckIntersection(checkedObject, unit, gridX, gridY, sides);
            if (result.IsTouch == true)
            {
                result.Coordinate = new Vector3f(
                    result.Coordinate.X - stepX * CoordinatesMoving,
                    result.Coordinate.Y - stepY * CoordinatesMoving,
                    result.Coordinate.Z
                    );

                return result;
            }

            if (tMaxX < tMaxY)
            {
                gridX += stepX;
                tMaxX += tDeltaX;
            }
            else
            {
                gridY += stepY;
                tMaxY += tDeltaY;
            }

            if (gridX < 0 || gridY < 0 || gridX >= mapWidth || gridY >= mapHeight)
                return new CollisionResult(false, default, sides);
        }

    }


    private static bool RayHitsObjectBounds(IUnit unit, IObject ownerBox, HitBoxLib.HitBoxSegment.Box box)
    {
        float minX = (float)(box[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0);
        float maxX = (float)(box[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0);
        float minY = (float)(box[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0);
        float maxY = (float)(box[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0);

        List<double> tValues = GetIntersectionParameter(unit, minX, maxX, minY, maxY);

        foreach (var tValue in tValues)
        {
            if (tValue < 0) continue;

            double x = unit.X.Axis + tValue * unit.Direction.X;
            double y = unit.Y.Axis + tValue * unit.Direction.Y;

            var result = CheckIntersection(ownerBox, unit, x, y, new Sides(minX, maxX, minY, maxY));
            if (result.IsTouch)
                return true;
        }

        return false;
    }
    /// <summary>
    /// Checks for ray intersections with objects in a cell
    /// </summary>
    private static void DetailedSearchInCell(ConcurrentBag<IObject> colisionObject, List<IObject> objs, IUnit unit, bool useIgnoreList = true)
    {

        foreach (var obj in objs)
        {
            if (obj == unit || (useIgnoreList && unit.IgnoreCollisionObjects.ContainsKey(obj)))
                continue;

            Vector3f obstaclePos = new Vector3f((float)obj.X.Axis, (float)obj.Y.Axis, (float)obj.Z.Axis);
            if (RayHitsObjectBounds(unit, obj, obj.HitBox.MainHitBox))
            {
                colisionObject.Add(obj);
                break;
            }
        }
    }

    /// <summary>
    /// Returns the closest object that the ray intersects
    /// </summary>
    public static (IObject?, CollisionResult) GetFirstTouchedObject(IMap map, ConcurrentBag<IObject> colisionObject, IUnit unit)
    {
        (IObject?, CollisionResult) nearestObstacle = default;
        double nearestDistance = double.MaxValue;

        foreach (var obstacle in colisionObject)
        {
            Vector3f obstaclePos = new Vector3f((float)obstacle.X.Axis, (float)obstacle.Y.Axis, (float)obstacle.Z.Axis);
            var mainHitBox = obstacle.HitBox.MainHitBox;

            float minX = (float)(mainHitBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0);
            float maxX = (float)(mainHitBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0);
            float minY = (float)(mainHitBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0);
            float maxY = (float)(mainHitBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0);

            List<double> tValues = GetIntersectionParameter(unit, minX, maxX, minY, maxY);

            foreach (var tValue in tValues)
            {
                if (tValue < 0) continue;

                double x = unit.X.Axis + tValue * unit.Direction.X;
                double y = unit.Y.Axis + tValue * unit.Direction.Y;

                var result = CheckIntersection(obstacle, unit, x, y, new Sides(minX, maxX, minY, maxY));

                if (result.IsTouch && tValue < nearestDistance)
                {
                    nearestDistance = tValue;
                    nearestObstacle = (obstacle, GetCoordinateTouch(map, obstacle, unit));
                }
            }
        }

        return nearestObstacle;
    }

    private static List<HitBoxLib.HitBoxSegment.Box> GetAllTouchedBoxes(IUnit unit, IObject touchedObject)
    {
        List<HitBoxLib.HitBoxSegment.Box> boxes = new() { touchedObject.HitBox.MainHitBox };
        foreach(var box in touchedObject.HitBox.SegmentedHitbox)
        {
            if (RayHitsObjectBounds(unit, touchedObject, box))
                boxes.Add(box);
        }

        return boxes;
    }

    /// <summary>
    /// Basic Ray Tracing Method
    /// </summary>
    public static (IObject?, Vector3f, List<HitBoxLib.HitBoxSegment.Box>) RaycastFun(IUnit unit, bool useIgnoreList = true)
    {
        if (unit.Map is null)
            return default;

        int tileSize = Screen.Setting.Tile;
        int mapWidth = unit.Map.Setting.MapTileWidth;
        int mapHeight = unit.Map.Setting.MapTileHeight;


        double dx = unit.Direction.X, dy = unit.Direction.Y;
        dx = Math.Abs(dx) < EPSILON ? (dx < 0 ? -EPSILON : EPSILON) : dx;
        dy = Math.Abs(dy) < EPSILON ? (dy < 0 ? -EPSILON : EPSILON) : dy;

        double startX = unit.X.Axis, startY = unit.Y.Axis;

        int gridX = (int)(startX / tileSize) * tileSize;
        int gridY = (int)(startY / tileSize) * tileSize;

        int stepX = dx > 0 ? tileSize : -tileSize;
        int stepY = dy > 0 ? tileSize : -tileSize;

        double tDeltaX = Math.Abs(tileSize / dx);
        double tDeltaY = Math.Abs(tileSize / dy);

        double tMaxX = (gridX + (dx > 0 ? tileSize : 0) - startX) / dx;
        double tMaxY = (gridY + (dy > 0 ? tileSize : 0) - startY) / dy;

        while (true)
        {
            ConcurrentBag<IObject> collisions = new();
            Parallel.For(0, ScanRadius + 1, radius =>
            {
                for (int x = -radius; x <= radius; x++)
                {
                    for (int y = -radius; y <= radius; y++)
                    {
                        int scanX = gridX + x * tileSize;
                        int scanY = gridY + y * tileSize;

                        if (!unit.Map.Obstacles.TryGetValue((scanX, scanY), out var value))
                            continue;

                        DetailedSearchInCell(collisions, value.Keys.ToList(), unit, useIgnoreList);
                    }
                }
            });


            var nearestObstacle = GetFirstTouchedObject(unit.Map, collisions, unit);
            if (nearestObstacle.Item1 != null)
            {
                return (nearestObstacle.Item1, nearestObstacle.Item2.Coordinate, GetAllTouchedBoxes(unit, nearestObstacle.Item1));
            }

            if (tMaxX < tMaxY)
            {
                gridX += stepX;
                tMaxX += tDeltaX;
            }
            else
            {
                gridY += stepY;
                tMaxY += tDeltaY;
            }

            if (gridX < 0 || gridY < 0 || gridX >= mapWidth || gridY >= mapHeight)
                return default;
        }
    }
}
/// <summary>Stores main side hitbox</summary>
public record struct Sides(float minX, float maxX, float minY, float maxY);
/// <summary>Stores collision result</summary>
public record struct CollisionResult(bool IsTouch, Vector3f Coordinate, Sides Sides);
