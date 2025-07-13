using System.Text.Json.Serialization;
using System.Text.Json;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using PartsWorldLib.Down;
using ScreenLib.Output;
using PartsWorldLib.Up;


namespace PartsWorldLib.DTO;
/// <summary>
/// Data Transfer Object (DTO) for the <see cref="Floor"/> class.
/// Provides serialization and deserialization functionality for a <see cref="Floor"/> instance,
/// including its properties such as texture, shader, output layer, and effect.
/// Implements <see cref="IDownPartDTO"/> for integration with downward-rendered surfaces,
/// <see cref="IDTO{T}"/> for general DTO behavior,
/// and <see cref="IRegisterableDTO{TDTO, TObject}"/> for DTO type registration support.
/// </summary>
[RegisterDTO(typeof(Floor))]
public class FloorDTO : IDownPartDTO, IDTO<Floor>, IRegisterableDTO<FloorDTO, Floor>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static new DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();
    /// <summary>
    /// A reference to the original <see cref="Floor"/> object. Ignored during serialization.
    /// </summary>
    [JsonIgnore]
    private Floor floor;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public override string BaseFileName { get; } = "floor_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public override string BaseExtension { get; } = ".json";

    /// <summary>
    /// Gets or sets the output rendering layer priority for this surface.
    /// </summary>
    public OutputPriorityType outputLayer;

    /// <summary>
    /// Red channel value of the color.
    /// </summary>
    public byte R;

    /// <summary>
    /// Green channel value of the color.
    /// </summary>
    public byte G;

    /// <summary>
    /// Blue channel value of the color.
    /// </summary>
    public byte B;

    /// <summary>
    /// Alpha channel value of the color.
    /// </summary>
    public byte A;

    public FloorDTO() { }
    /// <summary>
    /// Constructs the DTO from an existing <see cref="Floor"/> instance.
    /// </summary>
    /// <param name="floor">The source object to be serialized.</param>
    public FloorDTO(Floor floor)
    {
        this.floor = floor;
    }


    /// <summary>
    /// Populates this DTO with data extracted from the original <see cref="Floor"/> instance,
    /// including scale, angle, distance, and animation state.
    /// </summary>
    public override void ToDTO()
    {
        outputLayer = floor.OutputLayer;

        R = floor.ColorFilling.R;
        G = floor.ColorFilling.G;
        B = floor.ColorFilling.B;
        A = floor.ColorFilling.A;
    }

    /// <summary>
    /// Creates a new instance of <see cref="FloorDTO"/> from the given <see cref="IDownPart"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="FloorDTO"/>.</returns>
    public static new IDownPartDTO CreateDTO(IDownPart obj) => new FloorDTO((Floor)obj);
    /// <summary>
    /// Creates a new instance of <see cref="FloorDTO"/> from the given <see cref="Floor"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="FloorDTO"/>.</returns>
    public static FloorDTO CreateDTO(Floor obj) => new FloorDTO(obj);

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
        IDownPartDTO.DtoTypeRegistry.RegisterAllDTOJsonConverters(options);
    }


    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public override Floor ToObject()
    {
        Floor floor = new Floor();
        floor.ColorFilling = new SFML.Graphics.Color(R, G, B, A);
        floor.OutputLayer = outputLayer;

        return floor;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public override void ToObject(IDownPart obj)
    {
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(Floor floor)
    {
        floor.ColorFilling = new SFML.Graphics.Color(R, G, B, A);
        floor.OutputLayer = outputLayer;
    }
}
