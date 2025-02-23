using EntityLib;
using MapLib;
using Render.InterfaceRender;
using Render.ZBufferRender;
using Render.ResultAlgorithm;
using ScreenLib;
using Render;
using NGenerics.Extensions;
using System.Reflection;
using Render.RenderInterface;
using System.Collections.ObjectModel;
using SFML.System;
using SFML.Graphics;
using ObstacleLib;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using DataPipes.Pool;

using static SFML.Graphics.Font;
using static SFML.Window.Joystick;
using System.Buffers;
using EntityLib.Player;
using ObstacleLib.SpriteLib;

namespace BresenhamAlgorithm
{  
    public class Algorithm(Map map, Entity entity)
    {
        //------------------------------Pool-------------------------------
        static ObjectPool<Result> resultobjectPool = new ObjectPool<Result>();
        static ListPool<InfoObject> infoObjectPool = new ListPool<InfoObject>();


        //------------------------------Setting Render-------------------------------
        private ConcurrentDictionary<Type, bool> UniqueSelfDrawableTypes { get; init; } = new();
        private Dictionary<Type, Action<Result, Entity>> CachedDelegates { get; set; } = new();

        private bool HasNewTypes = false;

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

        private bool ProcessingHeightObstacle(List<InfoObject> infoObject,
            double x, double y, 
            double depth_h, double depth_v,
            double auxiliary, bool isVertical)
        {
            bool isAdded = false;
            double mappedX = isVertical ? x + auxiliary : x;
            double mappedY = isVertical ? y : y + auxiliary;


            if (!map.Obstacles.TryGetValue(Screen.Mapping(mappedX, mappedY, Screen.Setting.Tile), out var obstacles))
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
                        throw new Exception("Invalid object for rendering(CheckAndAddObstacle)");
                }
            }

            return isAdded;
        }
        List<InfoObject> FilterVisibleObstacles(List<InfoObject> info, bool rayPassability)
        {
            if (info.Count <= 1) return info;
            if (info.Any(item => item.depth != info[0].depth))
            {
                info.Sort((a, b) => a.depth.CompareTo(b.depth));
            }
            if (!rayPassability && info.Count == 2)
            {
                return new List<InfoObject> { info[0].depth < info[1].depth ? info[0] : info[1] };
            }

            InfoObject? current = null;
            var filtered = new List<InfoObject>();

            foreach (var item in info)
            {
                var zCoordinate = item.Obstacle?.GetZCoordinate();

                if (current == null ||
                    (item.depth > current.depth && zCoordinate.HasValue && zCoordinate > current.Obstacle?.GetZCoordinate()))
                {
                    filtered.Add(item);
                    current = item;
                }
            }

            return filtered;
        }

        private void CheckVericals(ref double coordinate, ref double auxiliaryA, double mapCoordinate, double ratio)
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



        private void RenderRayObstacles(int ray, bool rayPassability, double carAngleRay, List<InfoObject> InfoObject, Result ParallelResult)
        {
            var visibleObstacles = FilterVisibleObstacles(InfoObject, rayPassability);
            int sizeVisibleObst = visibleObstacles.Count;
            for (int obst = 0; obst < sizeVisibleObst; obst++)
            {
                if (obst > 0 && sizeVisibleObst > 1)
                    ParallelResult.PositionPreviousObject = visibleObstacles[obst - 1].Obstacle?.GetPositionOnScreen(ParallelResult, entity);

                ParallelResult.CalculationSettingRender(entity, ray, visibleObstacles[obst].depth, visibleObstacles[obst].coordinate, carAngleRay);
                visibleObstacles[obst].Obstacle?.Render(ParallelResult, entity);
            }
        }
        public void CalculationAlgorithm(bool rayPassability = true)
        {
            double carAngle = entity.Angle - entity.HalfFov;

            Vector2i coordinates = Screen.MappingVector(entity.X.Axis, entity.Y.Axis);

            Parallel.For(0, Screen.Setting.AmountRays, Screen.Setting.ParallelOptions, ray =>
            {
                var ParallelResult = resultobjectPool.Get();
                var ParallelInfoObj = infoObjectPool.Get();

                double hx = 0, x = 0, auxiliaryX = 0, depth_h = 0;
                double vy = 0, y = 0, auxiliaryY = 0, depth_v = 0;

                double carAngleRay = carAngle + ray * entity.DeltaAngle;
                double sinA = Math.Sin(carAngleRay);
                double cosA = Math.Cos(carAngleRay);
                ParallelResult.SinCarAngle = sinA;
                ParallelResult.CosCarAngle = cosA;

                CheckVericals(ref x, ref auxiliaryX, coordinates.X, cosA);
                for (int j = 0; j < entity.MaxRenderTile; j += Screen.Setting.Tile) 
                {
                    depth_v = (x - entity.X.Axis) / cosA;
                    vy = entity.Y.Axis + depth_v * sinA;

                    if (map.CheckTrueCoordinates(Screen.Mapping(x + auxiliaryX, vy)))
                    {
                        if(ProcessingHeightObstacle(ParallelInfoObj, x, vy, depth_h, depth_v, auxiliaryX, true) && !rayPassability)
                            break;
                    }
                    else
                        break;

                    x += auxiliaryX * Screen.Setting.Tile;
                };

                CheckVericals(ref y, ref auxiliaryY, coordinates.Y, sinA);
                for (int j = 0; j < entity.MaxRenderTile; j += Screen.Setting.Tile)
                {
                    depth_h = (y - entity.Y.Axis) / sinA;
                    hx = entity.X.Axis + depth_h * cosA;

                    if (map.CheckTrueCoordinates(Screen.Mapping(hx, y + auxiliaryY)))
                    {
                        if (ProcessingHeightObstacle(ParallelInfoObj, hx, y, depth_h, depth_v, auxiliaryY, false) && !rayPassability)
                            break;
                    }
                    else
                        break;

                    y += auxiliaryY * Screen.Setting.Tile;
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
}
