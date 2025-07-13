using System.Text.Json.Serialization;
using System.Text.Json;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using MiniMapLib.ObjectInMap.Player;

namespace MiniMapLib.DTO;
/// <summary>
/// Data Transfer Object (DTO) for the <see cref="PlayerLineOutput"/> class.
/// Used to serialize and deserialize the player's line settings and color.
/// </summary>
public class PlayerLineDTO : IDTO<PlayerLineOutput>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    [JsonIgnore]
    private PlayerLineOutput playerLine;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public string BaseFileName { get; } = "playerLine_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public string BaseExtension { get; } = ".json";

    /// <summary>
    /// Length of the player's line.
    /// </summary>
    public int length;
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


    /// <summary>
    /// Parameterless constructor required for deserialization and manual population of properties.
    /// </summary>
    public PlayerLineDTO() { }
    /// <summary>
    /// Initializes the DTO using an existing <see cref="PlayerLineOutput"/> instance,
    /// preparing it for conversion to a serializable format.
    /// </summary>
    /// <param name="playerLine">The source <see cref="PlayerLineOutput"/> object.</param>
    public PlayerLineDTO(PlayerLineOutput playerLine)
    {
        this.playerLine = playerLine;
    }

    /// <summary>
    /// Converts the internal <see cref="PlayerCircleOutput"/> object to a DTO format
    /// by extracting primitive values and frame paths for serialization.
    /// </summary>
    public void ToDTO()
    {
        length = playerLine.Length;

        R = playerLine.Color.R; 
        G = playerLine.Color.G;
        B = playerLine.Color.B;
        A = playerLine.Color.A;
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
    public PlayerLineOutput ToObject()
    {
        throw new Exception($"This method has no implementation({typeof(PlayerLineDTO)}).");
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(PlayerLineOutput playerLine)
    {
        playerLine.Length = length;
        playerLine.Color = new(R, G, B, A);
    }
}