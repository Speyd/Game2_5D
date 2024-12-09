 using EntityLib;
using MapLib;
using ScreenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using MapLib.Obstacles.DiversityObstacle.SpriteLib;
using EntityLib.Player;
using MapLib.Obstacles;
using static SFML.Window.Mouse;
namespace MoveLib
{
    public class Collision(Map map, MoveLib.Setting setting)
    {
        private int _radiusCheckTouch = 2;
        public int RadiusCheckTouch 
        {
            get => _radiusCheckTouch * Screen.Setting.Tile;
            set => _radiusCheckTouch = value;
        }

        public bool IsCollision(Obstacle obstacle, Entity entity, double nextX, double nextY)
        {
            double playerLeft = nextX - entity.Side;
            double playerRight = nextX + entity.Side;
            double playerTop = nextY - entity.Side;
            double playerBottom = nextY + entity.Side;

            bool isCollidingX = playerRight > obstacle.Left && playerLeft < obstacle.Right;
            bool isCollidingY = playerBottom > obstacle.Top && playerTop < obstacle.Bottom;


            return isCollidingX && isCollidingY && !obstacle.IsPassability;
        }

        private bool IsTouch(Entity entity, double nextX, double nextY)
        {
            var (playerCellX, playerCellY) = Map.Mapping(entity.X, entity.Y, Screen.Setting.Tile);

            int minX = Map.Mapping(playerCellX, Screen.Setting.Tile) - RadiusCheckTouch;
            int maxX = Map.Mapping(playerCellX, Screen.Setting.Tile) + RadiusCheckTouch;
            int minY = Map.Mapping(playerCellY, Screen.Setting.Tile) - RadiusCheckTouch;
            int maxY = Map.Mapping(playerCellY, Screen.Setting.Tile) + RadiusCheckTouch;

            for (int x = minX; x <= maxX; x += Screen.Setting.Tile)
            {
                for (int y = minY; y <= maxY; y += Screen.Setting.Tile)
                {
                    int worldX = x;
                    int worldY = y;

                    if (!map.CheckTrueCoordinates(worldX, worldY))
                        continue;

                    foreach (var obstacle in map.Obstacles[(worldX, worldY)])
                    {
                        if(IsCollision(obstacle, entity, nextX, nextY))
                            return true;
                    }
                }
            }

            return false;
        }

        public void IsCollision(Entity entity, double nextX, double nextY)
        {
            double deltaX = setting.MinDistanceFromWall / 2 * Math.Sign(nextX);
            double deltaY = setting.MinDistanceFromWall / 2 * Math.Sign(nextY);

            if(nextX != 0 && !IsTouch(entity, entity.X + nextX + deltaX, entity.Y))
                entity.X += nextX;
            if (nextY != 0 && !IsTouch(entity, entity.X, entity.Y + nextY + deltaY))
                entity.Y += nextY;
        }
    }
}
