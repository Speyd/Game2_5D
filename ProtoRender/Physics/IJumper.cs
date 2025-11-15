namespace ProtoRender.Physics;
/// <summary>
/// Represents a physics-enabled object capable of performing jumps.
/// Provides properties to control jump height, duration, force, and apex timing,
/// as well as information about the object's position relative to the ground.
/// Implements the IPhysicsObject interface for integration with a physics system.
/// </summary>
public interface IJumper : IPhysicsObject
{
    /// <summary>
    /// The elapsed time since the start of the current jump, in seconds.
    /// </summary>
    float JumpElapsed { get; set; }

    /// <summary>
    /// The total duration of a jump, in seconds.
    /// </summary>
    float JumpDuration { get; set; }

    /// <summary>
    /// The maximum height the object will reach during a jump.
    /// </summary>
    float JumpHeight { get; set; }

    /// <summary>
    /// The current upward force being applied to the object during a jump.
    /// </summary>
    float CurrentJumpForce { get; set; }

    /// <summary>
    /// The object's height at the start of the jump.
    /// </summary>
    float InitialJumpHeight { get; set; }

    /// <summary>
    /// The minimum distance (in units) above the surface at which the object
    /// is positioned after landing or when standing on the ground.
    /// Default value is 1.
    /// </summary>
    float GroundClearance { get; set; }

    /// <summary>
    /// The relative time (from 0 to 1) at which the jump reaches its apex.
    /// A value of 0.5 indicates the peak occurs at the midpoint of the jump duration.
    /// Adjust this to make the apex occur earlier or later.
    /// </summary>
    float JumpApexTime { get; set; }

    /// <summary>
    /// The absolute Z-coordinate of the ground surface where the object stands.
    /// </summary>
    float GroundLevel { get; set; }

    /// <summary>
    /// Initiates a jump for the object using the configured jump parameters.
    /// </summary>
    void Jump();
}
