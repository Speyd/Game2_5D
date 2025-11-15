using SFML.Graphics;

namespace TextureLib.DataCache;
/// <summary>
/// Static class that manages cached loading of SFML textures by file path.
/// Avoids reloading the same texture multiple times from disk.
/// </summary>
public static class TextureDataCache
{
    private static readonly PathDataCache<SFML.Graphics.Texture> _cache = new();

    /// <summary>
    /// Retrieves a cached <see cref="Texture"/> by its file path.
    /// Returns <c>null</c> if the texture is not found in the cache or if the path is null.
    /// </summary>
    /// <param name="path">The file path of the texture.</param>
    /// <returns>The cached <see cref="Texture"/> if found; otherwise, <c>null</c>.</returns>
    public static IReadOnlyList<Texture>? Get(string? path) => _cache.Get(path);

    /// <summary>
    /// Gets the cached <see cref="Texture"/> associated with the specified path.
    /// If the texture is not present, creates it using the provided factory function,
    /// adds it to the cache, and returns it.
    /// </summary>
    /// <param name="path">The file path of the texture.</param>
    /// <param name="factory">A function to create the texture if it is not cached.</param>
    /// <returns>The cached or newly created <see cref="Texture"/>.</returns>
    public static IReadOnlyList<Texture> GetOrAdd(string path, Func<string, Texture> factory) =>
        _cache.GetOrAdd(path, factory);

    /// <summary>
    /// Gets the cached <see cref="Texture"/> associated with the specified path.
    /// If the texture is not present, creates it using the provided factory function,
    /// adds it to the cache, and returns it.
    /// </summary>
    /// <param name="path">The file path of the texture.</param>
    /// <param name="factory">A function to create the texture if it is not cached.</param>
    /// <returns>The cached or newly created <see cref="Texture"/>.</returns>
    public static IReadOnlyList<Texture> GetOrAdd(string path, Func<string, IEnumerable<Texture>> factory) =>
        _cache.GetOrAdd(path, factory);

    /// <summary>
    /// Loads and adds the specified <see cref="Texture"/> into the cache under the given path.
    /// </summary>
    /// <param name="path">The file path key for the texture.</param>
    /// <param name="texture">The <see cref="Texture"/> instance to cache.</param>
    public static void Load(string path, Texture texture) => _cache.Load(path, texture);
    /// <summary>
    /// Loads and adds the specified <see cref="Texture"/> into the cache under the given path.
    /// </summary>
    /// <param name="path">The file path key for the texture.</param>
    /// <param name="textures">The <see cref="Texture"/> instance to cache.</param>
    public static void Load(string path, IEnumerable<Texture> textures) => _cache.Load(path, textures);

    /// <summary>
    /// Appends a single <see cref="Texture"/> to the cache entry associated with the specified path.
    /// In contrast to <c>Load</c>, this method adds the item to the existing entry instead of replacing it.
    /// Throws <see cref="FileNotFoundException"/> if the file or directory at the path does not exist.
    /// </summary>
    /// <param name="path">The file or directory path used as the cache key.</param>
    /// <param name="texture">The <see cref="Texture"/> to append to the cache entry.</param>
    public static void Append(string path, Texture texture) => _cache.Append(path, texture);

    /// <summary>
    /// Appends a collection of <see cref="Texture"/> objects to the cache entry associated with the specified path.
    /// In contrast to <c>Load</c>, this method adds the items to the existing entry instead of replacing it.
    /// Throws <see cref="FileNotFoundException"/> if the file or directory at the path does not exist.
    /// </summary>
    /// <param name="path">The file or directory path used as the cache key.</param>
    /// <param name="textures">The collection of <see cref="Texture"/> objects to append to the cache entry.</param>
    public static void Append(string path, IEnumerable<Texture> textures) => _cache.Append(path, textures);


    public static bool ContainsKey(string path) => _cache.ContainsKey(path);

    /// <summary>
    /// Clears all cached textures.
    /// </summary>
    public static void Clear() => _cache.Clear();
}

