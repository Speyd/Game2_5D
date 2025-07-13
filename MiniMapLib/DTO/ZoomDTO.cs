using System.Text.Json.Serialization;
using System.Text.Json;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using MiniMapLib.Window;
using SFML.Graphics;

namespace MiniMapLib.DTO;
/// <summary>
/// Data Transfer Object (DTO) for the <see cref="ZoomMiniMap"/> class.
/// Used to serialize and deserialize zoom state of the minimap.
/// </summary>
public class ZoomDTO : IDTO<ZoomMiniMap>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    [JsonIgnore]
    private ZoomMiniMap zoom;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public string BaseFileName { get; } = "zoom_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public string BaseExtension { get; } = ".json";


    /// <summary>
    /// The minimum zoom level of the minimap.
    /// </summary>
    public float minZoom;

    /// <summary>
    /// The maximum zoom level of the minimap.
    /// </summary>
    public float maxZoom;

    /// <summary>
    /// The current zoom level of the minimap.
    /// </summary>
    public float currentZoom;

    /// <summary>
    /// Indicates whether the minimap is currently zooming.
    /// </summary>
    public bool isZooming;


    /// <summary>
    /// Parameterless constructor required for deserialization and manual population of properties.
    /// </summary>
    public ZoomDTO() { }
    /// <summary>
    /// Initializes the DTO using an existing <see cref="ZoomMiniMap"/> instance,
    /// preparing it for conversion to a serializable format.
    /// </summary>
    /// <param name="zoom">The source <see cref="ZoomMiniMap"/> object.</param>
    public ZoomDTO(ZoomMiniMap zoom)
    {
        this.zoom = zoom;
    }

    /// <summary>
    /// Converts the internal <see cref="ZoomMiniMap"/> object to a DTO format
    /// by extracting primitive values and frame paths for serialization.
    /// </summary>
    public void ToDTO()
    {
        minZoom = zoom.MinZoom;
        maxZoom = zoom.MaxZoom;
        currentZoom = zoom.Zoom;
        isZooming = zoom.IsZooming;
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
    public ZoomMiniMap ToObject()
    {
        throw new Exception($"This method has no implementation({typeof(ZoomDTO)}).");
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(ZoomMiniMap zoom)
    {
        zoom.MinZoom = minZoom;
        zoom.MaxZoom = maxZoom;
        zoom.Zoom = currentZoom;
        zoom.IsZooming = isZooming;
    }
}