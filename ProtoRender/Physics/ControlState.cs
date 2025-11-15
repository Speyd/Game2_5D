
namespace ProtoRender.Physics;
/// <summary>
/// Represents the possible control states of a player character.
/// </summary>
public enum ControlState
{
    /// <summary>
    /// Normal state — the player has full control.
    /// </summary>
    Normal,

    /// <summary>
    /// Knockback state — the player is pushed back by an external force.
    /// </summary>
    Knockback,

    /// <summary>
    /// Stunned state — the player is unable to act for a duration.
    /// </summary>
    Stunned
}