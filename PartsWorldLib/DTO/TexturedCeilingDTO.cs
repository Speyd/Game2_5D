using System.Text.Json.Serialization;
using System.Text.Json;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using PartsWorldLib.Up;

namespace PartsWorldLib.DTO;
/// <summary>
/// Data Transfer Object (DTO) for the <see cref="TexturedCeiling"/> class.
/// Handles serialization and deserialization of <see cref="TexturedCeiling"/>,
/// including its texture, shader, vertical camera offsets, and stage surface data.
/// Implements <see cref="IUpPartDTO"/> for integration with upward-rendered parts,
/// <see cref="IDTO{T}"/> for generic DTO functionality,
/// and <see cref="IRegisterableDTO{TDTO, TObject}"/> for registration and type-mapping support.
/// </summary>
[RegisterDTO(typeof(TexturedCeiling))]
public class TexturedCeilingDTO : IUpPartDTO, IDTO<TexturedCeiling>, IRegisterableDTO<TexturedCeilingDTO, TexturedCeiling>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static new DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();
    /// <summary>
    /// A reference to the original <see cref="TexturedCeilingDTO"/> object. Ignored during serialization.
    /// </summary>
    [JsonIgnore]
    private TexturedCeiling ceiling;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public override string BaseFileName { get; } = "texturedCeiling_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public override string BaseExtension { get; } = ".json";


    /// <summary>
    /// Gets or sets the base vertical offset applied to the floor in camera units.
    /// </summary>
    public float offsetCameraY;

    /// <summary>
    /// Gets or sets the attenuation factor applied when calculating the vertical camera offset.
    /// </summary>
    public float offsetCameraYAttenuation;

    /// <summary>
    /// DTO containing texture, shader, and rendering details for the stage surface.
    /// </summary>
    public StageSurfaceDTO StageSurfaceDTO = new();


    public TexturedCeilingDTO() { }
    /// <summary>
    /// Constructs the DTO from an existing <see cref="TexturedCeiling"/> instance.
    /// </summary>
    /// <param name="ceiling">The source object to be serialized.</param>
    public TexturedCeilingDTO(TexturedCeiling ceiling)
    {
        this.ceiling = ceiling;
        StageSurfaceDTO = new StageSurfaceDTO(ceiling);
    }


    /// <summary>
    /// Populates this DTO with data extracted from the original <see cref="TexturedCeiling"/> instance,
    /// including scale, angle, distance, and animation state.
    /// </summary>
    public override void ToDTO()
    {
        StageSurfaceDTO.ToDTO();
        offsetCameraY = ceiling.OffsetCameraY;
        offsetCameraYAttenuation = ceiling.OffsetCameraYAttenuation;
    }

    /// <summary>
    /// Creates a new instance of <see cref="TexturedCeilingDTO"/> from the given <see cref="IUpPart"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="TexturedCeilingDTO"/>.</returns>
    public static new IUpPartDTO CreateDTO(IUpPart obj) => new TexturedCeilingDTO((TexturedCeiling)obj);
    /// <summary>
    /// Creates a new instance of <see cref="TexturedCeilingDTO"/> from the given <see cref="TexturedCeiling"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="TexturedCeiling"/>.</returns>
    public static TexturedCeilingDTO CreateDTO(TexturedCeiling obj) => new TexturedCeilingDTO(obj);

    /// <summary>
    /// Ensures that the DTO registration logic is initialized once per application domain.
    /// Typically used to prepare internal mappings or caches for DTOs.
    /// </summary>
    public static new void RegisterDTO() { }
    /// <summary>
    /// Registers custom JSON converters required for correct serialization and deserialization of DTO types.
    /// </summary>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to which converters will be added.</param>
    public static new void RegisterDTOJsonConverters(JsonSerializerOptions options)
    {
        DtoTypeRegistry.RegisterAllDTOJsonConverters(options);
        StageSurfaceDTO.DtoTypeRegistry.RegisterAllDTOJsonConverters(options);
        IUpPartDTO.DtoTypeRegistry.RegisterAllDTOJsonConverters(options);
    }


    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public override TexturedCeiling ToObject()
    {
        TexturedCeiling ceiling = new TexturedCeiling(StageSurfaceDTO.texturePath, StageSurfaceDTO.shaderPath);
        StageSurfaceDTO.ToObject(ceiling);
        ceiling.OffsetCameraY = offsetCameraY;
        ceiling.OffsetCameraYAttenuation = offsetCameraYAttenuation;

        return ceiling;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public override void ToObject(IUpPart obj)
    {
        obj.OutputLayer = StageSurfaceDTO.outputLayer;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(TexturedCeiling ceiling)
    {
        StageSurfaceDTO.ToObject(ceiling);
        ceiling.OffsetCameraY = offsetCameraY;
        ceiling.OffsetCameraYAttenuation = offsetCameraYAttenuation;
    }
}
