using ObstacleLib.BlankWallLib;
using System.Text.Json.Serialization;
using System.Text.Json;
using DataPipes.DTO.Register;
using ProtoRender.Object;
using ProtoRender.DTO;
using DataPipes.DTO;



namespace ObstacleLib.DTO;
[RegisterDTO(typeof(BlankWall))]
public class BlankWallDTO : IObjectDTO, IDTO<BlankWall>, IRegisterableDTO<BlankWallDTO, BlankWall>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static new DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public override string BaseFileName { get; } = "blankWall_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public override string BaseExtension { get; } = ".json";

    /// <summary>
    /// The original BlankWall instance from which this DTO was created. Used during serialization.
    /// </summary>
    [JsonIgnore]
    private BlankWall blankWall;


    /// <summary>
    /// Data transfer object representation of the base obstacle properties.
    /// </summary>
    public ObstacleDTO ObstacleDTO = new();

    /// <summary>
    /// Red channel value of the fill color.
    /// </summary>
    public byte RFill;

    /// <summary>
    /// Green channel value of the fill color.
    /// </summary>
    public byte GFill;

    /// <summary>
    /// Blue channel value of the fill color.
    /// </summary>
    public byte BFill;

    /// <summary>
    /// Alpha (transparency) value of the fill color.
    /// </summary>
    public byte AFill;


    /// <summary>
    /// Gets or sets the tile size in the world. Used for rendering or scaling purposes.
    /// </summary>
    public override int TileWorld { get; set; }


    static BlankWallDTO()
    {
        ObstacleDTO.DtoTypeRegistry.GetType();
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="BlankWallDTO"/> class.
    /// </summary>
    public BlankWallDTO() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="BlankWallDTO"/> class from a <see cref="BlankWall"/> object.
    /// </summary>
    /// <param name="blankWall">The <see cref="BlankWall"/> instance to convert into a DTO.</param>
    public BlankWallDTO(BlankWall blankWall)
    {
        this.blankWall = blankWall;
        ObstacleDTO = new ObstacleDTO(blankWall);
    }


    /// <summary>
    /// Populates this DTO with data extracted from the original <see cref="BlankWall"/>
    /// </summary>
    public override void ToDTO()
    {
        ObstacleDTO.ToDTO();
        TileWorld = ScreenLib.Screen.Setting.Tile;

        RFill = blankWall.ColorFilling.R;
        GFill = blankWall.ColorFilling.G;
        BFill = blankWall.ColorFilling.B;
        AFill = blankWall.ColorFilling.A;
    }
    /// <summary>
    /// Creates a new instance of <see cref="BlankWallDTO"/> from the given <see cref="IObject"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="BlankWallDTO"/>.</returns>
    public static new IObjectDTO CreateDTO(IObject obj) => new BlankWallDTO((BlankWall)obj);
    /// <summary>
    /// Creates a new instance of <see cref="BlankWallDTO"/> from the given <see cref="IObject"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="BlankWallDTO"/>.</returns>
    public static BlankWallDTO CreateDTO(BlankWall obj) => new BlankWallDTO(obj);


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
    public override BlankWall ToObject()
    {
        BlankWall blankWall = new BlankWall(new SFML.Graphics.Color(RFill,GFill, BFill, AFill));
        ObstacleDTO.ToObject(blankWall, ObstacleDTO);

        return blankWall;
    }

    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public override void ToObject(IObject obj)
    {
        if (obj is BlankWall blankWall)
        {
            blankWall = new BlankWall(new SFML.Graphics.Color(RFill, GFill, BFill, AFill));
            ObstacleDTO.ToObject(blankWall, ObstacleDTO);
        }
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(BlankWall obj)
    {
        blankWall = new BlankWall(new SFML.Graphics.Color(RFill, GFill, BFill, AFill));
        ObstacleDTO.ToObject(blankWall, ObstacleDTO);
    }
}
