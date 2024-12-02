using EntityLib;
using FpsLib;

namespace MoveLib
{
    public class MovePositions(Collision collision, MoveLib.Setting setting)
    {
        public void Move(Entity entity, double directionX, double directionY, double deltaTime)
        {
            double speed = setting.moveSpeed * (1 / FPS.fps);

            double rx = Math.Cos(entity.Angle) * directionX - Math.Sin(entity.Angle) * directionY;
            double ry = Math.Sin(entity.Angle) * directionX + Math.Cos(entity.Angle) * directionY;

            rx *= speed;
            ry *= speed;

            collision.IsCollision(rx, ry, entity);
        }
    }
}
