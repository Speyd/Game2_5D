using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataPipes.DTO.Converters;
/// <summary>
/// Provides static methods for deserializing data from various formats (e.g., JSON) into domain objects via DTOs.
/// </summary>
/// <remarks>
/// Currently supports JSON deserialization using System.Text.Json.
/// Future extensions may include support for XML, binary, or custom formats.
/// </remarks>
public static class Deserializer
{
    /// <summary>
    /// Deserializes a JSON file into an instance of the target object type using the specified DTO type.
    /// </summary>
    /// <typeparam name="TDto">The DTO type that implements <see cref="IDTO{TObject}"/>.</typeparam>
    /// <typeparam name="TObject">The type of the object to deserialize to.</typeparam>
    /// <param name="path">The path to the JSON file.</param>
    /// <param name="options">Optional JSON serialization options.</param>
    /// <returns>An instance of <typeparamref name="TObject"/> created from the deserialized DTO.</returns>
    /// <exception cref="Exception">Thrown if the file does not exist or deserialization fails.</exception>
    public static TObject FromJson<TDto, TObject>(string path, JsonSerializerOptions? options = null)
        where TDto : class, IDTO<TObject>
    {
        if (!File.Exists(path))
            throw new Exception("Error JSON path!");

        options = options ?? new JsonSerializerOptions
        {
            IncludeFields = true,
        };
        TDto.RegisterDTO();
        TDto.RegisterDTOJsonConverters(options);

        string json = File.ReadAllText(path);
        TDto? dto = JsonSerializer.Deserialize<TDto>(json, options);

        if (dto is null)
            throw new Exception("DTO is null(Deserializer - DeserializationJSON)");

        return dto.ToObject();
    }

    /// <summary>
    /// Deserializes a JSON file into an object of type <typeparamref name="TResult"/>, using a DTO as an intermediate step.
    /// </summary>
    /// <typeparam name="TDto">The DTO type that implements <see cref="IDTO{TObject}"/>.</typeparam>
    /// <typeparam name="TObject">The intermediate object type that the DTO maps to.</typeparam>
    /// <typeparam name="TResult">The final type to which the result will be cast.</typeparam>
    /// <param name="path">The path to the JSON file.</param>
    /// <param name="options">Optional JSON serialization options.</param>
    /// <returns>The deserialized object cast to <typeparamref name="TResult"/>, or <c>null</c> if the cast fails.</returns>
    /// <exception cref="Exception">Thrown if the file does not exist or deserialization fails.</exception>
    public static TResult? FromJson<TDto, TObject, TResult>(string path, JsonSerializerOptions? options = null)
        where TDto : class, IDTO<TObject>
    {
        var result = FromJson<TDto, TObject>(path, options);
        return result is TResult ReturnObject ? ReturnObject : default;
    }

    /// <summary>
    /// Deserializes a JSON file into a DTO and updates the fields of an existing target object instance.
    /// </summary>
    /// <typeparam name="TDto">The DTO type that implements <see cref="IDTO{TObject}"/>.</typeparam>
    /// <typeparam name="TObject">The type of the target object to update.</typeparam>
    /// <param name="target">The existing object instance to update with values from the deserialized DTO.</param>
    /// <param name="path">The path to the JSON file.</param>
    /// <param name="options">Optional JSON serialization options.</param>
    /// <exception cref="Exception">Thrown if the file does not exist or deserialization fails.</exception>
    public static void FromJson<TDto, TObject>(TObject target, string path, JsonSerializerOptions? options = null)
        where TDto : IDTO<TObject>
    {
        if (!File.Exists(path))
            throw new Exception("Error JSON path!");

        options = options ?? new JsonSerializerOptions
        {
            IncludeFields = true,
        };
        TDto.RegisterDTO();
        TDto.RegisterDTOJsonConverters(options);

        string json = File.ReadAllText(path);
        TDto? dto = JsonSerializer.Deserialize<TDto>(json, options);

        if (dto is null)
            throw new Exception("DTO is null(Deserializer - DeserializationJSON)");

        dto.ToObject(target);
    }

    /// <summary>
    /// Asynchronously deserializes a JSON file into an instance of the target object type using the specified DTO type.
    /// </summary>
    /// <typeparam name="TDto">The DTO type that implements <see cref="IDTO{TObject}"/>.</typeparam>
    /// <typeparam name="TObject">The type of the object to deserialize to.</typeparam>
    /// <param name="path">The path to the JSON file.</param>
    /// <param name="options">Optional JSON serialization options.</param>
    /// <returns>A task that returns an instance of <typeparamref name="TObject"/> created from the deserialized DTO.</returns>
    /// <exception cref="Exception">Thrown if the file does not exist or deserialization fails.</exception>
    public static async Task<TObject> FromJsonAsync<TDto, TObject>(string path, JsonSerializerOptions? options = null)
        where TDto : class, IDTO<TObject>
    {
        if (!File.Exists(path))
            throw new Exception("Error JSON path!");

        options ??= new JsonSerializerOptions
        {
            IncludeFields = true,
        };

        TDto.RegisterDTO();
        TDto.RegisterDTOJsonConverters(options);

        await using FileStream stream = File.OpenRead(path);
        TDto? dto = await JsonSerializer.DeserializeAsync<TDto>(stream, options);

        if (dto is null)
            throw new Exception("DTO is null (FromJSONAsync)");

        return dto.ToObject();
    }

    /// <summary>
    /// Asynchronously deserializes a JSON file into an object of type <typeparamref name="TResult"/>, using a DTO as an intermediate step.
    /// </summary>
    /// <typeparam name="TDto">The DTO type that implements <see cref="IDTO{TObject}"/>.</typeparam>
    /// <typeparam name="TObject">The intermediate object type that the DTO maps to.</typeparam>
    /// <typeparam name="TResult">The final type to which the result will be cast.</typeparam>
    /// <param name="path">The path to the JSON file.</param>
    /// <param name="options">Optional JSON serialization options.</param>
    /// <returns>A task that returns the deserialized object cast to <typeparamref name="TResult"/>, or <c>null</c> if the cast fails.</returns>
    /// <exception cref="Exception">Thrown if the file does not exist or deserialization fails.</exception>
    public static async Task<TResult?> FromJsonAsync<TDto, TObject, TResult>(string path, JsonSerializerOptions? options = null)
        where TDto : class, IDTO<TObject>
    {
        TObject result = await FromJsonAsync<TDto, TObject>(path, options);
        return result is TResult casted ? casted : default;
    }

    /// <summary>
    /// Asynchronously deserializes a JSON file into a DTO and updates the fields of an existing target object instance.
    /// </summary>
    /// <typeparam name="TDto">The DTO type that implements <see cref="IDTO{TObject}"/>.</typeparam>
    /// <typeparam name="TObject">The type of the target object to update.</typeparam>
    /// <param name="target">The existing object instance to update with values from the deserialized DTO.</param>
    /// <param name="path">The path to the JSON file.</param>
    /// <param name="options">Optional JSON serialization options.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown if the file does not exist or deserialization fails.</exception>
    public static async Task FromJsonAsync<TDto, TObject>(TObject target, string path, JsonSerializerOptions? options = null)
        where TDto : IDTO<TObject>
    {
        if (!File.Exists(path))
            throw new Exception("Error JSON path!");

        options ??= new JsonSerializerOptions
        {
            IncludeFields = true,
        };

        TDto.RegisterDTO();
        TDto.RegisterDTOJsonConverters(options);

        await using FileStream stream = File.OpenRead(path);
        TDto? dto = await JsonSerializer.DeserializeAsync<TDto>(stream, options);

        if (dto is null)
            throw new Exception("DTO is null (FromJSONAsync)");

        dto.ToObject(target);
    }
}