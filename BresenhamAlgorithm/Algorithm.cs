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

namespace BresenhamAlgorithm
{
    public class Algorithm(Map map, Entity entity, Result result, ZBuffer zBuffer)
    {
        //--------------------------Object Selection--------------------------
        private ValueTuple<IRenderable, IRenderable> obstacles = (null, null);

        //------------------------------Setting Render-------------------------------
        private HashSet<Type> UniqueSelfDrawableTypes { get; init; } = new HashSet<Type>();
        private bool HasNewTypes { get; set; } = false;
        private Dictionary<Type, Action<Result, Entity>> CachedDelegates { get; set; } = new();

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
        private bool CheckAndAddObstacle(double x, double y, double auxiliary, bool isVertical)
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
                    if (isVertical)
                    {
                        obstacles.Item1 = obstacle;
                        return true;
                    }
                    else
                    {
                        obstacles.Item2 = obstacle;
                        return true;
                    }
                }
                else
                    throw new Exception("Invalid object for rendering(CheckAndAddObstacle)");
            }
            return false;
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

        public void CalculationAlgorithm()
        {
            double carAngle = entity.Angle - entity.HalfFov;

            double hx = 0, x = 0, auxiliaryX = 0, depth_h = 0;
            double vy = 0, y = 0, auxiliaryY = 0, depth_v = 0;

            var coordinates = Screen.Mapping(entity.X, entity.Y);

            double sinA, cosA;

            for (int ray = 0; ray < Screen.Setting.AmountRays; ray++)
            {
                sinA = Math.Sin(carAngle);
                cosA = Math.Cos(carAngle);

                CheckVericals(ref x, ref auxiliaryX, coordinates.Item1, cosA);
                for (int j = 0; j < Screen.ScreenWidth; j += Screen.Setting.Tile)
                {
                    depth_v = (x - entity.X) / cosA;
                    vy = entity.Y + depth_v * sinA;

                    if (map.CheckTrueCoordinates(Screen.Mapping(x + auxiliaryX, vy)))
                    {
                        if (CheckAndAddObstacle(x, vy, auxiliaryX, true))
                            break;
                    }
                    else
                        break;

                    x += auxiliaryX * Screen.Setting.Tile;
                }


                CheckVericals(ref y, ref auxiliaryY, coordinates.Item2, sinA);
                for (int j = 0; j < Screen.ScreenHeight; j += Screen.Setting.Tile)
                {
                    
                    depth_h = (y - entity.Y) / sinA;
                    hx = entity.X + depth_h * cosA;

                    if (map.CheckTrueCoordinates(Screen.Mapping(hx, y + auxiliaryY)))
                    {
                        if (CheckAndAddObstacle(hx, y, auxiliaryY, false))
                            break;
                    }
                    else
                        break;

                    y += auxiliaryY * Screen.Setting.Tile;
                }


                result.CalculationSettingRender(entity, ref obstacles, ray, depth_v, depth_h, hx, vy, carAngle);

                if (result.obstacle is not null)
                    result.obstacle.Render(result, entity);

                carAngle += entity.DeltaAngle;
            }

            if (HasNewTypes)
            {
                PrepareRenderObjects();
                HasNewTypes = false;
            }
            CachedDelegates.ForEach(cd => cd.Value(result, entity));

            zBuffer.Render();
        }
    }
}
