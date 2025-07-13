using System.Text.Json.Serialization;
using System.Text.Json;
using HitBoxLib.Segment.DTOs;
using TextureLib.Textures;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using EffectLib;
using EffectLib.EffectCore;
using ProtoRender.DTO;


namespace ObstacleLib.DTO;
public class ObstacleDTO : IDTO<Obstacle>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    [JsonIgnore]
    private Obstacle obstacle;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public string BaseFileName { get; } = "obstacle_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public string BaseExtension { get; } = ".json";


    /// <summary>
    /// Globally unique identifier for the object.
    /// </summary>
    public Guid UUID;

    /// <summary>
    /// Represents the hitbox information of the obstacle.
    /// </summary>
    public HitBoxDTO HitBox;

    /// <summary>
    /// The X position of the obstacle.
    /// </summary>
    public double X;

    /// <summary>
    /// The Y position of the obstacle.
    /// </summary>
    public double Y;

    /// <summary>
    /// The Z position of the obstacle.
    /// </summary>
    public double Z;

    /// <summary>
    /// The X coordinate of the obstacle in the grid cell system.
    /// </summary>
    public int CellX;

    /// <summary>
    /// The Y coordinate of the obstacle in the grid cell system.
    /// </summary>
    public int CellY;

    /// <summary>
    /// Red channel value of the minimap color.
    /// </summary>
    public byte RMap;

    /// <summary>
    /// Green channel value of the minimap color.
    /// </summary>
    public byte GMap;

    /// <summary>
    /// Blue channel value of the minimap color.
    /// </summary>
    public byte BMap;

    /// <summary>
    /// Alpha channel value of the minimap color.
    /// </summary>
    public byte AMap;

    /// <summary>
    /// File path to the texture used for representing the obstacle in the minimap.
    /// </summary>
    public string TextureInMiniMap = "";

    /// <summary>
    /// Multiplier for obstacle size.
    /// </summary>
    public float SizeScale;

    /// <summary>
    /// Multiplier for obstacle position.
    /// </summary>
    public float PositionScale;

    /// <summary>
    /// Indicates whether the obstacle can be passed through.
    /// </summary>
    public bool IsPassability;

    /// <summary>
    /// If true, the obstacle ignores collisions with the main hitbox.
    /// </summary>
    public bool IgnoreCollisonMainBox;

    /// <summary>
    /// Determines if the obstacle can be added as a single instance.
    /// </summary>
    public bool IsSingleAddable;
    /// <summary>
    /// Contains serialized data related to the effect's basic properties.
    /// </summary>
    public IEffectDTO? IEffectDTO;

    static ObstacleDTO()
    {
        RegistratorDTO<IEffectDTO, IEffect>.EnsureInitialized();
        DtoTypeRegistry.Register<IEffectDTO, IEffect>();
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ObstacleDTO"/> class.
    /// </summary>
    public ObstacleDTO(){}
    /// <summary>
    /// Initializes a new instance of the <see cref="ObstacleDTO"/> class with the given <see cref="Obstacle"/>.
    /// </summary>
    /// <param name="obstacle">The obstacle to wrap in the DTO.</param>
    public ObstacleDTO(Obstacle obstacle)
    {
        this.obstacle = obstacle;
        HitBox = new HitBoxDTO(obstacle.HitBox);
        IEffectDTO = obstacle.Effect is not null? DTOFactory<IEffectDTO, IEffect>.CreateFrom(obstacle.Effect): null;
    }


    /// <summary>
    /// Populates the DTO fields from the associated obstacle object.
    /// </summary>
    public void ToDTO()
    {
        UUID = obstacle.UUID;

        HitBox.ToDTO();
        IEffectDTO?.ToDTO();

        X = obstacle.X.Axis;
        Y = obstacle.Y.Axis;
        Z = obstacle.Z.Axis;

        CellX = obstacle.CellX;
        CellY = obstacle.CellY;

        RMap = obstacle.ColorInMap.R;
        GMap = obstacle.ColorInMap.G;
        BMap = obstacle.ColorInMap.B;
        AMap = obstacle.ColorInMap.A;
        TextureInMiniMap = obstacle.TextureInMiniMap?.PathTexture ?? "";

        SizeScale = obstacle.SizeScale;
        PositionScale = obstacle.PositionScale;

        IsPassability = obstacle.IsPassability;
        IgnoreCollisonMainBox = obstacle.IgnoreCollisonMainBox;
        IsSingleAddable = obstacle.IsSingleAddable;
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
        RegistratorDTO<IEffectDTO, IEffect>.RegisterAllObjectDTOJsonConverters(options);
        DtoTypeRegistry.RegisterAllDTOJsonConverters(options);
    }


    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public Obstacle ToObject()
    {
        throw new NotImplementedException($"Cannot create an instance from abstract class '{typeof(ObstacleDTO).Name}'. " +
        $"The method '{nameof(ToObject)}' must be overridden in the concrete DTO class that implements '{typeof(ObstacleDTO).Name}'.");
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(Obstacle obstacle)
    {
        obstacle.UUID = UUID;
        obstacle.HitBox = HitBoxDTO.ToObject(HitBox);
        obstacle.Effect = IEffectDTO?.ToObject();

        obstacle.X.Axis = X;
        obstacle.Y.Axis = Y;
        obstacle.Z.Axis = Z;

        obstacle.CellX = CellX;
        obstacle.CellY = CellY;

        obstacle.ColorInMap = new SFML.Graphics.Color(RMap, GMap, BMap, AMap);
        obstacle.TextureInMiniMap = string.IsNullOrEmpty(TextureInMiniMap) ? null : new TextureWrapper(TextureInMiniMap, true);

        obstacle.SizeScale = SizeScale;
        obstacle.PositionScale = PositionScale;

        obstacle.IsPassability = IsPassability;
        obstacle.IgnoreCollisonMainBox = IgnoreCollisonMainBox;
        obstacle.IsSingleAddable = IsSingleAddable;
    }

    /// <summary>
    /// Populates an existing <see cref="Obstacle"/> instance using data from the provided <see cref="ObstacleDTO"/>.
    /// </summary>
    /// <param name="obstacle">The <see cref="Obstacle"/> instance to populate.</param>
    /// <param name="dto">The <see cref="ObstacleDTO"/> containing the serialized obstacle data.</param>
    /// <returns>The updated <see cref="Obstacle"/> instance with values copied from the DTO.</returns>
    /// <remarks>
    /// This method deserializes position, hitbox, color, texture, scale, and logical flags such as passability and collision behavior.
    /// It is used to reconstruct a runtime <see cref="Obstacle"/> object from serialized DTO data.
    /// </remarks>
    public static Obstacle ToObject(Obstacle obstacle, ObstacleDTO dto)
    {
        obstacle.UUID = dto.UUID;
        obstacle.HitBox = HitBoxDTO.ToObject(dto.HitBox);
        obstacle.Effect = dto.IEffectDTO?.ToObject();

        obstacle.X.Axis = dto.X;
        obstacle.Y.Axis = dto.Y;
        obstacle.Z.Axis = dto.Z;

        obstacle.CellX = dto.CellX;
        obstacle.CellY = dto.CellY;

        obstacle.ColorInMap = new SFML.Graphics.Color(dto.RMap, dto.GMap, dto.BMap, dto.AMap);
        obstacle.TextureInMiniMap = string.IsNullOrEmpty(dto.TextureInMiniMap) ? null : new TextureWrapper(dto.TextureInMiniMap, true);

        obstacle.SizeScale = dto.SizeScale;
        obstacle.PositionScale = dto.PositionScale;

        obstacle.IsPassability = dto.IsPassability;
        obstacle.IgnoreCollisonMainBox = dto.IgnoreCollisonMainBox;
        obstacle.IsSingleAddable = dto.IsSingleAddable;

        return obstacle;
    }
}