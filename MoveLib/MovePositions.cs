using EntityLib;
using FpsLib;

namespace MoveLib
{
    public class MovePositions(Collision collision, MoveLib.Setting setting)
    {
        public void Move(Entity entity, double directionX, double directionY, double deltaTime)
        {
            double speed = setting.moveSpeed * (1 / FPS.fps);

            double rx = Math.Cos(entity.GetEntityA()) * directionX - Math.Sin(entity.GetEntityA()) * directionY;
            double ry = Math.Sin(entity.GetEntityA()) * directionX + Math.Cos(entity.GetEntityA()) * directionY;

            rx *= speed;
            ry *= speed;

            collision.IsCollision(rx, ry, entity);
        }
    }
}
