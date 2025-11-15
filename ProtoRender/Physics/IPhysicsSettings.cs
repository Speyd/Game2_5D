
namespace ProtoRender.Physics;
public interface IPhysicsSettings
{
    /// <summary>
    /// Determines if this object is affected by the global gravity setting.
    /// When false, gravity calculations will be skipped for this object.
    /// </summary>
    bool HasGravity { get; }

    /// <summary>
    /// Downward acceleration force applied to the object, in units per second squared.
    /// </summary>
    float Gravity { get; }

    /// <summary>
    /// Resistance applied to the object's movement, expressed as a friction coefficient.
    /// Higher values cause faster deceleration.
    /// </summary>
    float Friction { get; }
}
