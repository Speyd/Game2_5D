using FpsLib;
using HitBoxLib.PositionObject;
using HitBoxLib.Segment.SignsTypeSide;
using ProtoRender.Object;
using ProtoRender.Physics;
using MoveLib.Move;

namespace MoveLib.Physics;
/// <summary>
/// Implements a physics update strategy for objects that can jump.
/// Handles jumping, falling, and ground checks while considering collisions
/// with ground and ceiling surfaces.
/// </summary>
public class JumpStrategy : IPhysicsUpdateStrategy
{
    /// <summary>
    /// Updates the vertical position of the given object based on its jump state.
    /// </summary>
    /// <param name="obj">The object to update. Must implement <see cref="IJumper"/> and have gravity enabled.</param>
    public void Update(IObject obj)
    {
        if(obj is not IJumper jumper || !jumper.HasGravity)
            return;

        float deltaTime = (float)FPS.GetDeltaTime();

        var ignoreList = obj is IMovable movable ? movable.IgnoreCollisionObjects.Keys.ToList() : new();
        float surfaceZ = (float)MoveLib.Move.Collision.FindSurfaceHeight(SurfaceCheckMode.Ground, obj, obj.HitBox.MainHitBox, ignoreList);

        switch (jumper.GroundState)
        {
            case GroundState.Jumping:
                HandleJumping(jumper, obj, deltaTime, ignoreList);
                break;

            case GroundState.Falling:
                HandleFalling(jumper, obj, deltaTime, surfaceZ);
                break;

            default:
                HandleGroundCheck(jumper, obj, surfaceZ);
                break;
        }
    }

    /// <summary>
    /// Handles the object's behavior while it is in the jumping state.
    /// Calculates the jump trajectory and checks for collisions with the ceiling.
    /// </summary>
    /// <param name="jumper">The object implementing <see cref="IJumper"/>.</param>
    /// <param name="obj">The object being updated.</param>
    /// <param name="deltaTime">The elapsed time since the last update, in seconds.</param>
    /// <param name="ignoreList">A list of objects to ignore during collision checks.</param>
    private void HandleJumping(IJumper jumper, IObject obj, double deltaTime, List<IObject> ignoreList)
    {
        jumper.JumpElapsed += (float)deltaTime;

        float t = jumper.JumpElapsed / jumper.JumpDuration;
        if (t > 1f) t = 1f;

        var jumpZ = jumper.InitialJumpHeight + jumper.JumpHeight * 4 * t * (1 - t);
        var newZ = MoveLib.Move.Collision.FindSurfaceHeight(SurfaceCheckMode.Ceiling, obj, obj.HitBox.MainHitBox, jumpZ, ignoreList);

        if (newZ != double.MaxValue)
        {
            jumper.GroundState = GroundState.Falling;
            return;
        }
        else
        {
            obj.Z.Axis = jumpZ;
        }
   
        if (t >= jumper.JumpApexTime)
        {
            jumper.GroundState = GroundState.Falling;
        }
    }

    /// <summary>
    /// Handles the object's behavior while it is falling.
    /// Applies gravity, updates the vertical position, and detects collision with the ground.
    /// </summary>
    /// <param name="jumper">The object implementing <see cref="IJumper"/>.</param>
    /// <param name="obj">The object being updated.</param>
    /// <param name="deltaTime">The elapsed time since the last update, in seconds.</param>
    /// <param name="surfaceZ">The Z coordinate of the nearest ground surface under the object.</param>
    private void HandleFalling(IJumper jumper, IObject obj, double deltaTime, double surfaceZ)
    {
        if (surfaceZ > jumper.GroundLevel)
            surfaceZ += GetBottomOffset(jumper, obj);
        else
            surfaceZ = jumper.GroundLevel;

        jumper.CurrentJumpForce -= (float)(jumper.Gravity * deltaTime);
        var newZ = obj.Z.Axis + jumper.CurrentJumpForce * deltaTime;

        if (newZ <= surfaceZ)
        {
            newZ = surfaceZ;
            jumper.GroundState = GroundState.OnGround;
            jumper.CurrentJumpForce = 0;
        }


        obj.Z.Axis = newZ;
    }

    /// <summary>
    /// Checks whether the object is standing on the ground or should start falling.
    /// Updates the vertical position accordingly.
    /// </summary>
    /// <param name="jumper">The object implementing <see cref="IJumper"/>.</param>
    /// <param name="obj">The object being updated.</param>
    /// <param name="surfaceZ">The Z coordinate of the nearest ground surface under the object.</param>
    private void HandleGroundCheck(IJumper jumper, IObject obj, double surfaceZ)
    {
        if (surfaceZ > jumper.GroundLevel)
            surfaceZ += GetBottomOffset(jumper, obj);
        else
            surfaceZ = jumper.GroundLevel;


        if (obj.Z.Axis > surfaceZ)
        {
            jumper.GroundState = GroundState.Falling;
            jumper.CurrentJumpForce = 0;
        }
        else
        {
            obj.Z.Axis = surfaceZ;
            jumper.GroundState = GroundState.OnGround;
            jumper.CurrentJumpForce = 0;
        }
    }

    /// <summary>
    /// Calculates the bottom offset of the object to maintain clearance above the ground.
    /// </summary>
    /// <param name="jumper">The object implementing <see cref="IJumper"/>.</param>
    /// <param name="obj">The object being updated.</param>
    /// <returns>The offset to apply to the ground Z coordinate.</returns>
    private double GetBottomOffset(IJumper jumper, IObject obj)
    {
        return (obj.HitBox[CoordinatePlane.Z, SideSize.Smaller]?.OriginalOffset ?? 0) + jumper.GroundClearance;
    }
}