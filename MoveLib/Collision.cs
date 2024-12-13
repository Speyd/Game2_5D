 using EntityLib;
using MapLib;
using ScreenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using ObstacleLib;
using EntityLib.Player;
using static SFML.Window.Mouse;
namespace MoveLib
{
    public class Collision
    {
        private Map Map { get; init; }
        private MoveLib.Setting Setting { get; init; }

        private int _radiusCheckTouch;
        public int RadiusCheckTouch 
        {
            get => _radiusCheckTouch;
            set => _radiusCheckTouch = value * Screen.Setting.Tile;
        }

        public Collision(Map map, MoveLib.Setting setting)
        {
            Map = map;
            Setting = setting;

            RadiusCheckTouch = 2;
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
            var (playerCellX, playerCellY) = Screen.Mapping(entity.X, entity.Y, Screen.Setting.Tile);

            int minX = Screen.Mapping(playerCellX, Screen.Setting.Tile) - RadiusCheckTouch;
            int maxX = Screen.Mapping(playerCellX, Screen.Setting.Tile) + RadiusCheckTouch;
            int minY = Screen.Mapping(playerCellY, Screen.Setting.Tile) - RadiusCheckTouch;
            int maxY = Screen.Mapping(playerCellY, Screen.Setting.Tile) + RadiusCheckTouch;

            for (int x = minX; x <= maxX; x += Screen.Setting.Tile)
            {
                for (int y = minY; y <= maxY; y += Screen.Setting.Tile)
                {
                    int worldX = x;
                    int worldY = y;

                    if (!Map.CheckTrueCoordinates(worldX, worldY))
                        continue;

                    foreach (var obstacle in Map.Obstacles[(worldX, worldY)])
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
            double deltaX = Setting.MinDistanceFromWall / 2 * Math.Sign(nextX);
            double deltaY = Setting.MinDistanceFromWall / 2 * Math.Sign(nextY);

            if(nextX != 0 && !IsTouch(entity, entity.X + nextX + deltaX, entity.Y))
                entity.X += nextX;
            if (nextY != 0 && !IsTouch(entity, entity.X, entity.Y + nextY + deltaY))
                entity.Y += nextY;
        }
    }
}
