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
using HitBoxLib;
using SFML.Graphics;

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

        public bool AccountHeight {  get; set; } = true;

        public Collision(Map map, MoveLib.Setting setting)
        {
            Map = map;
            Setting = setting;

            RadiusCheckTouch = 2;
        }

        public bool CollisionZ(Obstacle obstacle, Entity entity)
        {
            if(entity.HitBox[HitBoxSideType.DownSide]?.Side >= obstacle.HitBox[HitBoxSideType.DownSide]?.Side &&
               entity.HitBox[HitBoxSideType.UpSide]?.Side <= obstacle.HitBox[HitBoxSideType.UpSide]?.Side) 
            {
                return true;
            }
            else if (entity.HitBox[HitBoxSideType.DownSide]?.Side >= obstacle.HitBox[HitBoxSideType.DownSide]?.Side &&
                     entity.HitBox[HitBoxSideType.DownSide]?.Side <= obstacle.HitBox[HitBoxSideType.UpSide]?.Side &&
                     entity.HitBox[HitBoxSideType.UpSide]?.Side >= obstacle.HitBox[HitBoxSideType.UpSide]?.Side)
            {
                return true;
            }

            return false;
        }
        public bool IsCollision(Obstacle obstacle, Entity entity, double nextX, double nextY)
        {
            double originalEntityX = entity.X.Axis;
            double originalEntityY = entity.Y.Axis;

            entity.X.Axis = nextX;
            entity.Y.Axis = nextY;

            bool isCollidingX = entity.HitBox[HitBoxSideType.Right]?.Side >= obstacle.HitBox[HitBoxSideType.Left]?.Side &&
                                entity.HitBox[HitBoxSideType.Left]?.Side <= obstacle.HitBox[HitBoxSideType.Right]?.Side;

            bool isCollidingY = entity.HitBox[HitBoxSideType.Bottom]?.Side >= obstacle.HitBox[HitBoxSideType.Top]?.Side &&
                                entity.HitBox[HitBoxSideType.Top]?.Side <= obstacle.HitBox[HitBoxSideType.Bottom]?.Side;

            bool isCollidingZ = CollisionZ(obstacle, entity);

            entity.X.Axis = originalEntityX;
            entity.Y.Axis = originalEntityY;
          

            bool generalColliding = false;

            if ((isCollidingX && isCollidingY) == true && isCollidingZ == true)
                generalColliding = true;
            else
                generalColliding = false;

            return AccountHeight? generalColliding && !obstacle.IsPassability : 
                isCollidingX && isCollidingY && !obstacle.IsPassability;
        }

        private bool IsTouch(Entity entity, double nextX, double nextY)
        {
            var (playerCellX, playerCellY) = Screen.Mapping(entity.X.Axis, entity.Y.Axis);

            int minX = playerCellX - RadiusCheckTouch;
            int maxX = playerCellX + RadiusCheckTouch;
            int minY = playerCellY - RadiusCheckTouch;
            int maxY = playerCellY + RadiusCheckTouch;

            for (int x = minX; x <= maxX; x += Screen.Setting.Tile)
            {
                for (int y = minY; y <= maxY; y += Screen.Setting.Tile)
                {
                    if (!Map.Obstacles.ContainsKey((x, y)))
                        continue;

                    foreach (var obstacle in Map.Obstacles[(x, y)])
                    {
                        if (IsCollision(obstacle, entity, nextX, nextY))
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

            if(nextX != 0 && !IsTouch(entity, entity.X.Axis + nextX + deltaX, entity.Y.Axis))
                entity.X.Axis += nextX;
            if (nextY != 0 && !IsTouch(entity, entity.X.Axis, entity.Y.Axis + nextY + deltaY))
                entity.Y.Axis += nextY;
        }
    }
}
