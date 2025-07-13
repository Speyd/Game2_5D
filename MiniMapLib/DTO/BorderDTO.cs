using System.Text.Json.Serialization;
using System.Text.Json;
using DataPipes.DTO;
using DataPipes.DTO.Register;


namespace MiniMapLib.DTO;
/// <summary>
/// Data Transfer Object (DTO) for the <see cref="Border"/> class.
/// Handles serialization and deserialization by mapping domain object properties
/// to JSON-serializable fields and back.
/// </summary>
public class BorderDTO : IDTO<Border>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    [JsonIgnore]
    private Border border;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public string BaseFileName { get; } = "border_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public string BaseExtension { get; } = ".json";


    /// <summary>
    /// The path to the texture used by the border.
    /// </summary>
    public string texturePath = string.Empty;


    /// <summary>
    /// Parameterless constructor required for deserialization and manual population of properties.
    /// </summary>
    public BorderDTO() { }
    /// <summary>
    /// Initializes the DTO using an existing <see cref="Border"/> instance,
    /// preparing it for conversion to a serializable format.
    /// </summary>
    /// <param name="border">The source <see cref="Border"/> object.</param>
    public BorderDTO(Border border)
    {
        this.border = border;
    }

    /// <summary>
    /// Converts the internal <see cref="Border"/> object to a DTO format
    /// by extracting primitive values and frame paths for serialization.
    /// </summary>
    public void ToDTO()
    {
        texturePath = border.TexturePath;
    }

    /// <summary>
    /// Ensures that the DTO registration logic is initialized once per application domain.
    /// Typically used to prepare internal mappings or caches for DTOs.
    /// </summary>
    public static void RegisterDTO() { }
    /// <summary>
    /// Registers custom JSON converters required for correct serialization and deserialization of DTO types.
    /// </summary>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to which converters will be added.</param>
    public static void RegisterDTOJsonConverters(JsonSerializerOptions options)
    {
        DtoTypeRegistry.RegisterAllDTOJsonConverters(options);
    }

    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public Border ToObject()
    {
        throw new Exception($"This method has no implementation({typeof(BorderDTO)}).");
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(Border border)
    {
        border.TexturePath = texturePath;
    }
}
