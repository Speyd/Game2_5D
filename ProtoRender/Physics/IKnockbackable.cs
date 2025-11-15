using SFML.System;
namespace ProtoRender.Physics;
/// <summary>
/// Interface for objects that can be affected by a knockback effect.
/// </summary>
public interface IKnockbackable : IPhysicsObject
{
    /// <summary>
    /// Current velocity vector of the object, including knockback influence.
    /// </summary>
    Vector2f Velocity { get; set; }

    /// <summary>
    /// The power (magnitude) of the knockback, determining the initial velocity.
    /// </summary>
    float KnockbackPower { get; set; }

    /// <summary>
    /// The angle of the knockback, in degrees or radians depending on the implementation.
    /// </summary>
    float KnockbackAngle { get; set; }

    /// <summary>
    /// The velocity threshold below which the knockback effect is considered finished.
    /// </summary>
    float KnockbackVelocityEpsilon { get; set; }

    /// <summary>
    /// Applies the knockback effect to the object using the specified parameters.
    /// </summary>
    void Knockback();
}
