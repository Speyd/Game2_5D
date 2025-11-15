
namespace AnimationLib.Enum;
/// <summary>
/// Specifies how an animation should play its frames.
/// </summary>
public enum PlayMode
{
    /// <summary>
    /// The animation repeats continuously from start to end.
    /// </summary>
    Loop,

    /// <summary>
    /// The animation plays once from start to end and then stops.
    /// </summary>
    Once,

    /// <summary>
    /// The animation is paused and does not advance frames.
    /// </summary>
    Pause,
}
