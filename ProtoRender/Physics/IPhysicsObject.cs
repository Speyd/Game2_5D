
namespace ProtoRender.Physics;
/// <summary>
/// Represents the basic physics behavior for an object, including control and ground states.
/// Provides the ability to update the object's physics each frame.
/// Inherits physics settings from IPhysicsSettings.
/// </summary>
public interface IPhysicsObject : IPhysicsSettings
{
    /// <summary>
    /// The current control state of the object, indicating how it is being manipulated.
    /// </summary>
    ControlState ControlState { get; set; }

    /// <summary>
    /// The current ground state of the object, indicating whether it is grounded, airborne, or sliding.
    /// </summary>
    GroundState GroundState { get; set; }

    /// <summary>
    /// Updates the physics simulation for the object.
    /// Should be called each frame to apply gravity, friction, and other forces.
    /// </summary>
    void UpdatePhysics();
}