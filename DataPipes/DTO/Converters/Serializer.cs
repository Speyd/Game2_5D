using DataPipes.DTO.Register;
using System.Reflection;
using System.Text.Json;

namespace DataPipes.DTO.Converters;
/// <summary>
/// Provides generic methods for serializing domain objects into DTOs and saving them to files (currently JSON only).
/// </summary>
/// <typeparam name="TDto">The DTO type implementing <see cref="IDTO{TObject}"/> and <see cref="IRegisterableDTO{TDto, TObject}"/>.</typeparam>
/// <typeparam name="TObject">The domain object type to serialize.</typeparam>
/// <remarks>
/// The class uses reflection to invoke the static CreateDTO method defined in the DTO type.
/// </remarks>
public static class Serializer
{
    /// <summary>
    /// Uses reflection to invoke the static <c>CreateDTO</c> method on the DTO type and create a DTO instance from the target object.
    /// </summary>
    /// <param name="target">The domain object to convert into a DTO.</param>
    /// <returns>An instance of <typeparamref name="TDto"/> representing the DTO.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the CreateDTO method is not found on the DTO type.</exception>
    /// <exception cref="InvalidCastException">Thrown if the result cannot be cast to <typeparamref name="TDto"/>.</exception>
    public static TDto CreateTDto<TDto, TObject>(TObject target)
         where TDto : IDTO<TObject>, IRegisterableDTO<TDto, TObject>
    {
        var method = typeof(TDto)?.GetMethod(
              IRegisterableDTO<TDto, TObject>.CreateDTOPropertyName,
              BindingFlags.Public | BindingFlags.Static,
              null,
              new[] { typeof(TObject) },
              null
          );
        if (method is null)
            throw new InvalidOperationException($"Method '{IRegisterableDTO<TDto, TObject>.CreateDTOPropertyName}' not found in {typeof(TDto)}");

        object? result = method.Invoke(null, new object[] { target });
        if (result is not TDto dto)
            throw new InvalidCastException($"Cannot cast result of '{IRegisterableDTO<TDto, TObject>.CreateDTOPropertyName}' to {typeof(TDto)}");

        return dto;
    }

    /// <summary>
    /// Serializes the specified domain object into a JSON file using its corresponding DTO representation.
    /// </summary>
    /// <param name="target">The object to serialize.</param>
    /// <param name="directory">The output directory where the JSON file will be created.</param>
    /// <param name="options">Optional JSON serialization options. If not provided, defaults will be used.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the DTO's CreateDTO method is not found.</exception>
    /// <exception cref="InvalidCastException">Thrown if the created object is not of the expected DTO type.</exception>
    /// <remarks>
    /// The JSON file path is generated using the DTO's base name and extension via <see cref="IDTO{TObject}.GetUniqueFilePath"/>.
    /// </remarks>
    public static async Task ToJsonAsync<TDto, TObject>(TObject target, string directory, JsonSerializerOptions? options = null)
         where TDto : IDTO<TObject>, IRegisterableDTO<TDto, TObject>
    {
        options = options ?? new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true
        };
        TDto.RegisterDTO();
        TDto.RegisterDTOJsonConverters(options);

        TDto dto = CreateTDto<TDto, TObject>(target);
        dto.ToDTO();

        using FileStream createStream = File.Create(IDTO<TDto>.GetUniqueFilePath(directory, dto.BaseFileName, dto.BaseExtension));
        await JsonSerializer.SerializeAsync(createStream, dto, options);

        await createStream.FlushAsync();
    }

    /// <summary>
    /// Serializes the specified domain object into a JSON file using its corresponding DTO representation.
    /// </summary>
    /// <param name="target">The object to serialize.</param>
    /// <param name="directory">The output directory where the JSON file will be created.</param>
    /// <param name="options">Optional JSON serialization options. If not provided, defaults will be used.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the DTO's CreateDTO method is not found.</exception>
    /// <exception cref="InvalidCastException">Thrown if the created object is not of the expected DTO type.</exception>
    /// <remarks>
    /// The JSON file path is generated using the DTO's base name and extension via <see cref="IDTO{TObject}.GetUniqueFilePath"/>.
    /// </remarks>
    public static void ToJson<TDto, TObject>(TObject target, string directory, JsonSerializerOptions? options = null)
         where TDto : IDTO<TObject>, IRegisterableDTO<TDto, TObject>
    {
        options = options ?? new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true
        };
        TDto.RegisterDTO();
        TDto.RegisterDTOJsonConverters(options);

        TDto dto = CreateTDto<TDto, TObject>(target);
        dto.ToDTO();

        using FileStream createStream = File.Create(IDTO<TDto>.GetUniqueFilePath(directory, dto.BaseFileName, dto.BaseExtension));
        JsonSerializer.Serialize(createStream, dto, options);

        createStream.Flush();
    }
}
