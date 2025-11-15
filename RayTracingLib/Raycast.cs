using ScreenLib;
using HitBoxLib.PositionObject;
using SFML.System;
using HitBoxLib.Segment.SignsTypeSide;
using ProtoRender.Object;
using System.Collections.Concurrent;
using ProtoRender.Map;
using System;

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
    public static List<double> GetIntersectionParameter(IUnit unit, float minX, float maxX, float minY, float maxY,
        double? dx = null, double? dy = null)
    {
        List<double> tValues = new();
        var (dirX, dirY) = (dx ?? unit.LookDirection.X, dy ?? unit.LookDirection.Y);

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
    public static CollisionResult CheckIntersection(IObject obj, IUnit unit, double x, double y, Sides sides)
    {
        if (unit is null || obj is null)
            return default;

        var mainBox = obj.HitBox.MainHitBox;
        Vector3f center = new Vector3f((float)x, (float)y, (float)obj.Z.Axis);
        var z = HitBoxLib.Operations.Collision.IsRayTouchesObjectZ(obj.GetRenderHitBoxInfo(), unit.GetObserverInfo(), mainBox);

        bool isTouch2D = HitBoxLib.Operations.Collision.IsRayTouchesObject(mainBox, x, y);
        return new CollisionResult(HitBoxLib.Operations.Collision.IsTouches3D(isTouch2D, z.result), new Vector3f((float)x, (float)y, (float)z.coordinate), sides);
    }

    /// <summary>
    /// Retrieves the minimum and maximum boundary coordinates of the object's hitbox along the X and Y axes.
    /// </summary>
    public static Sides GetSideObject(IObject obj)
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
    public static CollisionResult GetCoordinateTouch(IMap? map, IObject checkedObject, IUnit unit, RayLimitType limitType = RayLimitType.MapBounds)
    {
        if (checkedObject == null || unit == null || map is null)
            return default;

        int mapWidth = map.Setting.MapTileWidth;
        int mapHeight = map.Setting.MapTileHeight;
        int tileSize = (int)(Screen.Setting.Tile * PercentTile);

        double dx = unit.LookDirection.X, dy = unit.LookDirection.Y;
        dx = Math.Abs(dx) < EPSILON ? (dx < 0 ? -EPSILON : EPSILON) : dx;
        dy = Math.Abs(dy) < EPSILON ? (dy < 0 ? -EPSILON : EPSILON) : dy;


        double startX = unit.X.Axis, startY = unit.Y.Axis;

        double startGridX = startX;
        double startGridY = startY;

        double gridX = startGridX;
        double gridY = startGridY;

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

            if (IsOutOfLimit(limitType, (int)gridX, (int)gridY, (int)startGridX, (int)startGridY, mapWidth, mapHeight, unit.MaxRenderTile))
                return default;
        }

    }


    /// <summary>
    /// Determines whether the ray intersects the specified object's hitbox boundaries.
    /// </summary>
    public static bool RayHitsObjectBounds(IUnit unit, IObject ownerBox, HitBoxLib.HitBoxSegment.Box box, 
        double? dx = null, double? dy = null)
    {
        if (ownerBox == null || box == null)
            return false;

        float minX = (float)(box[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0);
        float maxX = (float)(box[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0);
        float minY = (float)(box[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0);
        float maxY = (float)(box[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0);

        var dirX = (dx ?? unit.LookDirection.X);
        var dirY = (dy ?? unit.LookDirection.Y);


        List<double> tValues = GetIntersectionParameter(unit, minX, maxX, minY, maxY, dirX, dirY);

        foreach (var tValue in tValues)
        {
            if (tValue < 0) continue;

            double x = unit.X.Axis + tValue * dirX;
            double y = unit.Y.Axis + tValue * dirY;

            var result = CheckIntersection(ownerBox, unit, x, y, new Sides(minX, maxX, minY, maxY));
            if (result.IsTouch)
                return true;
        }

        return false;
    }
    /// <summary>
    /// Checks for ray intersections with objects in a cell
    /// </summary>
    public static void DetailedSearchInCell(
        ConcurrentBag<IObject> colisionObject, List<IObject> objs,
        IUnit unit, bool useIgnoreList = true,
        double? dx = null, double? dy = null)
    {

        foreach (var obj in objs)
        {
            if (obj == null || obj.HitBox == null || obj.HitBox.MainHitBox == null)
                continue;
            else if (obj == unit || (useIgnoreList && unit.IgnoreCollisionObjects.ContainsKey(obj)))
                continue;

            Vector3f obstaclePos = new Vector3f((float)obj.X.Axis, (float)obj.Y.Axis, (float)obj.Z.Axis);
            if (RayHitsObjectBounds(unit, obj, obj.HitBox.MainHitBox, dx, dy))
            {
                colisionObject.Add(obj);
                break;
            }
        }
    }

    /// <summary>
    /// Returns the closest object that the ray intersects
    /// </summary>
    public static (IObject?, CollisionResult) GetFirstTouchedInfoObject(
        IMap? map, 
        ConcurrentBag<IObject> colisionObject, IUnit unit,
        double? dx = null, double? dy = null, 
        RayLimitType limitType = RayLimitType.MapBounds)
    {
        var obstacle = GetFirstTouchedObject(map, colisionObject, unit, dx, dy, limitType);
        return obstacle is not null? (obstacle, GetCoordinateTouch(map, obstacle, unit, limitType)) : default;      
    }

    /// <summary>
    /// Finds the first object hit by a ray from the unit without calculating hit coordinates.
    /// Ignores null obstacles and respects the optional ray direction.
    /// </summary>
    public static IObject? GetFirstTouchedObject(
        IMap? map,
        ConcurrentBag<IObject> colisionObject, IUnit unit,
        double? dx = null, double? dy = null,
        RayLimitType limitType = RayLimitType.MapBounds)
    {
        if (unit is null || map is null)
            return default;

        IObject? nearestObstacle = default;
        double nearestDistance = double.MaxValue;

        foreach (var obstacle in colisionObject)
        {
            if(obstacle is null) 
                continue;

            Vector3f obstaclePos = new Vector3f((float)obstacle.X.Axis, (float)obstacle.Y.Axis, (float)obstacle.Z.Axis);
            var mainHitBox = obstacle.HitBox.MainHitBox;

            float minX = (float)(mainHitBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0);
            float maxX = (float)(mainHitBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0);
            float minY = (float)(mainHitBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0);
            float maxY = (float)(mainHitBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0);

            double dirX = dx ?? unit.LookDirection.X;
            double dirY = dy ?? unit.LookDirection.Y;


            List<double> tValues = GetIntersectionParameter(unit, minX, maxX, minY, maxY, dirX, dirY);

            foreach (var tValue in tValues)
            {
                if (tValue < 0) continue;

                double x = unit.X.Axis + tValue * dirX;
                double y = unit.Y.Axis + tValue * dirY;

                var result = CheckIntersection(obstacle, unit, x, y, new Sides(minX, maxX, minY, maxY));

                if (result.IsTouch && tValue < nearestDistance)
                {
                    nearestDistance = tValue;
                    nearestObstacle = obstacle;
                }
            }
        }

        return nearestObstacle;
    }

    /// <summary>
    /// Returns all hitbox segments of the specified object that are intersected by the ray.
    /// </summary>
    public static List<HitBoxLib.HitBoxSegment.Box> GetAllTouchedBoxes(IUnit unit, IObject touchedObject)
    {
        if (touchedObject?.HitBox?.MainHitBox == null)
            return new List<HitBoxLib.HitBoxSegment.Box>();

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
    public static (IObject?, Vector3f, List<HitBoxLib.HitBoxSegment.Box>) RaycastFun(IUnit unit, bool useIgnoreList = true, RayLimitType limitType = RayLimitType.MapBounds)
    {
        if (unit.Map is null)
            return default;

        var map = unit.Map;
        int tileSize = Screen.Setting.Tile;
        int mapWidth = map.Setting.MapTileWidth;
        int mapHeight = map.Setting.MapTileHeight;


        double dx = unit.LookDirection.X, dy = unit.LookDirection.Y;
        dx = Math.Abs(dx) < EPSILON ? (dx < 0 ? -EPSILON : EPSILON) : dx;
        dy = Math.Abs(dy) < EPSILON ? (dy < 0 ? -EPSILON : EPSILON) : dy;

        double startX = unit.X.Axis, startY = unit.Y.Axis;

        int startGridX = (int)(startX / tileSize) * tileSize;
        int startGridY = (int)(startY / tileSize) * tileSize;

        int gridX = startGridX;
        int gridY = startGridY;

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

                        if (!map.Obstacles.TryGetValue((scanX, scanY), out var value) || value?.Keys == null)
                            continue;

                        var keys = value.Keys.ToList();
                        if (keys == null || keys.Count == 0)
                            continue;

                        DetailedSearchInCell(collisions, value.Keys.ToList(), unit, useIgnoreList);
                    }
                }
            });


            var nearestObstacle = GetFirstTouchedInfoObject(map, collisions, unit, null, null, limitType);
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

            if (IsOutOfLimit(limitType, gridX, gridY, startGridX, startGridY, mapWidth, mapHeight, unit.MaxRenderTile))
                return default;
        }
    }

    /// <summary>
    /// Performs a raycast from the unit's position along a specified angle and returns the first object hit along with the intersection point.
    /// </summary>
    /// <param name="unit">The unit from whose position the ray is cast.</param>
    /// <param name="angle">The angle of the ray in radians relative to the map's coordinate system.</param>
    /// <param name="useIgnoreList">If true, objects in the unit's IgnoreCollisionObjects list are ignored during the search.</param>
    /// <param name="limitType">The limit for the raycast (e.g., map bounds or maximum number of tiles).</param>
    /// <returns>
    /// A tuple containing:
    /// - <see cref="IObject"/>? — the first object hit by the ray (or null if none found);
    /// - <see cref="Vector3f"/> — the coordinates of the intersection point with the object.
    /// </returns>
    /// <remarks>
    /// The method uses a DDA algorithm to step through map tiles and performs a parallel scan of nearby cells.
    /// When collisions are detected, <see cref="GetFirstTouchedObject"/> is called to precisely determine the intersection with object hitboxes.
    /// </remarks>

    public static IObject? RaycastAtAngle(IUnit unit, double angle, bool useIgnoreList = true, RayLimitType limitType = RayLimitType.MapBounds)
    {
        if (unit.Map is null)
            return default;

        var map = unit.Map;
        int tileSize = Screen.Setting.Tile;
        int mapWidth = map.Setting.MapTileWidth;
        int mapHeight = map.Setting.MapTileHeight;


        double dx = Math.Cos(angle), dy = Math.Sin(angle);
        dx = Math.Abs(dx) < EPSILON ? (dx < 0 ? -EPSILON : EPSILON) : dx;
        dy = Math.Abs(dy) < EPSILON ? (dy < 0 ? -EPSILON : EPSILON) : dy;

        double startX = unit.X.Axis, startY = unit.Y.Axis;

        int startGridX = (int)(startX / tileSize) * tileSize;
        int startGridY = (int)(startY / tileSize) * tileSize;

        int gridX = startGridX;
        int gridY = startGridY;

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

                        if (!map.Obstacles.TryGetValue((scanX, scanY), out var value) || value?.Keys == null)
                            continue;

                        var keys = value.Keys.ToList();
                        if (keys == null || keys.Count == 0)
                            continue;

                        DetailedSearchInCell(collisions, keys, unit, useIgnoreList, dx, dy);
                    }
                }
            });


            var nearestObstacle = GetFirstTouchedObject(map, collisions, unit, dx, dy, limitType);
            if (nearestObstacle != null)
            {
                return nearestObstacle;
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

            if (IsOutOfLimit(limitType, gridX, gridY, startGridX, startGridY, mapWidth, mapHeight, unit.MaxRenderTile))
                return default;
        }
    }


    private static bool IsOutOfLimit(
       RayLimitType limitType, int gridX, int gridY, int startGridX, int startGridY,
       int mapWidth, int mapHeight, int maxRenderTiles)
    {
        return limitType switch
        {
            RayLimitType.MapBounds => gridX < 0 || gridY < 0 || gridX >= mapWidth || gridY >= mapHeight,
            RayLimitType.MaxRenderTiles =>
                Math.Abs(gridX - startGridX) > maxRenderTiles ||
                Math.Abs(gridY - startGridY) > maxRenderTiles,
            _ => false
        };
    }
}


/// <summary>Stores main side hitbox</summary>
public record struct Sides(float minX, float maxX, float minY, float maxY);
/// <summary>Stores collision result</summary>
public record struct CollisionResult(bool IsTouch, Vector3f Coordinate, Sides Sides);
