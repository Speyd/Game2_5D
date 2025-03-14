using EntityLib;
using FpsLib;
using MapLib;

namespace MoveLib.Move;
public static class MovePositions
{
    public static void Move(Map map, Entity entity, double directionX, double directionY)
    {

        double speed = MoveLib.Setting.MoveSpeed * (1 / FPS.Fps);

        double cosAngle = entity.Direction.X;
        double sinAngle = entity.Direction.Y;

        double rx = cosAngle * directionX - sinAngle * directionY;
        double ry = sinAngle * directionX + cosAngle * directionY;

        rx *= speed;
        ry *= speed;

        Collision.IsCollision(map, entity, rx, ry);
    }
}
