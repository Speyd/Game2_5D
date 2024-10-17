using EntityLib;
using MapLib;
using MapLib.Obstacles;
using MapLib.Obstacles.Diversity_Obstacle;
using Render;
using Render.ResultAlgorithm;
using ScreenLib;

namespace BresenhamAlgorithm
{
    public class Algorithm(Screen screen, Map map, Entity entity, Result result)
    {
        private ValueTuple<IRenderable, IRenderable> obstacles = (null, null);

        #region CheckedObstacle
        private void isSprite(IRenderable obstacle)
        {
            if (obstacle is MapLib.Obstacles.Diversity_Obstacle.Sprite sprite)
            {
                if (!MapLib.Obstacles.Diversity_Obstacle.Sprite.spritesToRender.Contains(sprite))
                {
                    MapLib.Obstacles.Diversity_Obstacle.Sprite.spritesToRender.Add(sprite);
                }
            }
        }

        private bool isRenderObstacle(IRenderable obstacle)
        {
            return !Obstacle.obstaclesIgnoringRendering.Any(t => t.GetType() == obstacle.GetType());
        }
        private bool checkAndAddObstacle(double x, double y, double auxiliary, bool isVertical)
        {
            double mappedX = isVertical ? x + auxiliary : x;
            double mappedY = isVertical ? y : y + auxiliary;

            var key = map.mapping(mappedX, mappedY, screen.Setting.Tile);
            if (map.Obstacles.TryGetValue(key, out var obstacle))
            {
                isSprite(obstacle);


                if(isRenderObstacle(obstacle) == false)
                    return false;

                if (isVertical)
                {
                    obstacles.Item1 = obstacle;
                }
                else
                {
                    obstacles.Item2 = obstacle;
                }
                return true;
            }

            return false;
        }
        #endregion
        private void checkVericals(ref double a, ref double auxiliaryA, double mapA, double ratio)
        {

            if (ratio >= 0)
            {
                a = mapA + screen.Setting.Tile;
                auxiliaryA = 1;
            }
            else
            {
                a = mapA;
                auxiliaryA = -1;
            }
        }

        public void calculationAlgorithm()
        {
            double carAngle = entity.getEntityA() - entity.HalfFov;

            double entityX = entity.getEntityX();
            double entityY = entity.getEntityY();


            double hx = 0, x = 0, auxiliaryX = 0, depth_h = 0;
            double vy = 0, y = 0, auxiliaryY = 0, depth_v = 0;

            var coordinates = map.mapping(entityX, entityY, screen.Setting.Tile);

            double sinA, cosA;

            for (int ray = 0; ray < screen.Setting.AmountRays; ray++)
            {
                sinA = Math.Sin(carAngle);
                cosA = Math.Cos(carAngle);

                checkVericals(ref x, ref auxiliaryX, coordinates.Item1, cosA);
                for (int j = 0; j < screen.ScreenWidth; j += screen.Setting.Tile)
                {
                    depth_v = (x - entityX) / cosA;
                    vy = entityY + depth_v * sinA;

                    if (map.Obstacles.ContainsKey(map.mapping(x + auxiliaryX, vy, screen.Setting.Tile)))
                    {
                        if (checkAndAddObstacle(x, vy, auxiliaryX, true))
                            break;
                    }

                    x += auxiliaryX * screen.Setting.Tile;
                }


                checkVericals(ref y, ref auxiliaryY, coordinates.Item2, sinA);

                for (int j = 0; j < screen.ScreenHeight; j += screen.Setting.Tile)
                {
                    depth_h = (y - entityY) / sinA;
                    hx = entityX + depth_h * cosA;

                    if (map.Obstacles.ContainsKey(map.mapping(hx, y + auxiliaryY, screen.Setting.Tile)))
                    {
                        if (checkAndAddObstacle(hx, y, auxiliaryY, false))
                            break;
                    }

                    y += auxiliaryY * screen.Setting.Tile;
                }


                result.calculationSettingRender(ref screen, ref entity, ref obstacles, ray, depth_v, depth_h, hx, vy, carAngle);

                if (result.obstacle != null && isRenderObstacle(result.obstacle) == true)
                    result.obstacle.render(screen, result, entity);


                carAngle += entity.DeltaAngle;
            }

            Obstacle.obstaclesIgnoringRendering.ToList().ForEach
                (
                o => o.render(screen, result, entity)
                );
        }
    }
}
