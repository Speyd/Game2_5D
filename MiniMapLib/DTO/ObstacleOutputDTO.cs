using System.Text.Json.Serialization;
using System.Text.Json;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using MiniMapLib.ObjectInMap.Obstacles;

namespace MiniMapLib.DTO;
/// <summary>
/// Data Transfer Object (DTO) for the <see cref="ObstacleOutput"/> class.
/// Provides serialization and deserialization support, mapping domain object properties
/// to JSON-serializable fields and vice versa.
/// </summary>
public class ObstacleOutputDTO : IDTO<ObstacleOutput>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    [JsonIgnore]
    private ObstacleOutput obstacleOutput;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public string BaseFileName { get; } = "obstacleOutput_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public string BaseExtension { get; } = ".json";


    /// <summary>
    /// The rendering mode for displaying the obstacle.
    /// </summary>
    public DisplayRenderMode renderMode;

    /// <summary>
    /// The method used to render the obstacle output.
    /// </summary>
    public OutputRenderMethod renderMethod;

    /// <summary>
    /// The outline thickness or setting for the obstacle rendering.
    /// </summary>
    public int outLine;

    /// <summary>
    /// Parameterless constructor required for deserialization and manual population of properties.
    /// </summary>
    public ObstacleOutputDTO() { }
    /// <summary>
    /// Initializes the DTO using an existing <see cref="ObstacleOutput"/> instance,
    /// preparing it for conversion to a serializable format.
    /// </summary>
    /// <param name="obstacleOutput">The source <see cref="ObstacleOutput"/> object.</param>
    public ObstacleOutputDTO(ObstacleOutput obstacleOutput)
    {
        this.obstacleOutput = obstacleOutput;
    }

    /// <summary>
    /// Converts the internal <see cref="ObstacleOutput"/> object to a DTO format
    /// by extracting primitive values and frame paths for serialization.
    /// </summary>
    public void ToDTO()
    {
        renderMode = obstacleOutput.RenderMode;
        renderMethod = obstacleOutput.OutputRenderMethod;
        outLine = obstacleOutput.OutLine;
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
    public ObstacleOutput ToObject()
    {
        throw new Exception($"This method has no implementation({typeof(BorderDTO)}).");
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(ObstacleOutput obstacleOutput)
    {
        obstacleOutput.RenderMode = renderMode;
        obstacleOutput.OutputRenderMethod = renderMethod;
        obstacleOutput.OutLine = outLine;
    }
}
