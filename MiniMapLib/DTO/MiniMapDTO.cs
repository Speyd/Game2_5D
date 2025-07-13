using System.Text.Json.Serialization;
using System.Text.Json;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using ScreenLib.Output;


namespace MiniMapLib.DTO;
/// <summary>
/// Data Transfer Object (DTO) for the <see cref="MiniMap"/> class.
/// Provides methods for serialization and deserialization of the MiniMap domain object,
/// including its composed DTOs for Border, PlayerCircle, Obstacles, PlayerLine, Settings, and Zoom.
/// </summary>
public class MiniMapDTO : IDTO<MiniMap>, IRegisterableDTO<MiniMapDTO, MiniMap>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    [JsonIgnore]
    private MiniMap miniMap;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public string BaseFileName { get; } = "miniMap_";
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
    /// DTO representing the border of the MiniMap.
    /// </summary>
    public BorderDTO Border { get; set; }

    /// <summary>
    /// DTO representing the player circle on the MiniMap.
    /// </summary>
    public PlayerCircleDTO PlayerCircle { get; set; }

    /// <summary>
    /// DTO representing the obstacle output on the MiniMap.
    /// </summary>
    public ObstacleOutputDTO ObstacleOutput { get; set; }

    /// <summary>
    /// DTO representing the player line on the MiniMap.
    /// </summary>
    public PlayerLineDTO PlayerLine { get; set; }

    /// <summary>
    /// DTO representing the settings of the MiniMap.
    /// </summary>
    public SettingDTO Setting { get; set; }

    /// <summary>
    /// DTO representing the zoom settings of the MiniMap.
    /// </summary>
    public ZoomDTO Zoom { get; set; }


    /// <summary>
    /// Parameterless constructor required for deserialization and manual population of properties.
    /// </summary>
    public MiniMapDTO() { }
    /// <summary>
    /// Initializes the DTO using an existing <see cref="MiniMap"/> instance,
    /// preparing it for conversion to a serializable format.
    /// </summary>
    /// <param name="miniMap">The source <see cref="MiniMap"/> object.</param>
    public MiniMapDTO(MiniMap miniMap)
    {
        this.miniMap = miniMap;
        Border = new BorderDTO(miniMap.Border);
        PlayerCircle = new PlayerCircleDTO(miniMap.PlayerCircle);
        ObstacleOutput = new ObstacleOutputDTO(miniMap.Obstacle);
        PlayerLine = new PlayerLineDTO(miniMap.PlayerLine);
        Setting = new SettingDTO(miniMap.Setting);
        Zoom = new ZoomDTO(miniMap.Zoom);
    }

    /// <summary>
    /// Converts the internal <see cref="MiniMap"/> object to a DTO format
    /// by extracting primitive values and frame paths for serialization.
    /// </summary>
    public void ToDTO()
    {
        outputLayer = miniMap.OutputLayer;

        Border.ToDTO();
        PlayerCircle.ToDTO();
        ObstacleOutput.ToDTO();
        PlayerLine.ToDTO();
        Setting.ToDTO();
        Zoom.ToDTO();
    }

    /// <summary>
    /// Creates a new instance of <see cref="MiniMap"/> from the given <see cref="MiniMap"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="MiniMapDTO"/>.</returns>
    public static MiniMapDTO CreateDTO(MiniMap obj) => new MiniMapDTO(obj);

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
    public MiniMap ToObject()
    {
        throw new Exception($"This method has no implementation({typeof(MiniMapDTO)}).");
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(MiniMap miniMap)
    {
        miniMap.OutputLayer = outputLayer;

        Border.ToObject(miniMap.Border);
        PlayerCircle.ToObject(miniMap.PlayerCircle);
        ObstacleOutput.ToObject(miniMap.Obstacle);
        PlayerLine.ToObject(miniMap.PlayerLine);
        Setting.ToObject(miniMap.Setting);
        Zoom.ToObject(miniMap.Zoom);
    }
}