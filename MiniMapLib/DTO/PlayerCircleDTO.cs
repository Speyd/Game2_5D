using System.Text.Json.Serialization;
using System.Text.Json;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using MiniMapLib.ObjectInMap.Player;
using SFML.Graphics;

namespace MiniMapLib.DTO;
/// <summary>
/// Data Transfer Object (DTO) for the <see cref="PlayerCircleOutput"/> class.
/// Used to serialize and deserialize player circle settings including radius and color.
/// </summary>
public class PlayerCircleDTO : IDTO<PlayerCircleOutput>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    [JsonIgnore]
    private PlayerCircleOutput playerCircle;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public string BaseFileName { get; } = "playerCircle_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public string BaseExtension { get; } = ".json";


    /// <summary>
    /// The radius of the player's circle.
    /// </summary>
    public int radiusCircle;

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
    public PlayerCircleDTO() { }
    /// <summary>
    /// Initializes the DTO using an existing <see cref="PlayerCircleOutput"/> instance,
    /// preparing it for conversion to a serializable format.
    /// </summary>
    /// <param name="playerCircle">The source <see cref="PlayerCircleOutput"/> object.</param>
    public PlayerCircleDTO(PlayerCircleOutput playerCircle)
    {
        this.playerCircle = playerCircle;
    }

    /// <summary>
    /// Converts the internal <see cref="PlayerCircleOutput"/> object to a DTO format
    /// by extracting primitive values and frame paths for serialization.
    /// </summary>
    public void ToDTO()
    {
        radiusCircle = playerCircle.RadiusCircle;
        R = playerCircle.Color.R;
        G = playerCircle.Color.G;
        B = playerCircle.Color.B;
        A = playerCircle.Color.A;
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
    public PlayerCircleOutput ToObject()
    {
        throw new Exception($"This method has no implementation({typeof(PlayerCircleDTO)}).");
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(PlayerCircleOutput playerCircle)
    {
        playerCircle.Color = new Color(R,G,B,A);
        playerCircle.RadiusCircle = radiusCircle;
    }
}
