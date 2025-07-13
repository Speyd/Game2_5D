using System.Text.Json.Serialization;
using System.Text.Json;
using System.Reflection;

namespace DataPipes.DTO.Register;
/// <summary>
/// A custom JSON converter for serializing and deserializing implementations of <see cref="TDto"/>.
/// </summary>
/// <typeparam name="TDto">The type of DTO objects to create.</typeparam>
/// <typeparam name="TObject">The source object type from which DTOs are created.</typeparam>
/// <remarks>
/// This converter uses a type discriminator property ("Type") to determine the concrete type of the object during deserialization.
/// It reads the "Type" property from JSON, uses it to select the appropriate deserializer from the <see cref="Deserializers"/> dictionary,
/// and writes the "Type" property on serialization to include type information.
/// This enables polymorphic JSON serialization and deserialization of DTOs implementing <see cref="IRegisterableDTO{TDto, TObject}"/>.
/// </remarks>
public class DTOJsonConverter<TDto, TObject> : JsonConverter<TDto>
    where TDto : class, IDTO<TObject>, IRegisterableDTO<TDto, TObject>
{
    /// <summary>
    /// The JSON property name used as the type discriminator for identifying
    /// the concrete DTO type during serialization and deserialization.
    /// </summary>
    public static readonly string TypePropertyName = "Type";
    /// <summary>
    /// A dictionary mapping type identifiers (as strings) to deserialization functions.
    /// Each function takes a raw JSON string and deserialization options and returns a specific implementation of <see cref="TDto"/>.
    /// </summary>
    public static Dictionary<string, Func<string, JsonSerializerOptions, TDto?>> Deserializers = new();

    /// <summary>
    /// Reads and deserializes an <see cref="TDto"/> from the JSON input based on the TypePropertyName property.
    /// </summary>
    /// <param name="reader">The UTF-8 JSON reader positioned at the start of the object.</param>
    /// <param name="typeToConvert">The type to convert (expected to be <see cref="TDto"/>).</param>
    /// <param name="options">The JSON serialization options to use during deserialization.</param>
    /// <returns>The deserialized <see cref="TDto"/> instance.</returns>
    /// <exception cref="JsonException">Thrown if the TypePropertyName property is missing or not registered in the <see cref="Deserializers"/> dictionary.</exception>
    public override TDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty(TypePropertyName, out var typeProperty))
            throw new JsonException($"Missing {TypePropertyName} property");

        var type = typeProperty.GetString();

        if (type is null || !Deserializers.TryGetValue(type, out var deserializer))
            throw new JsonException($"Unknown Type: {type}");

        return deserializer(root.GetRawText(), options);
    }

    /// <summary>
    /// Writes the specified <see cref="TDto"/> to JSON, including its type information as a TypePropertyName.
    /// </summary>
    /// <param name="writer">The JSON writer to write to.</param>
    /// <param name="value">The <see cref="TDto"/> value to serialize.</param>
    /// <param name="options">The JSON serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, TDto value, JsonSerializerOptions options)
    {
        var json = JsonSerializer.SerializeToElement(value, value.GetType(), options);

        writer.WriteStartObject();
        writer.WriteString(TypePropertyName, value.GetType().Name);

        foreach (var prop in json.EnumerateObject())
            prop.WriteTo(writer);

        writer.WriteEndObject();
    }
}