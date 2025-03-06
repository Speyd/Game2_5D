using EntityLib;
using ScreenLib;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLib;
using HitBoxLib;
using HitBoxLib.PositionObject;
using HitBoxLib.Segment.SignsTypeSide;
using ProtoRender.RenderAlgorithm;
using DataPipes;
using ProtoRender.Object;

namespace RayTracingLib.Detection;
static public class RayDetectionX
{
    /// <summary>Defines the side faces of the object</summary>
    static private (Vector2f, float) DetermineCornerWallSide(IObject obj, 
        Entity entity, float cosAngle, float sinAngle,
        float minX, float maxX, float maxY, float minY)
    {
        float tempValue = float.MaxValue;

        if (cosAngle != 0)
        {
            float tVerticalLeft = (float)(minX - entity.X.Axis) / cosAngle;
            if (tVerticalLeft >= 0)
            {
                float hitYLeft = (float)entity.Y.Axis + tVerticalLeft * sinAngle;
                if (hitYLeft >= minY && hitYLeft <= maxY)
                    tempValue = tVerticalLeft;
            }

            float tVerticalRight = (float)(maxX - entity.X.Axis) / cosAngle;
            if (tVerticalRight >= 0)
            {
                float hitYRight = (float)entity.Y.Axis + tVerticalRight * sinAngle;
                if (hitYRight >= minY && hitYRight <= maxY)
                    tempValue = Math.Min(tempValue, tVerticalRight);
            }
        }
        if (sinAngle != 0)
        {
            float tHorizontalTop = (float)(minY - entity.Y.Axis) / sinAngle;
            if (tHorizontalTop >= 0)
            {
                float hitXTop = (float)entity.X.Axis + tHorizontalTop * cosAngle;
                if (hitXTop >= minX && hitXTop <= maxX)
                    tempValue = Math.Min(tempValue, tHorizontalTop);
            }

            float tHorizontalBottom = (float)(maxY - entity.Y.Axis) / sinAngle;
            if (tHorizontalBottom >= 0)
            {
                float hitXBottom = (float)entity.X.Axis + tHorizontalBottom * cosAngle;
                if (hitXBottom >= minX && hitXBottom <= maxX)
                    tempValue = Math.Min(tempValue, tHorizontalBottom);
            }
        }

        if (tempValue == float.MaxValue)
            return (new Vector2f(-1, -1), 0);

        float hitX = (float)entity.X.Axis + tempValue * cosAngle - (float)obj.X.Axis;
        float hitY = (float)entity.Y.Axis + tempValue * sinAngle - (float)obj.Y.Axis;

        return (new Vector2f(hitX, hitY), tempValue);
    }
        /// <summary>Defines the main sides of the object</summary>
    static private ObjectSide DetermineMainSide(Entity entity, float cosAngle, float sinAngle,
        float minX, float maxX, float maxY, float minY)   
    {
        if (entity.Y.Axis >= minY && entity.Y.Axis <= maxY)
        {
            if (cosAngle > 0 && entity.X.Axis <= maxX)
                return ObjectSide.Right;
            else if (cosAngle < 0 && entity.X.Axis >= minX)
                return ObjectSide.Left;
        }

        if (entity.X.Axis >= minX && entity.X.Axis <= maxX)
        {
            if (sinAngle > 0 && entity.Y.Axis <= maxY)
                return ObjectSide.Bottom;
            else if (sinAngle < 0 && entity.Y.Axis >= minY)
                return ObjectSide.Top;
        }

        return ObjectSide.Error;
    }




    static private void SetTextureWall(ref ObjectSide oldWallTexture, ObjectSide newWallTexture)
    {
        if (oldWallTexture == ObjectSide.Error)
            oldWallTexture = newWallTexture;
    }
    static private ObjectSide RedefiningWallSides(ObjectSide wallDetermine)
    {
        switch (wallDetermine)
        {
            case ObjectSide.LeftCorner:
                return ObjectSide.Left;
            case ObjectSide.RightCorner:
                return ObjectSide.Right;
            case ObjectSide.TopCorner:
                return ObjectSide.Top;
            case ObjectSide.BottomCorner:
                return ObjectSide.Bottom;
            default:
                return wallDetermine;
        }
    }
    static private void CalculateDistanceToWall(IObject obstacle, Entity entity, ref float distanceToWall, ObjectSide wallDetermine)
    {
        distanceToWall = (float)DataPipes.MathUtils.CalculateDistance(obstacle.X.Axis, obstacle.Y.Axis, entity.X.Axis, entity.Y.Axis);

        if (wallDetermine == ObjectSide.Top || wallDetermine == ObjectSide.Left)
            distanceToWall -= -(Screen.Setting.Tile * 3);
    }
    static private HitPoint CalculateHitPoint(Vector2f cornerHit, float distanceToPoint, float distanceToWall, ObjectSide wallDetermine)
    {
        if (cornerHit.X > cornerHit.Y)
        {
            cornerHit.X /= Screen.Setting.Tile;
            if (cornerHit.X < cornerHit.Y)
            {
                cornerHit.Y /= Screen.Setting.Tile;
                cornerHit.X = 0;
                SetTextureWall(ref wallDetermine, ObjectSide.LeftCorner);
            }
            else
                SetTextureWall(ref wallDetermine, ObjectSide.BottomCorner);
        }
        else
        {
            cornerHit.Y /= Screen.Setting.Tile;
            if (cornerHit.X > cornerHit.Y)
            {
                cornerHit.X /= Screen.Setting.Tile;
                cornerHit.Y = 0;
                SetTextureWall(ref wallDetermine, ObjectSide.TopCorner);
            }
            else
                SetTextureWall(ref wallDetermine, ObjectSide.RightCorner);
        }

        ObjectSide textureWallDetermine = RedefiningWallSides(wallDetermine);
        return new HitPoint(cornerHit, distanceToPoint, distanceToWall, wallDetermine, textureWallDetermine);
    }



    static public HitPoint DetermineHitObjectSides(IObject obj, Entity entity)
    {
        float distanceToPoint = 0;
        float distanceToWall = 0;


        float cosAngle = entity.Direction.X;
        float sinAngle = entity.Direction.Y;

        var mainHitBox = obj.HitBox.MainHitBox;
        float minX = (float)(mainHitBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0f);
        float maxX = (float)(mainHitBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0f);
        float minY = (float)(mainHitBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0f);
        float maxY = (float)(mainHitBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0f);


        ObjectSide wallDetermine = ObjectSide.Error;
        wallDetermine = DetermineMainSide(entity, cosAngle, sinAngle, minX, maxX, maxY, minY);

        CalculateDistanceToWall(obj, entity, ref distanceToWall, wallDetermine);

        (Vector2f cornerHit, distanceToPoint) = DetermineCornerWallSide(obj, entity, cosAngle, sinAngle, minX, maxX, maxY, minY);
        return CalculateHitPoint(cornerHit, distanceToPoint, distanceToWall, wallDetermine);
    }
    static private ObjectSide Determine(IObject obj, Entity entity, float cosAngle, float sinAngle)
    {
        var mainHitBox = obj.HitBox.MainHitBox;

        float minX = (float)(mainHitBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0f);
        float maxX = (float)(mainHitBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0f);
        float maxY = (float)(mainHitBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0f);
        float minY = (float)(mainHitBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0f);


        ObjectSide wallDetermine = ObjectSide.Error;
        wallDetermine = DetermineMainSide(entity, cosAngle, sinAngle, minX, maxX, maxY, minY);
        if (wallDetermine != ObjectSide.Error)
            return wallDetermine;

        Vector2f cornerHit = DetermineCornerWallSide(obj, entity, cosAngle, sinAngle, minX, maxX, maxY, minY).Item1;
        if (cornerHit.X > cornerHit.Y)
        {
            cornerHit.X /= Screen.Setting.Tile;
            if (cornerHit.X < cornerHit.Y)
                return ObjectSide.Left;
            else
                return ObjectSide.Bottom;
        }
        else
        {
            cornerHit.Y /= Screen.Setting.Tile;
            if (cornerHit.X > cornerHit.Y)
                return ObjectSide.Top;
            else
                return ObjectSide.Right;
        }
    }
    static public ObjectSide DetermineObjectSides(IObject obj, Entity entity, Result? result = null)
    {
        float cosAngle = (float)(result?.CosCarAngle ?? entity.Direction.X);
        float sinAngle = (float)(result?.SinCarAngle ?? entity.Direction.Y);

        return Determine(obj, entity, cosAngle, sinAngle);
    }
    static public ObjectSide DetermineObjectSides(IObject obj, Entity entity)
    {
        return Determine(obj, entity, entity.Direction.X, entity.Direction.Y);
    }
}
