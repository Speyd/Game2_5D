using HitBoxLib.Data.HitBoxObject;
using HitBoxLib.Data.Observer;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.PositionObject;
using HitBoxLib.Segment.SignsTypeSide;
using ScreenLib;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCoordinate = (double up, double down);


namespace HitBoxLib.Operations;
/// <summary>Checks beam collision and box hit on all axes</summary>
public static class Collision
{
    /// <summary>Checks for collision between a ray and a hitbox along the X want axis</summary>
    /// <param name="hitBox">Hitbox</param>
    /// <param name="currentRayX">X coordinate ray</param>
    public static bool IsRayTouchesObjectX(Box hitBox, double currentRayX)
    {
        return currentRayX >= hitBox[CoordinatePlane.X, SideSize.Smaller]?.Side &&
               currentRayX <= hitBox[CoordinatePlane.X, SideSize.Larger]?.Side;
    }
    /// <summary>Checks for collision between a ray and a hitbox along the Y want axis</summary>
    /// <param name="hitBox">Hitbox</param>
    /// <param name="currentRayY">Y coordinate ray</param>
    public static bool IsRayTouchesObjectY(Box hitBox, double currentRayY)
    {
        return currentRayY >= hitBox[CoordinatePlane.Y, SideSize.Smaller]?.Side &&
               currentRayY <= hitBox[CoordinatePlane.Y, SideSize.Larger]?.Side;
    }


    private static List<(Vector3f, bool isLarge)> GetVertices(ObserverInfo observerInfo, Box hitBox)
    {
        float minY = (float)(hitBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0f);
        float minX = (float)(hitBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0f);
        float maxY = (float)(hitBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0f);
        float maxX = (float)(hitBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0f);

        float maxZ = (float)(hitBox[CoordinatePlane.Z, SideSize.Larger]?.Side ?? 0f) * HitBoxLib.Operations.Render.MultHeight - observerInfo.position.Z;
        float minZ = (float)(hitBox[CoordinatePlane.Z, SideSize.Smaller]?.Side ?? 0f) * HitBoxLib.Operations.Render.MultHeight - observerInfo.position.Z;
      
        return  new()
        {
            (new Vector3f(minY, maxZ, minX), true),
            (new Vector3f(maxY, maxZ, minX), true),
            (new Vector3f(minY, minZ, minX), false),
            (new Vector3f(maxY, minZ, minX), false),

            // Back edge
            (new Vector3f(minY, maxZ, maxX), true),
            (new Vector3f(maxY, maxZ, maxX), true),
            (new Vector3f(minY, minZ, maxX), false),
            (new Vector3f(maxY, minZ, maxX), false),
        };
    }
    private static (double? max, double? min) GetHitboxBoundaries(RenderInfo renderInfoObj, ObserverInfo observerInfo, Box hitBox)
    {
        List<(Vector3f, bool isLarge)> vertices = GetVertices(observerInfo, hitBox);

        double? max = null;
        double? min = null;

        foreach (var vertex in vertices)
        {
            Vector2f position = new Vector2f(vertex.Item1.Z, vertex.Item1.X);
            float dist = DataPipes.MathUtils.CalculateDistance(position, observerInfo.position);
            float safeDistance = MathF.Max(dist, 0.1f);

            double angleDistance = DataPipes.MathUtils.CalculateAngleToTarget(new Vector2f(vertex.Item1.Z, vertex.Item1.X), observerInfo.position);
            double normalizedAngle = DataPipes.MathUtils.NormalizeAngleDifference(observerInfo.angle, angleDistance);

            float screenX = renderInfoObj.worldToScreenX(normalizedAngle, observerInfo.deltaAngle);
            float screenY = renderInfoObj.worldToScreenY(vertex.Item1.Y, safeDistance, observerInfo.verticalAngle, normalizedAngle);

            if (max is null || max > screenY && vertex.isLarge)
                max = screenY;
            if (min is null || min < screenY && !vertex.isLarge)
                min = screenY;
        }

        return (max, min);
    }
    private static (bool result, double touchCoo) CheckTouchesZ(RenderInfo renderInfoObj, ObserverInfo observerInfo, Box hitBox)
    {
        int halfHeight = Screen.Setting.HalfHeight;

        double dx = (renderInfoObj.position.X - observerInfo.position.X) / Screen.Setting.Tile;
        double dy = (renderInfoObj.position.Y - observerInfo.position.Y) / Screen.Setting.Tile;
        double distance = Math.Sqrt(dx * dx + dy * dy);

        (double? max, double? min) = GetHitboxBoundaries(renderInfoObj, observerInfo, hitBox);


        if (halfHeight >= max && halfHeight <= min)
        {
            float maxZ = (float)(hitBox[CoordinatePlane.Z, SideSize.Larger]?.Side ?? 0f) * HitBoxLib.Operations.Render.MultHeight;
            float minZ = (float)(hitBox[CoordinatePlane.Z, SideSize.Smaller]?.Side ?? 0f) * HitBoxLib.Operations.Render.MultHeight;
            
            float percentage = (float)((halfHeight - min) / (max - min) * 100f);
            double Z = minZ + (percentage / 100f) * (maxZ - minZ); ;

            return (true, Z);
        }
        else
            return (false, 0);
    }

   
    
    /// <summary>Checks for collision between a ray and a hitbox along the Z want axis</summary>
    /// <param name="renderInfoObj">Info about Hitbox object</param>
    /// <param name="observerInfo">Info about observer</param>
    /// <param name="hitBox">Hitbox</param>
    public static (bool result, double coordinate) IsRayTouchesObjectZ(RenderInfo renderInfoObj, ObserverInfo observerInfo, Box hitBox)
    {
        return CheckTouchesZ(renderInfoObj, observerInfo, hitBox);
    }
    /// <summary>Checks for collision between a ray and a hitbox along the Y want axis</summary>
    /// <param name="z">Cooridnate in Z axis</param>
    /// <param name="hitBox">Hitbox</param>
    public static bool IsRayTouchesObjectZ(Box hitBox, double? z)
    {
        double num = hitBox[CoordinatePlane.Z, SideSize.Larger]?.Side ?? 0.0;
        double num2 = hitBox[CoordinatePlane.Z, SideSize.Smaller]?.Side ?? 0.0;
        return z.HasValue && z >= num2 && z <= num;
    }


    /// <summary>Checks for collision in 3D space</summary>
    /// <param name="isCollidingX">collision on the X axis</param>
    /// <param name="isCollidingY">collision on the Y axis</param>
    /// <param name="isCollidingZ">collision on the Z axis</param>
    public static bool IsTouches3D(bool isCollidingX, bool isCollidingY, bool isCollidingZ)
    {
        if ((isCollidingX && isCollidingY) && isCollidingZ)
            return true;

        return false;
    }
    /// <summary>Checks for collision in 3D space</summary>
    /// <param name="touch2D">collision on the X-Y axis</param>
    /// <param name="isCollidingZ">collision on the Z axis</param>
    public static bool IsTouches3D(bool touch2D, bool isCollidingZ)
    {
        if (touch2D && isCollidingZ)
            return true;

        return false;
    }
    /// <summary>Checks for collision in 2D space</summary>
    /// <param name="isCollidingX">collision on the X axis</param>
    /// <param name="isCollidingY">collision on the Y axis</param>
    public static bool IsTouches2D(bool isCollidingX, bool isCollidingY) => isCollidingX && isCollidingY;



    /// <summary>Checks for dropouts on all axes</summary>
    /// <param name="renderInfoObj">Info about Hitbox object</param>
    /// <param name="observerInfo">Info about observer</param>
    /// <param name="box">Box of Hitbox</param>
    /// <param name="currentRayX">X coordinate ray</param>
    /// <param name="currentRayY">Y coordinate ray</param>
    public static (bool result, double coordinateZ) IsRayTouchesObject(RenderInfo renderInfoObj, ObserverInfo observerInfo, Box box, double currentRayX, double currentRayY)
    {
        bool isCollidingX = IsRayTouchesObjectX(box, currentRayX);
        bool isCollidingY = IsRayTouchesObjectY(box, currentRayY);
        var isCollidingZ = IsRayTouchesObjectZ(renderInfoObj, observerInfo, box);

        return (IsTouches3D(isCollidingX, isCollidingY, isCollidingZ.result), isCollidingZ.coordinate);
    }
    /// <summary>Checks for dropouts on all axes</summary>
    /// <param name="renderInfoObj">Info about Hitbox object</param>
    /// <param name="observerInfo">Info about observer</param>
    /// <param name="hitBox">Hitbox</param>
    /// <param name="currentRayX">X coordinate ray</param>
    /// <param name="currentRayY">Y coordinate ray</param>
    public static (bool result, double coordinateZ) IsRayTouchesObject(RenderInfo renderInfoObj, ObserverInfo observerInfo, HitBox hitBox, double currentRayX, double currentRayY)
    {
        var mainToucheHitBox = IsRayTouchesObject(renderInfoObj, observerInfo, hitBox.MainHitBox, currentRayX, currentRayY);
        if (mainToucheHitBox.result)
            return mainToucheHitBox;

        foreach (var box in hitBox.SegmentedHitbox)
        {
            var segmentToucheHitBox = IsRayTouchesObject(renderInfoObj, observerInfo, box, currentRayX, currentRayY);
            if (segmentToucheHitBox.result)
                return segmentToucheHitBox;
        }

        return (false, 0);
    }


    /// <summary>Checks for dropouts on all axes</summary>
    /// <param name="box">Box of Hitbox</param>
    /// <param name="currentRayX">X coordinate ray</param>
    /// <param name="currentRayY">Y coordinate ray</param>
    /// <param name="currentRayZ">Z coordinate ray</param>
    public static bool IsRayTouchesObject(Box box, double currentRayX, double currentRayY, double? currentRayZ)
    {
        bool isCollidingX = IsRayTouchesObjectX(box, currentRayX);
        bool isCollidingY = IsRayTouchesObjectY(box, currentRayY);
        bool isCollidingZ = IsRayTouchesObjectZ(box, currentRayZ);

        return IsTouches3D(isCollidingX, isCollidingY, isCollidingZ);
    }
    /// <summary>Checks for dropouts on all axes</summary>
    /// <param name="hitBox">Hitbox</param>
    /// <param name="currentRayX">X coordinate ray</param>
    /// <param name="currentRayY">Y coordinate ray</param>
    /// <param name="currentRayZ">Z coordinate ray</param>
    public static bool IsRayTouchesObject(HitBox hitBox, double currentRayX, double currentRayY, double? currentRayZ)
    {
        bool mainToucheHitBox = IsRayTouchesObject(hitBox.MainHitBox, currentRayX, currentRayY, currentRayZ);
        if (mainToucheHitBox)
            return true;

        foreach(var box in hitBox.SegmentedHitbox)
        {
            bool segmentToucheHitBox = IsRayTouchesObject(box, currentRayX, currentRayY, currentRayZ);
            if (segmentToucheHitBox)
                return true;
        }

        return false;
    }



    /// <summary>Checks for dropouts on X-Y axes</summary>
    /// <param name="box">Box of Hitbox</param>
    /// <param name="currentRayX">X coordinate ray</param>
    /// <param name="currentRayY">Y coordinate ray</param>
    public static bool IsRayTouchesObject(Box box, double currentRayX, double currentRayY)
    {
        bool isCollidingX = IsRayTouchesObjectX(box, currentRayX);
        bool isCollidingY = IsRayTouchesObjectY(box, currentRayY);

        return IsTouches2D(isCollidingX, isCollidingY);
    }
    /// <summary>Checks for dropouts on X-Y axes</summary>
    /// <param name="hitBox">Hitbox</param>
    /// <param name="currentRayX">X coordinate ray</param>
    /// <param name="currentRayY">Y coordinate ray</param>
    public static bool IsRayTouchesObject(HitBox hitBox, double currentRayX, double currentRayY)
    {
        bool mainToucheHitBox = IsRayTouchesObject(hitBox.MainHitBox, currentRayX, currentRayY);
        if (mainToucheHitBox)
            return true;

        foreach (var box in hitBox.SegmentedHitbox)
        {
            bool segmentToucheHitBox = IsRayTouchesObject(box, currentRayX, currentRayY);
            if (segmentToucheHitBox)
                return true;
        }

        return false;
    }
}
