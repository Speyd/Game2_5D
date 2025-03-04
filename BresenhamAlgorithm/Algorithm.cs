using EntityLib;
using MapLib;
using ScreenLib;
using Render;
using NGenerics.Extensions;
using System.Reflection;
using System.Collections.ObjectModel;
using SFML.System;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using DataPipes.Pool;

using static SFML.Graphics.Font;
using static SFML.Window.Joystick;
using System.Buffers;
using EntityLib.Player;
using Render.RenderAlgorithm;
using Render.RenderInterface;
using System.Runtime.InteropServices;

namespace BresenhamAlgorithm;
public class Algorithm(Map map, Entity entity)
{
    //------------------------------Pool-------------------------------
    static ObjectPool<Result> resultobjectPool = new ObjectPool<Result>();
    static ListPool<InfoObject> infoObjectPool = new ListPool<InfoObject>();


    //------------------------------Setting Render-------------------------------
    /// <summary> List of unique types of objects that render themselves (not with rays) </summary>
    private ConcurrentDictionary<Type, bool> UniqueSelfDrawableTypes { get; init; } = new();
    /// <summary> List with rendering methods of objects that render themselves (not rays) </summary>
    private Dictionary<Type, Action<Result, Entity>> CachedDelegates { get; init; } = new();
    /// <summary> Check if a new object type has been added that renders itself </summary>
    private bool HasNewTypes = false;


    /// <summary> Writes a new type of object that renders itself </summary>
    public void PrepareRenderObjects()
    {
        foreach (var type in UniqueSelfDrawableTypes.Keys)
        {
            if (!CachedDelegates.TryGetValue(type, out var del))
            {
                var method = type.GetMethod(ISelfRenderable.NameRenderFun, BindingFlags.Public | BindingFlags.Static);
                if (method != null)
                {
                    del = (Action<Result, Entity>)Delegate.CreateDelegate(typeof(Action<Result, Entity>), method);
                    CachedDelegates[type] = del;
                }
            }
            del?.Invoke(new Result(), entity);
        }
    }

    /// <summary> Checks the type of objects in a map cell </summary>
    private bool ProcessingHeightObstacle(List<InfoObject> infoObject,
        double x, double y,
        int mappedX, int mappedY,
        double depth_h, double depth_v,
        double auxiliary, bool isVertical)
    {
        bool isAdded = false;

        if (!map.Obstacles.TryGetValue((mappedX, mappedY), out var obstacles))
            return false;

        double coordinate = 0;
        double depth = 0;
        if (isVertical)
        {
            coordinate = y;
            depth = depth_v;
        }
        else
        {
            coordinate = x;
            depth = depth_h;
        }


        foreach (var obstacle in obstacles)
        {
            switch (obstacle)
            {
                case ISelfRenderable self:
                    self.ProcessForRendering(UniqueSelfDrawableTypes, ref HasNewTypes);
                    break;

                case IRayRenderable ray:
                    ray.ProcessForRendering(infoObject, coordinate, depth, entity.MaxRenderTile); isAdded = true;
                    break;

                default:
                    throw new Exception($"Invalid object for rendering(CheckAndAddObstacle)\nType: {obstacle.GetType()}");
            }
        }

        return isAdded;
    }

    /// <summary> Filters horizontal and vertical rays by height and range </summary>
    /// /// <returns> Returns a list of InfoObjects to render</returns>
    private static List<InfoObject> FilterVisibleObstacles(List<InfoObject> info, bool rayPassability)
    {
        int count = info.Count;

        if (count <= 1) return info;
        if (!rayPassability && count == 2)
        {
            var first = info[0];
            var second = info[1];

            return new List<InfoObject> { first.depth < second.depth ? first : second };
        }


        var span = CollectionsMarshal.AsSpan(info);
        span.Sort((a, b) => a.depth.CompareTo(b.depth));

        var filtered = new List<InfoObject>(count);
        InfoObject? current = null;
        foreach (var item in span)
        {
            if (current == null || (item.depth > current.depth && item.Object?.Z.Axis > current.Object?.Z.Axis))
            {
                filtered.Add(item);
                current = item;
            }
        }

        return filtered;
    }

    /// <summary> Determines in which axis the ray should move </summary>
    private static void CheckVericals(ref double coordinate, ref double auxiliaryA, double mapCoordinate, double ratio)
    {

        if (ratio >= 0)
        {
            coordinate = mapCoordinate + Screen.Setting.Tile;
            auxiliaryA = 1;
        }
        else
        {
            coordinate = mapCoordinate;
            auxiliaryA = -1;
        }
    }


    /// <summary> Render ready sorted objects (which are rendered using rays) </summary>
    private void RenderRayObstacles(int ray, bool rayPassability, double carAngleRay, List<InfoObject> InfoObject, Result ParallelResult)
    {
        var visibleObstacles = FilterVisibleObstacles(InfoObject, rayPassability);
        int sizeVisibleObst = visibleObstacles.Count;
        for (int obst = 0; obst < sizeVisibleObst; obst++)
        {
            if (obst > 0 && sizeVisibleObst > 1)
                ParallelResult.PositionPreviousObject = visibleObstacles[obst - 1].Object?.GetPositionOnScreen(ParallelResult, entity);

            ParallelResult.CalculationSettingRender(entity, ray, visibleObstacles[obst].depth, visibleObstacles[obst].coordinate, carAngleRay);
            visibleObstacles[obst].Object?.Render(ParallelResult, entity);
        }
    }

    /// <summary> Bresenham's algorithm renders objects </summary>
    public void CalculationAlgorithm(bool rayPassability = true)
    {
        double carAngle = entity.Angle - entity.HalfFov;

        Vector2i coordinates = Screen.MappingVector(entity.X.Axis, entity.Y.Axis);

        double entityX = entity.X.Axis;
        double entityY = entity.Y.Axis;
        double entityDeltaAngle = entity.DeltaAngle;
        double entityMaxRenderTile = entity.MaxRenderTile;
        int tile = Screen.Setting.Tile;

        Parallel.For(0, Screen.Setting.AmountRays, Screen.Setting.ParallelOptions, ray =>
        {
            var ParallelResult = resultobjectPool.Get();
            var ParallelInfoObj = infoObjectPool.Get();

            double hx = 0, x = 0, auxiliaryX = 0, depth_h = 0;
            double vy = 0, y = 0, auxiliaryY = 0, depth_v = 0;

            double carAngleRay = carAngle + ray * entityDeltaAngle;
            double sinA = Math.Sin(carAngleRay);
            double cosA = Math.Cos(carAngleRay);
            ParallelResult.SinCarAngle = sinA;
            ParallelResult.CosCarAngle = cosA;

            CheckVericals(ref x, ref auxiliaryX, coordinates.X, cosA);
            for (int j = 0; j < entityMaxRenderTile; j += tile) 
            {
                depth_v = (x - entityX) / cosA;
                vy = entityY + depth_v * sinA;

                (int, int) mappX = Screen.Mapping(x + auxiliaryX, vy);
                if (map.CheckTrueCoordinates(mappX))
                {
                    if(ProcessingHeightObstacle(ParallelInfoObj, x, vy, mappX.Item1, mappX.Item2, depth_h, depth_v, auxiliaryX, true) && !rayPassability)
                        break;
                }
                else
                    break;

                x += auxiliaryX * tile;
            };

            CheckVericals(ref y, ref auxiliaryY, coordinates.Y, sinA);
            for (int j = 0; j < entityMaxRenderTile; j += tile)
            {
                depth_h = (y - entityY) / sinA;
                hx = entityX + depth_h * cosA;

                (int, int) mappY = Screen.Mapping(hx, y + auxiliaryY);
                if (map.CheckTrueCoordinates(Screen.Mapping(hx, y + auxiliaryY)))
                {
                    if (ProcessingHeightObstacle(ParallelInfoObj, hx, y, mappY.Item1, mappY.Item2, depth_h, depth_v, auxiliaryY, false) && !rayPassability)
                        break;
                }
                else
                    break;

                y += auxiliaryY * tile;
            };


            RenderRayObstacles(ray, rayPassability, carAngleRay, ParallelInfoObj, ParallelResult);

            resultobjectPool.Return(ParallelResult);
            infoObjectPool.Return(ParallelInfoObj);
        });


        if (HasNewTypes)
        {
            PrepareRenderObjects();
            HasNewTypes = false;
        }
        CachedDelegates.ForEach(cd => cd.Value(new Result(), entity));

        ZBuffer.Render();
    }
}
