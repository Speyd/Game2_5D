using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLib;
using MapLib;
using ScreenLib;
using SFML.Graphics;
using static SFML.Window.Mouse;
using Render.RenderInterface;
using System.Reflection.Metadata;
using EntityLib.Player;
using SFML.Window;
using static OpenTK.Graphics.OpenGL.GL;
using static System.Runtime.InteropServices.JavaScript.JSType;
using HitBoxLib.PositionObject;
using System.Numerics;
using SFML.System;
using HitBoxLib.Segment.SignsTypeSide;
using HitBoxLib.Operations;
using Render.Object;


namespace RayTracingLib;
public static class Raycast
{
    private const float epsilon = 0.000001f;

    public static int _scanRadius = 1;
    /// <summary>Beam scanning radius at each step</summary>
    private static int ScanRadius 
    {
        get => _scanRadius;
        set
        {
            if (value < 0)
                _scanRadius = 0;
            else
                _scanRadius = value;
        }
    }


    public static List<double> GetIntersectionParameter(Entity entity,
                float minX, float maxX,
                float minY, float maxY)
    {
        List<double> tValues = new();
        double dirX = entity.Direction.X;
        double dirY = entity.Direction.Y;

        if (dirX != 0)
        {
            double t1 = (minX - entity.X.Axis) / dirX;
            double t2 = (maxX - entity.X.Axis) / dirX;

            tValues.Add(t1);
            tValues.Add(t2);
        }
        if (dirY != 0)
        {
            double t3 = (minY - entity.Y.Axis) / dirY;
            double t4 = (maxY - entity.Y.Axis) / dirY;

            tValues.Add(t3);
            tValues.Add(t4);
        }

        return tValues;
    }
    public static bool IntersectionCalculation(IObject obj, 
                Entity entity, double tValue,
                float minX, float maxX,
                float minY, float maxY)
    {
        double xInter = entity.X.Axis + tValue * entity.Direction.X;
        double yInter = entity.Y.Axis + tValue * entity.Direction.Y;

        Vector3f obstaclePos = new Vector3f((float)xInter, (float)yInter, (float)obj.Z.Axis);


        bool result = Collision.IsRayTouchesObject(obstaclePos, entity.GetObserverInfo(), obj.HitBox.MainHitBox, xInter, yInter);

        return result;
    }

    private static void DetailedSearchInCell(List<IObject> colisionObject, List<IObject> objs, Entity entity)
    {

        foreach (var obj in objs)
        {
            Vector3f obstaclePos = new Vector3f((float)obj.X.Axis, (float)obj.Y.Axis, (float)obj.Z.Axis);
            var mainHitBox = obj.HitBox.MainHitBox;

            float minX = (float)(mainHitBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0);
            float maxX = (float)(mainHitBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0);
            float minY = (float)(mainHitBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0);
            float maxY = (float)(mainHitBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0);

            List<double> tValues = GetIntersectionParameter(entity, minX, maxX, minY, maxY);

            foreach (var tValue in tValues)
            {
                if (tValue < 0) continue;

                if (IntersectionCalculation(obj, entity, tValue, minX, maxX, minY, maxY))
                {
                    colisionObject.Add(obj);
                    break;
                }
            }
        }
    }
    public static IObject? GetFirstTouchedObject(List<IObject> colisionObject, Entity entity)
    {
        IObject? nearestObstacle = null;
        double nearestDistance = double.MaxValue;

        foreach (var obstacle in colisionObject)
        {
            Vector3f obstaclePos = new Vector3f((float)obstacle.X.Axis, (float)obstacle.Y.Axis, (float)obstacle.Z.Axis);
            var mainHitBox = obstacle.HitBox.MainHitBox;

            float minX = (float)(mainHitBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0);
            float maxX = (float)(mainHitBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0);
            float minY = (float)(mainHitBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0);
            float maxY = (float)(mainHitBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0);

            List<double> tValues = GetIntersectionParameter(entity, minX, maxX, minY, maxY);

            foreach (var tValue in tValues)
            {
                if (tValue < 0) continue;

                if (IntersectionCalculation(obstacle, entity, tValue, minX, maxX, minY, maxY) && tValue < nearestDistance)
                {
                    nearestDistance = tValue;
                    nearestObstacle = obstacle;
                }
            }
        }

        return nearestObstacle;
    }


    public static IObject? RaycastFun(Map map, Entity entity)
    {
        double dx = entity.Direction.X;
        double dy = entity.Direction.Y;

        if (Math.Abs(dx) < epsilon) dx = dx < 0 ? -epsilon : epsilon;
        if (Math.Abs(dy) < epsilon) dy = dy < 0 ? -epsilon : epsilon;

        int tileSize = Screen.Setting.Tile;

        double startX = entity.X.Axis;
        double startY = entity.Y.Axis;

        int gridX = (int)(startX / tileSize) * tileSize;
        int gridY = (int)(startY / tileSize) * tileSize;

        int stepX = dx > 0 ? tileSize : -tileSize;
        int stepY = dy > 0 ? tileSize : -tileSize;

        double tDeltaX = Math.Abs(Screen.Setting.Tile / dx);
        double tDeltaY = Math.Abs(Screen.Setting.Tile / dy);

        double tMaxX = dx > 0 ? (gridX + tileSize - startX) / dx : (startX - gridX) / -dx;
        double tMaxY = dy > 0 ? (gridY + tileSize - startY) / dy : (startY - gridY) / -dy;

        while (true)
        {
            List<IObject> colisionObject = new();

            for (int radius = 0; radius <= ScanRadius; radius++)
            {
                for (int xOffset = -radius; xOffset <= radius; xOffset++)
                {
                    for (int yOffset = -radius; yOffset <= radius; yOffset++)
                    {
                        var scanX = gridX + xOffset * tileSize;
                        var scanY = gridY + yOffset * tileSize;

                        if (map.Obstacles.ContainsKey((scanX, scanY)))
                            DetailedSearchInCell(colisionObject, map.Obstacles[(scanX, scanY)], entity);
                    }
                }
            }

            IObject? findObstacle = GetFirstTouchedObject(colisionObject, entity);
            if (findObstacle != null)
                return findObstacle;

            if (tMaxX < tMaxY)
            {
                gridX += stepX;
                tMaxX += tDeltaX;
            }
            else
            {
                gridY += stepY;
                tMaxY += tDeltaY;
            }

            if (gridX < 0 || gridY < 0 ||
                gridX >= map.Setting.MapWidth * tileSize ||
                gridY >= map.Setting.MapHeight * tileSize)
            {
                return null;
            }
        }
    }

}
