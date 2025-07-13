using ObstacleLib.TexturedWallLib;
using System.Text.Json.Serialization;
using ProtoRender.DTO;
using ProtoRender.Object;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using System.Text.Json;

namespace ObstacleLib.DTO;

/// <summary>
/// Data Transfer Object (DTO) for serializing and deserializing instances of <see cref="MultiWall"/>.
/// </summary>
[RegisterDTO(typeof(MultiWall))]
public class MultiWallDTO : IObjectDTO, IDTO<MultiWall>, IRegisterableDTO<MultiWallDTO, MultiWall>
{
    /// <summary>
    /// Gets the DTO registry associated with this DTO type.
    /// </summary>
    [JsonIgnore]
    public static new DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    /// <summary>
    /// Reference to the original <see cref="MultiWall"/> instance. Ignored during serialization.
    /// </summary>
    [JsonIgnore]
    private MultiWall multiWall;

    /// <summary>
    /// Gets the base filename used for serialization.
    /// </summary>
    [JsonIgnore]
    public override string BaseFileName { get; } = "multiWall_";

    /// <summary>
    /// Gets the base file extension used for serialization.
    /// </summary>
    [JsonIgnore]
    public override string BaseExtension { get; } = ".json";

    /// <summary>
    /// Contains serialized data related to the base Obstacle properties.
    /// </summary>
    public ObstacleDTO ObstacleDTO = new();

    /// <summary>
    /// Serialized list of wall levels that make up the <see cref="MultiWall"/>.
    /// </summary>
    public List<TexturedWallDTO> WallLevels { get; set; } = new();

    /// <summary>
    /// The base offset along the Z axis between wall levels.
    /// </summary>
    public double BaseOffset { get; set; }

    /// <summary>
    /// The size of one tile in world coordinates at the time of serialization.
    /// </summary>
    public override int TileWorld { get; set; }

    static MultiWallDTO()
    {
        ObstacleDTO.DtoTypeRegistry.GetType();
    }

    /// <summary>
    /// Default parameterless constructor used during deserialization.
    /// </summary>
    public MultiWallDTO() { }

    /// <summary>
    /// Constructs a new DTO instance from an existing <see cref="MultiWall"/> object.
    /// </summary>
    /// <param name="multiWall">The source MultiWall to serialize.</param>
    public MultiWallDTO(MultiWall multiWall)
    {
        this.multiWall = multiWall;
        ObstacleDTO = new ObstacleDTO(multiWall);
    }

    /// <summary>
    /// Serializes the <see cref="MultiWall"/> into its DTO representation.
    /// </summary>
    public override void ToDTO()
    {
        ObstacleDTO.ToDTO();
        TileWorld = ScreenLib.Screen.Setting.Tile;
        BaseOffset = multiWall.BaseOffset;

        foreach (var wall in multiWall.Walls)
        {
            var dto = new TexturedWallDTO(wall);
            dto.ToDTO();
            WallLevels.Add(dto);
        }
    }

    /// <summary>
    /// Creates a new <see cref="MultiWallDTO"/> from an <see cref="IObject"/>.
    /// </summary>
    /// <param name="obj">The source object to convert.</param>
    public static new IObjectDTO CreateDTO(IObject obj) => new MultiWallDTO((MultiWall)obj);

    /// <summary>
    /// Creates a new <see cref="MultiWallDTO"/> from a <see cref="MultiWall"/> instance.
    /// </summary>
    /// <param name="obj">The source object to convert.</param>
    public static MultiWallDTO CreateDTO(MultiWall obj) => new MultiWallDTO(obj);

    /// <summary>
    /// Registers this DTO type in the global registry.
    /// </summary>
    public static new void RegisterDTO() { }

    /// <summary>
    /// Registers JSON converters required to serialize and deserialize this DTO.
    /// </summary>
    /// <param name="options">The JSON serializer options to register against.</param>
    public static new void RegisterDTOJsonConverters(JsonSerializerOptions options)
    {
        DtoTypeRegistry.RegisterAllDTOJsonConverters(options);
        TexturedWallDTO.RegisterDTOJsonConverters(options);
    }

    /// <summary>
    /// Deserializes this DTO back into a <see cref="MultiWall"/> object.
    /// </summary>
    /// <returns>The restored <see cref="MultiWall"/> object.</returns>
    public override MultiWall ToObject()
    {
        var walls = new List<TexturedWall>();
        foreach (var wallDto in WallLevels)
        {
            var wall = wallDto.ToObject();
            walls.Add(wall);
        }

        MultiWall multi = new MultiWall(walls);
        ObstacleDTO.ToObject(multi, ObstacleDTO);
        multi.BaseOffset = BaseOffset;
        return multi;
    }

    /// <summary>
    /// Applies DTO data to an existing <see cref="IObject"/> instance.
    /// </summary>
    /// <param name="obj">The target object to populate.</param>
    public override void ToObject(IObject obj)
    {
        if (obj is MultiWall multi)
        {
            multi.Walls.Clear();

            foreach (var wallDto in WallLevels)
            {
                var wall = wallDto.ToObject();
                multi.Walls.Add(wall);
            }

            multi.BaseOffset = BaseOffset;
            ObstacleDTO.ToObject(multi, ObstacleDTO);
        }
    }

    /// <summary>
    /// Applies DTO data to an existing <see cref="MultiWall"/> instance.
    /// </summary>
    /// <param name="multi">The target MultiWall to populate.</param>
    public void ToObject(MultiWall multi)
    {
        multi.Walls.Clear();

        foreach (var wallDto in WallLevels)
        {
            var wall = wallDto.ToObject();
            multi.Walls.Add(wall);
        }

        multi.BaseOffset = BaseOffset;
        ObstacleDTO.ToObject(multi, ObstacleDTO);
    }
}
