using EntityLib;
using FpsLib;


namespace MoveLib;
public class MovePositions(Collision collision, MoveLib.Setting setting)
{
    double cosAngle = 1;
    double sinAngle = 1;

    public void Move(Entity entity, double deltaTime, double directionX, double directionY)
    {
        double speed = setting.MoveSpeed * (1 / FPS.fps);

        cosAngle = entity.Direction.X;
        sinAngle = entity.Direction.Y;

        double rx = cosAngle * directionX - sinAngle * directionY;
        double ry = sinAngle * directionX + cosAngle * directionY;

        rx *= speed;
        ry *= speed;

        collision.IsCollision(entity, rx, ry);
    }
}
