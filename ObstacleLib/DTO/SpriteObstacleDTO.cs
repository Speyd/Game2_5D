using ObstacleLib.SpriteLib;
using System.Text.Json.Serialization;
using System.Text.Json;
using AnimationLib;
using DataPipes.DTO;
using ProtoRender.Object;
using ProtoRender.DTO;
using DataPipes.DTO.Register;

namespace ObstacleLib.DTO;
/// <summary>
/// Data Transfer Object (DTO) for serializing and deserializing instances of <see cref="SpriteObstacle"/>.
/// </summary>
/// <remarks>
/// This class encapsulates the necessary data to represent a <see cref="SpriteObstacle"/> for persistence or transmission,
/// including obstacle data, animation state, render properties like scale and distance, and tile conversion information.
/// It also supports reconstruction of the original <see cref="SpriteObstacle"/> object from its serialized form.
/// </remarks>
[RegisterDTO(typeof(SpriteObstacle))]
public class SpriteObstacleDTO : IObjectDTO, IDTO<SpriteObstacle>, IRegisterableDTO<SpriteObstacleDTO, SpriteObstacle>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static new DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    /// <summary>
    /// The internal reference to the original <see cref="SpriteObstacle"/> instance.
    /// Used for data extraction during DTO conversion.
    /// </summary>
    [JsonIgnore]
    private SpriteObstacle spriteObstacle;


    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public override string BaseFileName { get; } = "spriteObstacle_";
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
    /// Contains serialized animation state data for the sprite.
    /// </summary>
    public AnimationStateDTO AnimationState = new();


    /// <summary>
    /// Indicates whether the sprite should be rendered in the game world.
    /// </summary>
    public bool IsRenderable;
    /// <summary>
    /// The scale factor applied to the sprite when rendered.
    /// </summary>
    public float Scale;
    /// <summary>
    /// The angle between the observer and the sprite, used for perspective or visibility calculations.
    /// </summary>
    public double AngleToObserver;
    /// <summary>
    /// The distance from the observer to the sprite.
    /// </summary>
    public double Distance;
    /// <summary>
    /// Gets or sets the tile-to-world unit conversion factor used by the sprite.
    /// </summary>
    public override int TileWorld { get; set; }


    static SpriteObstacleDTO()
    {
        ObstacleDTO.DtoTypeRegistry.GetType();
    }
    /// <summary>
    /// Initializes a new empty instance of <see cref="SpriteObstacleDTO"/> and sets its static DTO type.
    /// </summary>
    public SpriteObstacleDTO(){}
    /// <summary>
    /// Initializes a new instance of <see cref="SpriteObstacleDTO"/> from a given <see cref="SpriteObstacle"/> instance.
    /// </summary>
    /// <param name="spriteObstacle">The sprite obstacle to convert into a DTO.</param>
    public SpriteObstacleDTO(SpriteObstacle spriteObstacle)
    {
        this.spriteObstacle = spriteObstacle;
        ObstacleDTO = new ObstacleDTO(spriteObstacle);
        AnimationState = new AnimationStateDTO(spriteObstacle.Animation);
    }


    /// <summary>
    /// Populates this DTO with data extracted from the original <see cref="SpriteObstacle"/> instance,
    /// including scale, angle, distance, and animation state.
    /// </summary>
    public override void ToDTO()
    {
        ObstacleDTO.ToDTO();
        AnimationState.ToDTO();

        Scale = spriteObstacle.Scale;
        AngleToObserver = spriteObstacle.AngleToObserver;
        Distance = spriteObstacle.Distance;
        TileWorld = ScreenLib.Screen.Setting.Tile;
    }
    /// <summary>
    /// Creates a new instance of <see cref="SpriteObstacleDTO"/> from the given <see cref="IObject"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="SpriteObstacleDTO"/>.</returns>
    public static new IObjectDTO CreateDTO(IObject obj) => new SpriteObstacleDTO((SpriteObstacle)obj);
    /// <summary>
    /// Creates a new instance of <see cref="SpriteObstacleDTO"/> from the given <see cref="IObject"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="SpriteObstacleDTO"/>.</returns>
    public static SpriteObstacleDTO CreateDTO(SpriteObstacle obj) => new SpriteObstacleDTO(obj);


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
        AnimationStateDTO.DtoTypeRegistry.RegisterAllDTOJsonConverters(options);
    }


    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public override SpriteObstacle ToObject()
    {
        SpriteObstacle sprite = new SpriteObstacle(AnimationState.ToObject());
        ObstacleDTO.ToObject(sprite, ObstacleDTO);

        return sprite;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public override void ToObject(IObject obj)
    {
        if (obj is SpriteObstacle sprite)
        {
            sprite = new SpriteObstacle(AnimationState.ToObject());
            ObstacleDTO.ToObject(sprite, ObstacleDTO);
        }
    }

    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(SpriteObstacle sprite)
    {
        sprite = new SpriteObstacle(AnimationState.ToObject());
        ObstacleDTO.ToObject(sprite, ObstacleDTO);
    }
}
