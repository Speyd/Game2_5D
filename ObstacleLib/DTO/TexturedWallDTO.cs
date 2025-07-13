using ObstacleLib.TexturedWallLib;
using System.Text.Json.Serialization;
using System.Text.Json;
using ProtoRender.DTO;
using TextureLib.Textures.Pair;
using ProtoRender.Object;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using TextureLib.Textures;
using SFML.Graphics;

namespace ObstacleLib.DTO;
/// <summary>
/// Data Transfer Object (DTO) for serializing and deserializing instances of <see cref="TexturedWall"/>.
/// </summary>
/// <remarks>
/// This class encapsulates data needed to save and restore a <see cref="TexturedWall"/> object,
/// including obstacle properties, texture references, and additional metadata such as level and tiling.
/// </remarks>
[RegisterDTO(typeof(TexturedWall))]
public class TexturedWallDTO : IObjectDTO, IDTO<TexturedWall>, IRegisterableDTO<TexturedWallDTO, TexturedWall>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static new DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    /// <summary>
    /// A reference to the original <see cref="TexturedWall"/> object. Ignored during serialization.
    /// </summary>
    [JsonIgnore]
    private TexturedWall texturedWall;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public override string BaseFileName { get; } = "texturedWall_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public override string BaseExtension { get; } = ".json";


    /// <summary>
    /// Contains serialized data related to the obstacle's basic properties.
    /// </summary>
    public ObstacleDTO ObstacleDTO = new();
    /// <summary>
    /// Gets or sets the DTO instance representing a <see cref="MultiSideTexture"/>.
    /// Contains per-side and shared texture data along with image loading options.
    /// </summary>

    public MultiSideTextureDTO MultiSide = new();

    /// <summary>
    /// The wall level information used for rendering or game logic.
    /// </summary>
    public int LvlWall;


    /// <summary>
    /// The size of a tile in world units at the time of serialization.
    /// </summary>
    public override int TileWorld { get; set; }

    static TexturedWallDTO()
    {
        ObstacleDTO.DtoTypeRegistry.GetType();
    }
    /// <summary>
    /// Default parameterless constructor used during deserialization.
    /// </summary>
    public TexturedWallDTO(){}
    /// <summary>
    /// Constructs the DTO from an existing <see cref="TexturedWall"/> instance.
    /// </summary>
    /// <param name="texturedWall">The source object to be serialized.</param>
    public TexturedWallDTO(TexturedWall texturedWall)
    {
        this.texturedWall = texturedWall;
        ObstacleDTO = new ObstacleDTO(texturedWall);
        MultiSide = new(texturedWall.MultiSide);
    }


    /// <summary>
    /// Populates this DTO with data extracted from the original <see cref="TexturedWall"/> instance,
    /// including scale, angle, distance, and animation state.
    /// </summary>
    public override void ToDTO()
    {
        ObstacleDTO.ToDTO();
        MultiSide.ToDTO();

        LvlWall = texturedWall.LvlWall;     
        TileWorld = ScreenLib.Screen.Setting.Tile;
    }
    /// <summary>
    /// Creates a new instance of <see cref="TexturedWallDTO"/> from the given <see cref="IObject"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="TexturedWallDTO"/>.</returns>
    public static new IObjectDTO CreateDTO(IObject obj) => new TexturedWallDTO((TexturedWall)obj);
    /// <summary>
    /// Creates a new instance of <see cref="TexturedWallDTO"/> from the given <see cref="IObject"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="TexturedWallDTO"/>.</returns>
    public static TexturedWallDTO CreateDTO(TexturedWall obj) => new TexturedWallDTO(obj);


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
        IObjectDTO.DtoTypeRegistry.RegisterAllDTOJsonConverters(options);
        ObstacleDTO.DtoTypeRegistry.RegisterAllDTOJsonConverters(options);
    }


    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public override TexturedWall ToObject()
    {
        TexturedWall wall = new TexturedWall();
        wall.LvlWall = LvlWall;
        wall.MultiSide = MultiSide.ToObject();

        ObstacleDTO.ToObject(wall, ObstacleDTO);

        return wall;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public override void ToObject(IObject obj)
    {
        if (obj is TexturedWall wall)
        {
            wall = new TexturedWall();
            wall.LvlWall = LvlWall;
            wall.MultiSide = MultiSide.ToObject();

            ObstacleDTO.ToObject(wall, ObstacleDTO);
        }
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(TexturedWall wall)
    {
        wall.LvlWall = LvlWall;
        wall.MultiSide = MultiSide.ToObject();

        ObstacleDTO.ToObject(wall, ObstacleDTO);
    }
}
