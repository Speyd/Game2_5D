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


namespace HitBoxLib.Operations;
public static class Collision
{
    const double multUp = 0.003;
    const double multDown = 0.0085;


    public static bool IsRayTouchesObjectX(Box hitBox, double currentRayX)
    {
        return currentRayX > hitBox[SideType.Left]?.Side &&
               currentRayX < hitBox[SideType.Right]?.Side;
    }
    public static bool IsRayTouchesObjectY(Box hitBox, double currentRayY)
    {
        return currentRayY > hitBox[SideType.Top]?.Side &&
               currentRayY < hitBox[SideType.Bottom]?.Side;
    }


    static double GetMult(double side, double mult, double distance)
    {
        return (side * mult * Screen.ScreenRatio) / distance;
    }
    public static bool IsRayTouchesObjectZ(Vector3f hitBoxCenterPos, ObserverInfo observerInfo, Box hitBox)
    {
        Vector2f observerPos = observerInfo.position;

        double distance = Math.Sqrt(Math.Pow(hitBoxCenterPos.X - observerPos.X, 2) + Math.Pow(hitBoxCenterPos.Y - observerPos.Y, 2));
        distance /= Screen.Setting.Tile;

        double Up = (hitBox[CoordinatePlane.Z, SideSize.Larger]?.Side ?? 0);
        double Down = (hitBox[CoordinatePlane.Z, SideSize.Smaller]?.Side ?? 0);

        double angleEntity = -observerInfo.vertivalAngle;

        double rayHitUp = Up - GetMult(Up, multUp, distance) + angleEntity;
        double rayHitDown = Down - GetMult(Down, multDown, distance * 2) + angleEntity;
        Console.WriteLine(rayHitDown >= Down && rayHitUp <= Up);
        Console.WriteLine(rayHitDown);
        Console.WriteLine(Down);


        return rayHitDown >= Down && rayHitUp <= Up;
    }


    private static bool IsTouches(bool isCollidingX, bool isCollidingY, bool isCollidingZ)
    {
        if ((isCollidingX || isCollidingY) == true && isCollidingZ == true)
            return true;

        else
            return false;
    }
    public static bool IsRayTouchesObject(Vector3f hitBoxCenterPos, ObserverInfo observerInfo, Box box, double currentRayX, double currentRayY)
    {
        bool isCollidingX = IsRayTouchesObjectX(box, currentRayX);
        bool isCollidingY = IsRayTouchesObjectY(box, currentRayY);
        bool isCollidingZ = IsRayTouchesObjectZ(hitBoxCenterPos, observerInfo, box);

        return IsTouches(isCollidingX, isCollidingY, isCollidingZ);
    }
}
