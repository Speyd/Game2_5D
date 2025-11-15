using FpsLib;
using ProtoRender.Object;
using ProtoRender.Physics;
using SFML.System;


namespace MoveLib.Physics;
public class KnockbackStrategy : IPhysicsUpdateStrategy
{
    public void Update(IObject obj)
    {
        if (obj is not IKnockbackable knockbackable)
            return;
        else if (knockbackable.ControlState is not ControlState.Knockback)
            return;

        float deltaTime = (float)FPS.GetDeltaTime();

        float deltaX = knockbackable.Velocity.X * deltaTime;
        float deltaY = knockbackable.Velocity.Y * deltaTime;

        List<IObject> ignoreList = obj is IMovable movable ? movable.IgnoreCollisionObjects.Keys.ToList() : new List<IObject>();
        var collided = MoveLib.Move.Collision.GetCollision(obj, deltaX, deltaY, ignoreList);


        if (collided.CollisionObject is not null && collided.CollisionCoordinate.HasValue)
        {
            MoveLib.Move.Collision.ResolvePositionCollision(obj, collided.CollisionObject, collided.CollisionCoordinate.Value, deltaX, deltaY);

            knockbackable.Velocity = new Vector2f(0, 0);
            knockbackable.ControlState = ControlState.Normal;
        }
        else
        {
            obj.X.Axis += deltaX;
            obj.Y.Axis += deltaY;

            knockbackable.Velocity *= knockbackable.Friction;

            float velocityLength = MathF.Sqrt(knockbackable.Velocity.X * knockbackable.Velocity.X + knockbackable.Velocity.Y * knockbackable.Velocity.Y);
            if (velocityLength < knockbackable.KnockbackVelocityEpsilon)
            {
                knockbackable.Velocity = new Vector2f(0, 0);
                knockbackable.ControlState = ControlState.Normal;
            }
        }
    }
}