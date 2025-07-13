using TextureLib.Loader.LoaderMode;

namespace TextureLib.Loader.ImageProcessing;
/// <summary>
/// Represents advanced options for controlling how images are loaded by <see cref="ImageLoader"/>.
/// Allows specifying behaviors such as:
/// - Frame loading mode (e.g., single frame, accumulated frames),
/// - Filtering or replacing specific color channels,
/// - Customizing color space handling and compositing,
/// - Other fine-grained parameters influencing the texture loading process.
///
/// This class is optional — if not provided, default loading behavior is used.
/// </summary>
public class ImageLoadOptions
{
    /// <summary>
    /// Flags specifying which color channels to filter during processing.
    /// </summary>
    public ColorChannelFilter ColorChannelFilter { get; set; } = ColorChannelFilter.None;

    /// <summary>
    /// Indicates whether the color processing has already been applied to avoid redundant operations.
    /// </summary>
    public bool ColorsAlreadyProcessed { get; set; } = false;

    /// <summary>
    /// Mode specifying how color replacement should be applied (per channel or all channels).
    /// </summary>
    public ColorReplaceMode ColorReplaceMode { get; set; } = ColorReplaceMode.AllChannels;
    /// <summary>
    /// Mode specifying how frames are loaded (full frame, accumulate, or none).
    /// </summary>
    public FrameLoadMode FrameLoadMode { get; set; } = FrameLoadMode.Accumulate;

    /// <summary>
    /// Start color range for filtering pixels during color replacement.
    /// </summary>
    public SFML.Graphics.Color StartFilterRGBA { get; set; } = new SFML.Graphics.Color(0, 0, 0, 0);
    /// <summary>
    /// End color range for filtering pixels during color replacement.
    /// </summary>
    public SFML.Graphics.Color EndFilterRGBA { get; set; } = new SFML.Graphics.Color(0, 0, 0, 0);
    /// <summary>
    /// Base color used for replacing filtered pixel colors.
    /// </summary>
    public SFML.Graphics.Color BaseReplaceColor { get; set; } = new SFML.Graphics.Color(0, 0, 0, 0);


    /// <summary>
    /// String representing which color channels to load (e.g., "RGBA").
    /// </summary>
    public string LoadSettingMapping { get; set; } = "RGBA";
}