
namespace TextureLib.DataCache;
/// <summary>
/// Interface for a data cache that stores and retrieves lists of type <typeparamref name="TValue"/> by keys of type <typeparamref name="TKey"/>.
/// Provides methods to get, add, load, check existence, and clear cached data.
/// </summary>
/// <typeparam name="TKey">The type of the cache key.</typeparam>
/// <typeparam name="TValue">The type of data to be cached.</typeparam>
public interface IDataCache<TKey, TValue>
{
    /// <summary>
    /// Retrieves a read-only list of cached items by the given key.
    /// Returns null if the key does not exist.
    /// </summary>
    /// <param name="key">The key to look up data.</param>
    /// <returns>Read-only list of cached items or null if not found.</returns>
    IReadOnlyList<TValue>? Get(TKey? key);

    /// <summary>
    /// Retrieves cached data by key or adds it if missing using a factory function
    /// that returns a single item of type <typeparamref name="TValue"/>.
    /// </summary>
    /// <param name="key">The key to retrieve or create data for.</param>
    /// <param name="factory">A function that creates a <typeparamref name="TValue"/> item given the key.</param>
    /// <returns>A read-only list containing the created or cached item.</returns>
    IReadOnlyList<TValue> GetOrAdd(TKey key, Func<TKey, TValue> factory);

    /// <summary>
    /// Retrieves cached data by key or adds it if missing using a factory function
    /// that returns an enumerable collection of items of type <typeparamref name="TValue"/>.
    /// </summary>
    /// <param name="key">The key to retrieve or create data for.</param>
    /// <param name="factory">A function that creates an enumerable of <typeparamref name="TValue"/> items given the key.</param>
    /// <returns>A read-only list containing the created or cached items.</returns>
    IReadOnlyList<TValue> GetOrAdd(TKey key, Func<TKey, IEnumerable<TValue>> factory);

    /// <summary>
    /// Loads a single item into the cache under the specified key.
    /// </summary>
    /// <param name="key">The key to store the data.</param>
    /// <param name="data">The item to cache.</param>
    void Load(TKey key, TValue data);

    /// <summary>
    /// Loads a collection of items into the cache under the specified key.
    /// </summary>
    /// <param name="key">The key to store the data.</param>
    /// <param name="data">The collection of items to cache.</param>
    void Load(TKey key, IEnumerable<TValue> data);

    /// <summary>
    /// Appends a single item to the existing cache entry under the specified key.
    /// If the key does not exist, a new entry is created.
    /// </summary>
    /// <param name="key">The key under which the data is stored.</param>
    /// <param name="data">The item to append to the cache.</param>
    void Append(TKey key, TValue data);

    /// <summary>
    /// Appends a collection of items to the existing cache entry under the specified key.
    /// If the key does not exist, a new entry is created.
    /// </summary>
    /// <param name="key">The key under which the data is stored.</param>
    /// <param name="data">The collection of items to append to the cache.</param>
    void Append(TKey key, IEnumerable<TValue> data);


    /// <summary>
    /// Checks if the cache contains data for the specified key.
    /// </summary>
    /// <param name="key">The key to check for.</param>
    /// <returns>True if data exists in the cache; otherwise, false.</returns>
    bool ContainsKey(TKey key);

    /// <summary>
    /// Clears all cached data.
    /// </summary>
    void Clear();
}