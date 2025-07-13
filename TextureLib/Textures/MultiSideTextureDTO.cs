using System.Text.Json.Serialization;
using System.Text.Json;
using TextureLib.Loader.ImageProcessing;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using TextureLib.Textures.Pair;

namespace TextureLib.Textures;
/// <summary>
/// Data Transfer Object (DTO) for <see cref="MultiSideTexture"/>.
/// Encapsulates data required to serialize and deserialize textures
/// with per-side and shared texture configurations, along with image loading options.
/// </summary>
public class MultiSideTextureDTO : IDTO<MultiSideTexture>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    [JsonIgnore]
    private MultiSideTexture multiTexture;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public string BaseFileName { get; } = "multiSideTexture_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public string BaseExtension { get; } = ".json";


    /// <summary>
    /// Stores unique texture paths for each specific object side.
    /// </summary>
    public Dictionary<ObjectSide, string> UniqueTexture = new();

    /// <summary>
    /// Stores the sides of the object which share the same texture.
    /// </summary>
    public HashSet<ObjectSide> SharedSides;

    /// <summary>
    /// Gets or sets the image loading options used to configure how images are processed and loaded.
    /// </summary>
    public ImageLoadOptionsDTO LoadOptions;


    /// <summary>
    /// Parameterless constructor.
    /// </summary>
    public MultiSideTextureDTO()
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="MultiSideTextureDTO"/> class from a <see cref="MultiSideTexture"/> object.
    /// </summary>
    /// <param name="multiTexture">The domain object to convert into DTO.</param>
    public MultiSideTextureDTO(MultiSideTexture multiTexture)
    {
        this.multiTexture = multiTexture;
        LoadOptions = new ImageLoadOptionsDTO(multiTexture.LoadOptions);
    }

    /// <summary>
    /// Converts a HitBoxSide instance into this DTO representation.
    /// </summary>
    public void ToDTO()
    {
        LoadOptions.ToDTO();

        SharedSides = multiTexture.SharedSides;
        foreach (var key in multiTexture.UniqueTexture.GetAllKey())
        {
            var value = multiTexture[key];
            if (value is not null)
                UniqueTexture.Add(key, value.Base.PathTexture);
        }
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
    public MultiSideTexture ToObject()
    {
        var multiTexture = new MultiSideTexture(UniqueTexture, SharedSides, LoadOptions.ToObject());

        return multiTexture;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(MultiSideTexture multiTexture)
    {
        multiTexture = new MultiSideTexture(UniqueTexture, SharedSides, LoadOptions.ToObject());
    }
}