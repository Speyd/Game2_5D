using EntityLib;
using MapLib;
using MapLib.Obstacles;
using Render.InterfaceRender;
using Render.ZBufferRender;
using Render.ResultAlgorithm;
using ScreenLib;
using Render;
using MapLib.Obstacles.DiversityObstacle.SpriteLib;

namespace BresenhamAlgorithm
{
    public class Algorithm(Map map, Entity entity, Result result, ZBuffer zBuffer)
    {
        private ValueTuple<IRenderable, IRenderable> obstacles = (null, null);

        #region CheckedObstacle
        private void IsSprite(IRenderable obstacle)
        {
            if (obstacle is SpriteObstacle sprite)
            {
                if (!SpriteObstacle.SpritesToRender.Contains(sprite))
                {
                    SpriteObstacle.SpritesToRender.Add(sprite);
                }
            }
        }

        //private bool IsRenderObstacle(IRenderable obstacle)
        //{
        //    return obstacle is IRaylessRenderable;
        //}
        private bool CheckAndAddObstacle(double x, double y, double auxiliary, bool isVertical)
        {
            double mappedX = isVertical ? x + auxiliary : x;
            double mappedY = isVertical ? y : y + auxiliary;

            var key = Map.Mapping(mappedX, mappedY, Screen.Setting.Tile);
            foreach (var obstacle in map.Obstacles[key])
            {
                if (obstacle is not IWall)
                {
                    IsSprite(obstacle);
                    continue;
                }

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
            return false;
        }
        #endregion
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

            var coordinates = Map.Mapping(entity.X, entity.Y, Screen.Setting.Tile);

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

                    if (map.CheckTrueCoordinates(Map.Mapping(x + auxiliaryX, vy, Screen.Setting.Tile)))
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

                    if (map.CheckTrueCoordinates(Map.Mapping(hx, y + auxiliaryY, Screen.Setting.Tile)))
                    {
                        if (CheckAndAddObstacle(hx, y, auxiliaryY, false))
                            break;
                    }
                    else
                        break;

                    y += auxiliaryY * Screen.Setting.Tile;
                }


                result.calculationSettingRender(ref entity, ref obstacles, ray, depth_v, depth_h, hx, vy, carAngle);

                if (result.obstacle != null)
                    result.obstacle.Render(result, entity);


                carAngle += entity.DeltaAngle;
            }

            SpriteObstacle.RenderSprites(result, entity);
            zBuffer.Render();
        }
    }
}
