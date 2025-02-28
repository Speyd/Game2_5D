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
using HitBoxLib.PositionObject;


namespace MoveLib;
public static class Collision
{
    private static int _radiusCheckTouch = Screen.Setting.Tile;
    /// <summary>Collisions hitbox radius definition</summary>
    public static int RadiusCheckTouch 
    {
        get => _radiusCheckTouch;
        set => _radiusCheckTouch = value * Screen.Setting.Tile;
    }

    public static bool CollisionZ(Obstacle obstacle, Entity entity)
    {
        double entityDown = (entity.HitBox[CoordinatePlane.Z, SideSize.Smaller]?.Side ?? 0);
        double entityUp = (entity.HitBox[CoordinatePlane.Z, SideSize.Larger]?.Side ?? 0);
        double obstacleDown = (obstacle.HitBox[CoordinatePlane.Z, SideSize.Smaller]?.Side ?? 0);
        double obstacleUp = (obstacle.HitBox[CoordinatePlane.Z, SideSize.Larger]?.Side ?? 0);



        if (entityDown >= obstacleDown && entityUp <= obstacleUp) 
        {
            return true;
        }
        else if (entityDown >= obstacleDown && entityDown <= obstacleUp && entityUp >= obstacleUp)
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

        var entityHitBox = entity.HitBox;
        var obstacleHitBox = obstacle.HitBox;

        double entityMinX = (entityHitBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0);
        double entityMaxX = (entityHitBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0);
        double obstacleMinX = (obstacleHitBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0);
        double obstacleMaxX = (obstacleHitBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0);

        double entityMinY = (entityHitBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0);
        double entityMaxY = (entityHitBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0);
        double obstacleMinY = (obstacleHitBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0);
        double obstacleMaxY = (obstacleHitBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0);


        bool isCollidingX = entityMaxX >= obstacleMinX && entityMinX <= obstacleMaxX;
        bool isCollidingY = entityMaxY >= obstacleMinY && entityMinY <= obstacleMaxY;
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
