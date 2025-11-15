
namespace AnimationLib.Enum;
/// <summary>
/// Specifies which texture rectangle should be used when rendering a frame.
/// </summary>
public enum FrameRectMode
{
    /// <summary>
    /// Always use the maximum bounding rectangle among all animation frames.
    /// </summary>
    UseMaxFrameRect,

    /// <summary>
    /// Use the rectangle of the current animation frame.
    /// </summary>
    UseCurrentFrameRect
}

