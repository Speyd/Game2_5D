using ScreenLib.Output;
using System.Text.Json.Serialization;
using System.Text.Json;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using PartsWorldLib.Up;

namespace PartsWorldLib.DTO;
/// <summary>
/// Data Transfer Object (DTO) for the <see cref="Ceiling"/> class.
/// Responsible for serialization and deserialization of the main ceiling properties,
/// including fill color and output layer priority.
/// Implements the <see cref="IUpPartDTO"/>, <see cref="IDTO{Ceiling}"/>, and
/// <see cref="IRegisterableDTO{CeilingDTO, Ceiling}"/> interfaces to support registration and conversion.
/// </summary>
[RegisterDTO(typeof(Ceiling))]
public class CeilingDTO : IUpPartDTO, IDTO<Ceiling>, IRegisterableDTO<CeilingDTO, Ceiling>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static new DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();
    /// <summary>
    /// A reference to the original <see cref="Ceiling"/> object. Ignored during serialization.
    /// </summary>
    [JsonIgnore]
    private Ceiling ceiling;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public override string BaseFileName { get; } = "ceiling_";
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

    public CeilingDTO() { }
    /// <summary>
    /// Constructs the DTO from an existing <see cref="Ceiling"/> instance.
    /// </summary>
    /// <param name="ceiling">The source object to be serialized.</param>
    public CeilingDTO(Ceiling ceiling)
    {
        this.ceiling = ceiling;
    }


    /// <summary>
    /// Populates this DTO with data extracted from the original <see cref="Ceiling"/> instance,
    /// including scale, angle, distance, and animation state.
    /// </summary>
    public override void ToDTO()
    {
        outputLayer = ceiling.OutputLayer;

        R = ceiling.ColorFilling.R;
        G = ceiling.ColorFilling.G;
        B = ceiling.ColorFilling.B;
        A = ceiling.ColorFilling.A;
    }

    /// <summary>
    /// Creates a new instance of <see cref="CeilingDTO"/> from the given <see cref="IUpPart"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="CeilingDTO"/>.</returns>
    public static new IUpPartDTO CreateDTO(IUpPart obj) => new CeilingDTO((Ceiling)obj);
    /// <summary>
    /// Creates a new instance of <see cref="Ceiling"/> from the given <see cref="Ceiling"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="CeilingDTO"/>.</returns>
    public static CeilingDTO CreateDTO(Ceiling obj) => new CeilingDTO(obj);

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
        IUpPartDTO.DtoTypeRegistry.RegisterAllDTOJsonConverters(options);
    }


    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public override Ceiling ToObject()
    {
        Ceiling ceiling = new Ceiling();
        ceiling.ColorFilling = new SFML.Graphics.Color(R, G, B, A);
        ceiling.OutputLayer = outputLayer;

        return ceiling;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public override void ToObject(IUpPart obj)
    {
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(Ceiling ceiling)
    {
        ceiling.ColorFilling = new SFML.Graphics.Color(R, G, B, A);
        ceiling.OutputLayer = outputLayer;
    }
}

