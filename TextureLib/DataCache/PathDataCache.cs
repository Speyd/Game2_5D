using System.Collections.Concurrent;
using System.Collections.Generic;


namespace TextureLib.DataCache;
/// <summary>
/// Thread-safe cache that stores and retrieves lists of type <typeparamref name="T"/> by string keys (usually file or directory paths).
/// Supports retrieval by exact key or by matching parent directory paths.
/// </summary>
/// <typeparam name="T">The type of data stored in the cache.</typeparam>
public class PathDataCache<T> : IDataCache<string, T>
{
    private readonly ConcurrentDictionary<string, List<T>> _cache = new();

    /// <summary>
    /// Retrieves cached items by key. Returns null if the key is null, empty, or not found.
    /// Also supports prefix matching for parent directory paths.
    /// </summary>
    /// <param name="path">The key or path to retrieve cached data for.</param>
    /// <returns>A read-only list of cached items, or null if none found.</returns>
    public IReadOnlyList<T>? Get(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return default;

        if (_cache.TryGetValue(path, out var value))
            return value;

        string fullPath = Path.GetFullPath(path);

        foreach (var kvp in _cache)
        {
            string cachedPath = Path.GetFullPath(kvp.Key);
            if (fullPath.StartsWith(cachedPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                return kvp.Value;
        }

        return default;
    }
    /// <summary>
    /// Gets cached data by key or creates and caches a single item using the provided factory if missing.
    /// </summary>
    /// <param name="path">The key to retrieve or add.</param>
    /// <param name="factory">A function that creates a single cached item for the key.</param>
    /// <returns>A read-only list containing the cached or newly created item.</returns>
    public IReadOnlyList<T> GetOrAdd(string path, Func<string, T> factory)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException("Path cannot be null or empty", nameof(path));

        return _cache.GetOrAdd(path, p =>
        {
            if (!File.Exists(p) && !Directory.Exists(p))
                throw new FileNotFoundException($"File not found: {p}");

            return new List<T> { factory(p) };
        });
    }
    /// <summary>
    /// Gets cached data by key or creates and caches multiple items using the provided factory if missing.
    /// </summary>
    /// <param name="path">The key to retrieve or add.</param>
    /// <param name="factory">A function that creates a collection of cached items for the key.</param>
    /// <returns>A read-only list containing the cached or newly created items.</returns>
    public IReadOnlyList<T> GetOrAdd(string path, Func<string, IEnumerable<T>> factory)
    {
        if (string.IsNullOrEmpty(path))
            return new List<T>();

        return _cache.GetOrAdd(path, p =>
        {
            if (!File.Exists(p) && !Directory.Exists(p))
                throw new FileNotFoundException($"File not found: {p}");

            return factory(p).ToList();
        });
    }
    /// <summary>
    /// Loads a single item into the cache for the specified key, overwriting any existing data.
    /// </summary>
    /// <param name="path">The key under which to store the data.</param>
    /// <param name="data">The item to cache.</param>
    public void Load(string path, T data)
    {
        if (!File.Exists(path) && !Directory.Exists(path))
            throw new FileNotFoundException($"File not found: {path}");

        _cache[path] = new List<T>() { data };
    }
    /// <summary>
    /// Loads multiple items into the cache for the specified key, overwriting any existing data.
    /// </summary>
    /// <param name="path">The key under which to store the data.</param>
    /// <param name="data">The collection of items to cache.</param>
    public void Load(string path, IEnumerable<T> data)
    {
        if (!File.Exists(path) && !Directory.Exists(path))
            throw new FileNotFoundException($"File not found: {path}");

        _cache[path] = data.ToList();
    }
    /// <summary>
    /// Determines whether the cache contains an entry for the specified key.
    /// </summary>
    /// <param name="path">The key to check for existence.</param>
    /// <returns>True if the key exists in the cache; otherwise, false.</returns>
    public bool ContainsKey(string path)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        return _cache.ContainsKey(path);
    }
    /// <summary>
    /// Removes all entries from the cache.
    /// </summary>
    public void Clear() => _cache.Clear();
}
