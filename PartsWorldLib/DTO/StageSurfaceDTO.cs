using System.Text.Json.Serialization;
using System.Text.Json;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using EffectLib.EffectCore;
using EffectLib;
using ScreenLib.Output;
using TextureLib.Loader.ImageProcessing;
using System.Xml.Linq;
using PartsWorldLib.Up;
using TextureLib.Loader;

namespace PartsWorldLib.DTO;
/// <summary>
/// Data Transfer Object (DTO) for <see cref="StageSurface"/>.
/// Used to serialize and deserialize surface properties, including shader, texture, output layer, and effect.
/// </summary>
public class StageSurfaceDTO : IDTO<StageSurface>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    [JsonIgnore]
    private StageSurface stageSurface;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public string BaseFileName { get; } = "stagesurface_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public string BaseExtension { get; } = ".json";


    /// <summary>
    /// Gets or sets the output rendering layer priority for this surface.
    /// </summary>
    public OutputPriorityType outputLayer;

    /// <summary>
    /// Gets or sets the path to the shader resource.
    /// </summary>
    public string shaderPath = string.Empty;

    /// <summary>
    /// Gets or sets the path to the texture resource.
    /// </summary>
    public string texturePath = string.Empty;

    /// <summary>
    /// Contains serialized data related to the effect's basic properties.
    /// </summary>
    public IEffectDTO? IEffectDTO;

    /// <summary>
    /// Gets or sets the image loading options used to configure how images are processed and loaded.
    /// </summary>
    public ImageLoadOptionsDTO LoadOptions;




    static StageSurfaceDTO()
    {
        RegistratorDTO<IEffectDTO, IEffect>.EnsureInitialized();
        DtoTypeRegistry.Register<IEffectDTO, IEffect>();
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="StageSurfaceDTO"/> class.
    /// </summary>
    public StageSurfaceDTO() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="StageSurfaceDTO"/> class with the given <see cref="StageSurface"/>.
    /// </summary>
    /// <param name="stageSurface">The stageSurface to wrap in the DTO.</param>
    public StageSurfaceDTO(StageSurface stageSurface)
    {
        this.stageSurface = stageSurface;
        IEffectDTO = stageSurface.Effect is not null ? DTOFactory<IEffectDTO, IEffect>.CreateFrom(stageSurface.Effect) : null;
        LoadOptions = new ImageLoadOptionsDTO(stageSurface.LoadOptions);
    }


    /// <summary>
    /// Populates the DTO fields from the associated obstacle object.
    /// </summary>
    public void ToDTO()
    {
        IEffectDTO?.ToDTO();
        LoadOptions.ToDTO();

        outputLayer = stageSurface.OutputLayer;
        shaderPath = stageSurface.ShaderPath;
        texturePath = stageSurface.TexturePath;
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
        RegistratorDTO<IEffectDTO, IEffect>.RegisterAllObjectDTOJsonConverters(options);
        DtoTypeRegistry.RegisterAllDTOJsonConverters(options);
        ImageLoadOptionsDTO.RegisterDTOJsonConverters(options);
    }


    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public StageSurface ToObject()
    {
        throw new NotImplementedException($"Cannot create an instance from abstract class '{typeof(StageSurfaceDTO).Name}'. " +
        $"The method '{nameof(ToObject)}' must be overridden in the concrete DTO class that implements '{typeof(StageSurfaceDTO).Name}'.");
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(StageSurface stageSurface)
    {
        stageSurface.Effect = IEffectDTO?.ToObject();
        stageSurface.LoadOptions = LoadOptions.ToObject();

        stageSurface.OutputLayer = outputLayer;
        stageSurface.ShaderPath = shaderPath;
        stageSurface.TexturePath = texturePath;
    }

    /// <summary>
    /// Populates an existing <see cref="StageSurface"/> instance using data from the provided <see cref="StageSurfaceDTO"/>.
    /// </summary>
    /// <param name="stageSurface">The <see cref="StageSurface"/> instance to populate.</param>
    /// <param name="dto">The <see cref="StageSurfaceDTO"/> containing the serialized obstacle data.</param>
    /// <returns>The updated <see cref="StageSurface"/> instance with values copied from the DTO.</returns>
    /// <remarks>
    /// This method deserializes position, hitbox, color, texture, scale, and logical flags such as passability and collision behavior.
    /// It is used to reconstruct a runtime <see cref="StageSurface"/> object from serialized DTO data.
    /// </remarks>
    public static StageSurface ToObject(StageSurface stageSurface, StageSurfaceDTO dto)
    {
        stageSurface.Effect = dto.IEffectDTO?.ToObject();
        stageSurface.LoadOptions = dto.LoadOptions.ToObject();

        stageSurface.OutputLayer = dto.outputLayer;
        stageSurface.ShaderPath = dto.shaderPath;
        stageSurface.TexturePath = dto.texturePath;

        return stageSurface;
    }
}
