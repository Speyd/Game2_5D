
namespace ProtoRender.Physics;
/// <summary>
/// Represents the different movement states of a character relative to the ground.
/// </summary>
public enum GroundState
{
    /// <summary>
    /// The character is standing on solid ground.
    /// </summary>
    OnGround,

    /// <summary>
    /// The character is moving upward after a jump.
    /// </summary>
    Jumping,

    /// <summary>
    /// The character is descending in the air (falling).
    /// </summary>
    Falling
}

