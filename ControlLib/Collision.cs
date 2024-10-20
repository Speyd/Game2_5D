using EntityLib;
using MapLib;
using ScreenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlLib
{
    internal class Collision(Screen screen, Map map, Setting setting)
    {
        public bool IsObstacle(double x, double y)
        {
            var coords = map.mapping(x, y, screen.Setting.Tile);
            return map.Obstacles.TryGetValue(coords, out var obstacle) && !obstacle.isPassability;
        }

        public void isCollision(double nextX, double nextY, Entity entity)
        {
            double deltaX = setting.minDistanceFromWall / 2 * Math.Sign(nextX);
            double deltaY = setting.minDistanceFromWall / 2 * Math.Sign(nextY);

            if (nextX != 0 && !IsObstacle(entity.getEntityX() + nextX + deltaX, entity.getEntityY()))
                entity.getEntityX() += nextX;

            if (nextY != 0 && !IsObstacle(entity.getEntityX(), entity.getEntityY() + nextY + deltaY))
                entity.getEntityY() += nextY;
        }

    }
}
