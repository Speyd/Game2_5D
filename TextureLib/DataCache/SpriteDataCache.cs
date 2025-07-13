using SFML.Graphics;

namespace TextureLib.DataCache;
/// <summary>
/// Static class that manages cached loading of SFML textures by file path.
/// Avoids reloading the same texture multiple times from disk.
/// </summary>
public static class SpriteDataCache
{
    private static readonly PathDataCache<SFML.Graphics.Sprite> _cache = new();

    /// <summary>
    /// Retrieves a cached <see cref="Texture"/> by its file path.
    /// Returns <c>null</c> if the texture is not found in the cache or if the path is null.
    /// </summary>
    /// <param name="path">The file path of the texture.</param>
    /// <returns>The cached <see cref="Texture"/> if found; otherwise, <c>null</c>.</returns>
    public static IReadOnlyList<Sprite>? Get(string? path) => _cache.Get(path);

    /// <summary>
    /// Gets the cached <see cref="Texture"/> associated with the specified path.
    /// If the texture is not present, creates it using the provided factory function,
    /// adds it to the cache, and returns it.
    /// </summary>
    /// <param name="path">The file path of the texture.</param>
    /// <param name="factory">A function to create the texture if it is not cached.</param>
    /// <returns>The cached or newly created <see cref="Texture"/>.</returns>
    public static IReadOnlyList<Sprite> GetOrAdd(string path, Func<string, Sprite> factory) =>
        _cache.GetOrAdd(path, factory);

    /// <summary>
    /// Gets the cached <see cref="Texture"/> associated with the specified path.
    /// If the texture is not present, creates it using the provided factory function,
    /// adds it to the cache, and returns it.
    /// </summary>
    /// <param name="path">The file path of the texture.</param>
    /// <param name="factory">A function to create the texture if it is not cached.</param>
    /// <returns>The cached or newly created <see cref="Texture"/>.</returns>
    public static IReadOnlyList<Sprite> GetOrAdd(string path, Func<string, IEnumerable<Sprite>> factory) =>
        _cache.GetOrAdd(path, factory);

    /// <summary>
    /// Loads and adds the specified <see cref="Texture"/> into the cache under the given path.
    /// </summary>
    /// <param name="path">The file path key for the texture.</param>
    /// <param name="texture">The <see cref="Texture"/> instance to cache.</param>
    public static void Load(string path, Sprite texture) => _cache.Load(path, texture);
    /// <summary>
    /// Loads and adds the specified collection of <see cref="Sprite"/> instances into the cache under the given path.
    /// Replaces any existing sprites cached for the same path.
    /// </summary>
    /// <param name="path">The file path key for the sprites.</param>
    /// <param name="texture">The collection of <see cref="Sprite"/> instances to cache.</param>
    public static void Load(string path, IEnumerable<Sprite> texture) => _cache.Load(path, texture);

    /// <summary>
    /// Determines whether the cache contains any sprites associated with the specified path.
    /// </summary>
    /// <param name="path">The file path key to check in the cache.</param>
    /// <returns><c>true</c> if the cache contains sprites for the given path; otherwise, <c>false</c>.</returns>
    public static bool ContainsKey(string path) => _cache.ContainsKey(path);

    /// <summary>
    /// Clears all cached textures.
    /// </summary>
    public static void Clear() => _cache.Clear();
}

