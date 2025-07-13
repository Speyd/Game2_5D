using SFML.Graphics;


namespace ProtoRender.WindowInterface;
/// <summary>
/// Provides a shared cache of loaded fonts to avoid redundant loading from disk.
/// Ensures each font is loaded only once and reused throughout the application.
/// </summary>
public static class FontCache
{
    private static readonly Dictionary<string, Font> _fonts = new();
    /// <summary>
    /// Retrieves a <see cref="Font"/> object from the cache based on the specified file path.
    /// If the font is not already cached, it is loaded from disk and added to the cache.
    /// </summary>
    /// <param name="path">The file path to the font resource.</param>
    /// <returns>The loaded <see cref="Font"/> object.</returns>
    /// <exception cref="Exception">Thrown if the font file cannot be found at the specified path.</exception>
    public static Font GetFont(string path)
    {
        if (!_fonts.TryGetValue(path, out var font))
        {
            if (!File.Exists(path))
                throw new Exception("Error load text Font");

            font = new Font(path);
            _fonts[path] = font;
        }

        return font;
    }
    /// <summary>
    /// Clears all cached fonts, releasing associated resources.
    /// This can be useful when unloading fonts or reloading assets.
    /// </summary>
    public static void Clear()
    {
        _fonts.Clear();
    }
}

