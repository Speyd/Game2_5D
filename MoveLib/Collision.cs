 using EntityLib;
using MapLib;
using ScreenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveLib
{
    public class Collision(Map map, MoveLib.Setting setting)
    {
        public bool IsObstacle(double x, double y)
        {
            var coords = map.Mapping(x, y, Screen.Setting.Tile);
            return map.Obstacles.TryGetValue(coords, out var obstacle) && !obstacle.isPassability;
        }

        public void IsCollision(double nextX, double nextY, Entity entity)
        {
            double deltaX = setting.minDistanceFromWall / 2 * Math.Sign(nextX);
            double deltaY = setting.minDistanceFromWall / 2 * Math.Sign(nextY);

            if (nextX != 0 && !IsObstacle(entity.X + nextX + deltaX, entity.Y))
                entity.X += nextX;

            if (nextY != 0 && !IsObstacle(entity.X, entity.Y + nextY + deltaY))
                entity.Y += nextY;
        }

    }
}
