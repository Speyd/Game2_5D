
namespace TextureLib.Loader.LoaderMode;
/// <summary>
/// Specifies color channels for filtering operations.
/// Can be combined using bitwise operations to filter multiple channels simultaneously.
/// </summary>
[Flags]
public enum ColorChannelFilter
{
    /// <summary>No color channels selected; no filtering applied.</summary>
    None = 0,

    /// <summary>Red channel.</summary>
    R = 1 << 0,

    /// <summary>Green channel.</summary>
    G = 1 << 1,

    /// <summary>Blue channel.</summary>
    B = 1 << 2,

    /// <summary>Alpha (transparency) channel.</summary>
    A = 1 << 3,

    /// <summary>Combination of red, green, and blue channels.</summary>
    RGB = R | G | B,

    /// <summary>Combination of red, green, blue, and alpha channels.</summary>
    RGBA = R | G | B | A,
}