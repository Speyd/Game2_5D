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
using static SFML.Graphics.Font;

namespace BresenhamAlgorithm
{
    //class InfoObject
    //{
    //    public int ray;
    //    public double carAngle;
    //    public double depth;
    //    public double coordinate;
    //    public Obstacle Obstacle;

    //    public InfoObject(int ray, double carAngle, double depth, double coordinate, Obstacle Obstacle)
    //    {
    //        this.ray = ray;
    //        this.carAngle = carAngle;
    //        this.depth = depth;
    //        this.coordinate = coordinate;
    //        this.Obstacle = Obstacle;
    //    }
    //}
    class InfoObject
    {
        public double depth;
        public double coordinate;
        public Obstacle Obstacle;

        public InfoObject(double depth, double coordinate, Obstacle Obstacle)
        {
            this.depth = depth;
            this.coordinate = coordinate;
            this.Obstacle = Obstacle;
        }
    }
    public class Algorithm(Map map, Entity entity, Result result, ZBuffer zBuffer)
    {
        static int _maxVertical = 1200;
        static int MaxVerticalDistance 
        {
            get => _maxVertical;
            set
            {
                if (value <= 0)
                    throw new Exception("Error value MaxVerticalDistance(Algorithm)");
                _maxVertical = value * Screen.Setting.Tile;
            }
        }
       
        static int _maxHorizontal = 1200;
        static int MaxHorizontalDistance
        {
            get => _maxHorizontal;
            set
            {
                if (value <= 0)
                    throw new Exception("Error value MaxVerticalDistance(Algorithm)");
                _maxHorizontal = value * Screen.Setting.Tile; 
            }
        }

        //--------------------------Object Selection--------------------------
        //private ValueTuple<IRenderable?, IRenderable?> obstacles = (null, null);

        //------------------------------Setting Render-------------------------------
        private HashSet<Type> UniqueSelfDrawableTypes { get; init; } = new HashSet<Type>();
        private bool HasNewTypes { get; set; } = false;
        private Dictionary<Type, Action<Result, Entity>> CachedDelegates { get; set; } = new();

        object locker = new object();


        public void PrepareRenderObjects()
        {
            //Console.WriteLine($"uniqueSelfDrawableTypes count: {uniqueSelfDrawableTypes.Count}");
            foreach (var type in UniqueSelfDrawableTypes)
            {
                //Console.WriteLine($"Processing type: {type.Name}");
                if (!CachedDelegates.TryGetValue(type, out var del))
                {
                    var method = type.GetMethod(ISelfRenderable.NameRenderFun, BindingFlags.Public | BindingFlags.Static);
                    if (method != null)
                    {
                        //Console.WriteLine($"Creating delegate for {type.Name}");
                        del = (Action<Result, Entity>)Delegate.CreateDelegate(typeof(Action<Result, Entity>), method);
                        CachedDelegates[type] = del;
                    }
                    else
                    {
                        //Console.WriteLine($"Method RenderSelfDrawableList not found for {type.Name}");
                    }
                }
                del?.Invoke(result, entity);
            }
        }
        private void CheckAndAddHeightObstacle(List<InfoObject> infoObject,
            double x, double y, 
            double depth_h, double depth_v,
            double auxiliary, bool isVertical)
        {
            double mappedX = isVertical ? x + auxiliary : x;
            double mappedY = isVertical ? y : y + auxiliary;


            var key = Screen.Mapping(mappedX, mappedY, Screen.Setting.Tile);
            if (!map.Obstacles.ContainsKey(key))
                return;


            foreach (var obstacle in map.Obstacles[key])
            {
                if (obstacle is ISelfRenderable self)
                {
                    var type = self.GetType();
                    if (!UniqueSelfDrawableTypes.Contains(type))
                    {
                        UniqueSelfDrawableTypes.Add(type);
                        HasNewTypes = true;
                    }
                    self.AddObstacleToRenderList();
                    continue;
                }
                else if (obstacle is IRayRenderable)
                {
                    if (isVertical && depth_v < MaxVerticalDistance)
                        infoObject.Add(new InfoObject(depth_v, y, obstacle));
                    else if (!isVertical && depth_h < MaxHorizontalDistance)
                        infoObject.Add(new InfoObject(depth_h, x, obstacle));
                }
                else
                    throw new Exception("Invalid object for rendering(CheckAndAddObstacle)");
            }
        }
        private bool CheckAndAddLowerObstacle(ref (IRenderable?, IRenderable?) obstacles, 
            double x, double y, 
            double depth_h, double depth_v, 
            double auxiliary, bool isVertical)
        {
            double mappedX = isVertical ? x + auxiliary : x;
            double mappedY = isVertical ? y : y + auxiliary;


            var key = Screen.Mapping(mappedX, mappedY, Screen.Setting.Tile);
            if (!map.Obstacles.ContainsKey(key))
                return false;


            foreach (var obstacle in map.Obstacles[key])
            {
                if (obstacle is ISelfRenderable self)
                {
                    var type = self.GetType();
                    if (!UniqueSelfDrawableTypes.Contains(type))
                    {
                        UniqueSelfDrawableTypes.Add(type);
                        HasNewTypes = true;
                    }
                    self.AddObstacleToRenderList();
                    continue;
                }
                else if (obstacle is IRayRenderable)
                {
                    if (isVertical && depth_v < MaxVerticalDistance)
                        obstacles.Item1 = obstacle;
                    else if(!isVertical && depth_h < MaxHorizontalDistance)
                        obstacles.Item2 = obstacle;

                    return true;
                }
                else
                    throw new Exception("Invalid object for rendering(CheckAndAddObstacle)");
            }
            return false;
        }
        List<InfoObject> FilterVisibleObstacles(List<InfoObject> info)
        {
            if (info.Count == 0) return info;

            InfoObject? current = null;

            info.Sort((a, b) => a.depth.CompareTo(b.depth));
            var filtered = new List<InfoObject>();

            foreach (var item in info)
            {
                if (current == null || (item.depth > current.depth && item.Obstacle.GetLevelHeight() > current.Obstacle.GetLevelHeight()))
                {
                    filtered.Add(item);
                    current = item;
                }
            }

            return filtered;
        }
        private void CheckVericals(ref double a, ref double auxiliaryA, double mapA, double ratio)
        {

            if (ratio >= 0)
            {
                a = mapA + Screen.Setting.Tile;
                auxiliaryA = 1;
            }
            else
            {
                a = mapA;
                auxiliaryA = -1;
            }
        }

        private void RenderHigherObstacles()
        {
            double carAngle = entity.Angle - entity.HalfFov;

            var coordinates = Screen.Mapping(entity.X, entity.Y);

            Parallel.For(0, Screen.Setting.AmountRays, ray =>
            {
                double hx = 0, x = 0, auxiliaryX = 0, depth_h = 0;
                double vy = 0, y = 0, auxiliaryY = 0, depth_v = 0;

                double carAngleRay = carAngle + ray * entity.DeltaAngle;

                double sinA = Math.Sin(carAngleRay);
                double cosA = Math.Cos(carAngleRay);
                List<InfoObject> InfoObject = new List<InfoObject> { };

                CheckVericals(ref x, ref auxiliaryX, coordinates.Item1, cosA);
                for (int j = 0; j < MaxVerticalDistance; j++)
                {
                    depth_v = (x - entity.X) / cosA;
                    vy = entity.Y + depth_v * sinA;

                    if (map.CheckTrueCoordinates(Screen.Mapping(x + auxiliaryX, vy)))
                        CheckAndAddHeightObstacle(InfoObject, x, vy, depth_h, depth_v, auxiliaryX, true);
                    else
                        break;

                    x += auxiliaryX * Screen.Setting.Tile;
                };

                CheckVericals(ref y, ref auxiliaryY, coordinates.Item2, sinA);
                for (int j = 0; j < MaxHorizontalDistance; j++)
                {
                    depth_h = (y - entity.Y) / sinA;
                    hx = entity.X + depth_h * cosA;

                    if (map.CheckTrueCoordinates(Screen.Mapping(hx, y + auxiliaryY)))
                        CheckAndAddHeightObstacle(InfoObject, hx, y, depth_h, depth_v, auxiliaryY, false);
                    else
                        break;

                    y += auxiliaryY * Screen.Setting.Tile;
                };

                Result result1 = new Result();      
                foreach (var obstacle in FilterVisibleObstacles(InfoObject))
                {
                    result1.CalculationSettingRender(entity, ray, obstacle.depth, obstacle.coordinate, carAngleRay);

                    lock(locker)
                        obstacle.Obstacle.Render(result1, entity);
                }
            });
           
            if (HasNewTypes)
            {
                PrepareRenderObjects();
                HasNewTypes = false;
            }

            CachedDelegates.ForEach(cd => cd.Value(result, entity));

            zBuffer.Render();
        }
        private void RenderLowerObstacles()
        {
            double carAngle = entity.Angle - entity.HalfFov;


            var coordinates = Screen.Mapping(entity.X, entity.Y);


            Parallel.For(0, Screen.Setting.AmountRays, ray =>
            {
                ValueTuple<IRenderable?, IRenderable?> obstacles = (null, null);


                double hx = 0, x = 0, auxiliaryX = 0, depth_h = 0;
                double vy = 0, y = 0, auxiliaryY = 0, depth_v = 0;

                double carAngleRay = carAngle + ray * entity.DeltaAngle;

                double sinA = Math.Sin(carAngleRay);
                double cosA = Math.Cos(carAngleRay);

                CheckVericals(ref x, ref auxiliaryX, coordinates.Item1, cosA);
                for (int j = 0; j < MaxVerticalDistance; j += Screen.Setting.Tile)
                {
                    depth_v = (x - entity.X) / cosA;
                    vy = entity.Y + depth_v * sinA;

                    if (map.CheckTrueCoordinates(Screen.Mapping(x + auxiliaryX, vy)))
                    {
                        if (CheckAndAddLowerObstacle(ref obstacles, x, vy, depth_h, depth_v, auxiliaryX, true))
                            break;
                    }
                    else
                        break;

                    x += auxiliaryX * Screen.Setting.Tile;
                }


                CheckVericals(ref y, ref auxiliaryY, coordinates.Item2, sinA);
                for (int j = 0; j < MaxHorizontalDistance; j += Screen.Setting.Tile)
                {

                    depth_h = (y - entity.Y) / sinA;
                    hx = entity.X + depth_h * cosA;

                    if (map.CheckTrueCoordinates(Screen.Mapping(hx, y + auxiliaryY)))
                    {
                        if (CheckAndAddLowerObstacle(ref obstacles, hx, y, depth_h, depth_v, auxiliaryY, false))
                            break;
                    }
                    else
                        break;

                    y += auxiliaryY * Screen.Setting.Tile;
                }

                Result result1 = new Result();
                result1.CalculationSettingRender(entity, obstacles, ray, depth_v, depth_h, hx, vy, carAngleRay);

                if (result1.obstacle is not null)
                {
                    lock (locker)
                        result1.obstacle.Render(result1, entity);
                }
            });

            if (HasNewTypes)
            {
                PrepareRenderObjects();
                HasNewTypes = false;
            }
            CachedDelegates.ForEach(cd => cd.Value(result, entity));

            zBuffer.Render();
        }
        public void CalculationAlgorithm(bool rayPassability = true)
        {
            if (rayPassability)
                RenderHigherObstacles();
            else
                RenderLowerObstacles();
        }
    }
}
