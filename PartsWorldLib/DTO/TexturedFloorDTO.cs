using System.Text.Json.Serialization;
using System.Text.Json;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using PartsWorldLib.Down;


namespace PartsWorldLib.DTO;
/// <summary>
/// Data Transfer Object (DTO) for the <see cref="TexturedFloor"/> class.
/// Handles serialization and deserialization of <see cref="TexturedFloor"/>, 
/// including its texture, shader, vertical camera offsets, and stage surface data.
/// Implements <see cref="IDownPartDTO"/> for integration with downward-rendered parts,
/// <see cref="IDTO{T}"/> for generic DTO functionality,
/// and <see cref="IRegisterableDTO{TDTO, TObject}"/> for registration and type-mapping support.
/// </summary>
[RegisterDTO(typeof(TexturedFloor))]
public class TexturedFloorDTO : IDownPartDTO, IDTO<TexturedFloor>, IRegisterableDTO<TexturedFloorDTO, TexturedFloor>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static new DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();
    /// <summary>
    /// A reference to the original <see cref="TexturedFloor"/> object. Ignored during serialization.
    /// </summary>
    [JsonIgnore]
    private TexturedFloor floor;
 
    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public override string BaseFileName { get; } = "texturedFloor_";
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


    public TexturedFloorDTO() { }
    /// <summary>
    /// Constructs the DTO from an existing <see cref="TexturedFloor"/> instance.
    /// </summary>
    /// <param name="floor">The source object to be serialized.</param>
    public TexturedFloorDTO(TexturedFloor floor)
    {
        this.floor = floor;
        StageSurfaceDTO = new StageSurfaceDTO(floor);
    }


    /// <summary>
    /// Populates this DTO with data extracted from the original <see cref="TexturedFloor"/> instance,
    /// including scale, angle, distance, and animation state.
    /// </summary>
    public override void ToDTO()
    {
        StageSurfaceDTO.ToDTO();
        offsetCameraY = floor.OffsetCameraY;
        offsetCameraYAttenuation = floor.OffsetCameraYAttenuation;
    }

    /// <summary>
    /// Creates a new instance of <see cref="TexturedFloorDTO"/> from the given <see cref="IDownPart"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="TexturedFloorDTO"/>.</returns>
    public static new IDownPartDTO CreateDTO(IDownPart obj) => new TexturedFloorDTO((TexturedFloor)obj);
    /// <summary>
    /// Creates a new instance of <see cref="TexturedFloorDTO"/> from the given <see cref="TexturedFloor"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="TexturedFloorDTO"/>.</returns>
    public static TexturedFloorDTO CreateDTO(TexturedFloor obj) => new TexturedFloorDTO(obj);

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
        IDownPartDTO.DtoTypeRegistry.RegisterAllDTOJsonConverters(options);
    }


    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public override TexturedFloor ToObject()
    {
        TexturedFloor floor = new TexturedFloor(StageSurfaceDTO.texturePath, StageSurfaceDTO.shaderPath);
        StageSurfaceDTO.ToObject(floor);
        floor.OffsetCameraY = offsetCameraY;
        floor.OffsetCameraYAttenuation = offsetCameraYAttenuation;

        return floor;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public override void ToObject(IDownPart obj)
    {
        obj.OutputLayer = StageSurfaceDTO.outputLayer;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(TexturedFloor floor)
    {
        StageSurfaceDTO.ToObject(floor);
        floor.OffsetCameraY = offsetCameraY;
        floor.OffsetCameraYAttenuation = offsetCameraYAttenuation;
    }
}
