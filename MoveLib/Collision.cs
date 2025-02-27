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
using SFML.Graphics;
using HitBoxLib.Segment.SignsTypeSide;


namespace MoveLib;
public static class Collision
{
    private static int _radiusCheckTouch = Screen.Setting.Tile;
    public static int RadiusCheckTouch 
    {
        get => _radiusCheckTouch;
        set => _radiusCheckTouch = value * Screen.Setting.Tile;
    }

    public static bool CollisionZ(Obstacle obstacle, Entity entity)
    {
        if(entity.HitBox[SideType.Down]?.Side >= obstacle.HitBox[SideType.Down]?.Side &&
           entity.HitBox[SideType.Up]?.Side <= obstacle.HitBox[SideType.Up]?.Side) 
        {
            return true;
        }
        else if (entity.HitBox[SideType.Down]?.Side >= obstacle.HitBox[SideType.Down]?.Side &&
                 entity.HitBox[SideType.Down]?.Side <= obstacle.HitBox[SideType.Up]?.Side &&
                 entity.HitBox[SideType.Up]?.Side >= obstacle.HitBox[SideType.Up]?.Side)
        {
            return true;
        }

        return false;
    }
    public static bool IsCollision(Obstacle obstacle, Entity entity, double nextX, double nextY)
    {
        double originalEntityX = entity.X.Axis;
        double originalEntityY = entity.Y.Axis;

        entity.X.Axis = nextX;
        entity.Y.Axis = nextY;

        bool isCollidingX = entity.HitBox[SideType.Right]?.Side >= obstacle.HitBox[SideType.Left]?.Side &&
                            entity.HitBox[SideType.Left]?.Side <= obstacle.HitBox[SideType.Right]?.Side;

        bool isCollidingY = entity.HitBox[SideType.Bottom]?.Side >= obstacle.HitBox[SideType.Top]?.Side &&
                            entity.HitBox[SideType.Top]?.Side <= obstacle.HitBox[SideType.Bottom]?.Side;

        bool isCollidingZ = CollisionZ(obstacle, entity);

        entity.X.Axis = originalEntityX;
        entity.Y.Axis = originalEntityY;
      

        bool generalColliding = false;

        if ((isCollidingX && isCollidingY) == true && isCollidingZ == true)
            generalColliding = true;
        else
            generalColliding = false;

        return generalColliding && !obstacle.IsPassability;
    }

    private static bool IsTouch(Map Map, Entity entity, double nextX, double nextY)
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


    public static void IsCollision(Map Map, Entity entity, double nextX, double nextY)
    {
        double deltaX = Setting.MinDistanceFromWall / 2 * Math.Sign(nextX);
        double deltaY = Setting.MinDistanceFromWall / 2 * Math.Sign(nextY);

        if(nextX != 0 && !IsTouch(Map, entity, entity.X.Axis + nextX + deltaX, entity.Y.Axis))
            entity.X.Axis += nextX;
        if (nextY != 0 && !IsTouch(Map, entity, entity.X.Axis, entity.Y.Axis + nextY + deltaY))
            entity.Y.Axis += nextY;
    }
}
