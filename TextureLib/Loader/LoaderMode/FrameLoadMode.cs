namespace TextureLib.Loader.LoaderMode;
/// <summary>
/// Specifies options for how image frames are loaded and composed.
/// </summary>
[Flags]
public enum FrameLoadMode
{
    /// <summary>
    /// No specific frame loading mode is applied.
    /// </summary>
    None = 0,

    /// <summary>
    /// Load each frame as a full independent image without accumulation.
    /// </summary>
    FullFrame = 1 << 0,

    /// <summary>
    /// Load frames by accumulating previous frames (compositing frames over each other).
    /// </summary>
    Accumulate = 1 << 1,
}
