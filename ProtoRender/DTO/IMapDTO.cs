using ProtoRender.Map;
using DataPipes.DTO;
using System.Text.Json.Serialization;
using System.Text.Json;
using DataPipes.DTO.Register;
using ProtoRender.Object;
using System;


namespace ProtoRender.DTO;
/// <summary>
/// Represents a Data Transfer Object (DTO) for serializing and deserializing map data implementing <see cref="IMap"/>.
/// </summary>
/// <remarks>
/// This class is used to persist and reconstruct the state of a map, including its dimensions and contained objects (such as obstacles).
/// It provides functionality to convert an <see cref="IMap"/> into a serializable form and back, supporting both synchronous
/// and asynchronous JSON serialization and deserialization.
/// </remarks>
public class IMapDTO : IDTO<IMap>, IRegisterableDTO<IMapDTO, IMap>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    [JsonIgnore]
    private IMap map;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public string BaseFileName { get; } = "iMap_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public string BaseExtension { get; } = ".json";


    /// <summary>
    /// A collection of data transfer objects representing all objects placed on the map,
    /// including obstacles and interactive entities.
    /// </summary>
    public List<IObjectDTO> Objects = new();

    /// <summary>
    /// Globally unique identifier for the object.
    /// </summary>
    public Guid UUID;
    /// <summary>
    /// The width of the map in world units or tiles, as defined in the original <see cref="IMap"/>.
    /// </summary>
    public int MapWidth;

    /// <summary>
    /// The height of the map in world units or tiles, as defined in the original <see cref="IMap"/>.
    /// </summary>
    public int MapHeight;

    /// <summary>
    /// Indicates whether the coordinates of deserialized objects should be reset to their default positions.
    /// </summary>
    /// <remarks>
    /// If <c>true</c>, object coordinates will be recalculated during deserialization; 
    /// otherwise, original saved coordinates will be preserved.
    /// </remarks>
    [JsonIgnore]
    public bool ResetObjectCoordinate { get; set; } = false;

    static IMapDTO()
    {
        RegistratorDTO<IObjectDTO, IObject>.EnsureInitialized();
        DtoTypeRegistry.Register<IObjectDTO, IObject>();
    }
    public IMapDTO() { }
    public IMapDTO(IMap map)
    {
        this.map = map;
    }

    public void ToDTO()
    {
        UUID = map.UUID;
        MapWidth = map.Setting.MapWidth;
        MapHeight = map.Setting.MapHeight;

        foreach (var kvp in map.Obstacles)
        {
            foreach (var obj in kvp.Value.Keys)
            {
                var dto = DTOFactory<IObjectDTO, IObject>.CreateFrom(obj);
                dto.ToDTO();
                Objects.Add(dto);
            }
        }
    }
    public static IMapDTO CreateDTO(IMap obj)
    {
        return new IMapDTO(obj);
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
        RegistratorDTO<IObjectDTO, IObject>.RegisterAllObjectDTOJsonConverters(options);
        DtoTypeRegistry.RegisterAllDTOJsonConverters(options);
    }


    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public IMap ToObject()
    {
        throw new NotImplementedException($"Cannot create an instance from interface '{typeof(IMapDTO).Name}'. " +
        $"The method '{nameof(ToObject)}' must be overridden in the concrete DTO class that implements '{typeof(IMapDTO).Name}'.");

    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(IMap map)
    {
        map.UUID = UUID;
        map.SetWidthMap(MapWidth);
        map.SetHeightMap(MapHeight);

        foreach (var obj in Objects)
        {
            var obstacle = obj.ToObject();
            map.AddObstacle(obstacle.CellX / obj.TileWorld, obstacle.CellY / obj.TileWorld, obstacle, ResetObjectCoordinate);
        }
    }
}

