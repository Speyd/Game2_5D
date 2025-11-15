using MiniMapLib.ObjectInMap.Player;
using MiniMapLib.ObjectInMap.Positions;
using MiniMapLib.Setting;
using System.Text.Json.Serialization;
using System.Text.Json;
using DataPipes.DTO.Register;
using DataPipes.DTO;
using SFML.System;

namespace MiniMapLib.DTO;
public class SettingWindowDTO : IDTO<SettingWindow>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    [JsonIgnore]
    private SettingWindow setting;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public string BaseFileName { get; } = "settingWindow_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public string BaseExtension { get; } = ".json";

    /// <summary>
    /// Horizontal scaling factor of the map.
    /// </summary>
    public float ScaleX;

    /// <summary>
    /// Vertical scaling factor of the map.
    /// </summary>
    public float ScaleY;

    /// <summary>
    /// Positions of key elements on the minimap.
    /// </summary>
    public PositionsMiniMap Positions;

    /// <summary>
    /// Coordinates of the minimap in the window.
    /// </summary>
    public Vector2f CoordinatesInWindow { get; set; }



    /// <summary>
    /// Parameterless constructor required for deserialization and manual population of properties.
    /// </summary>
    public SettingWindowDTO() { }
    /// <summary>
    /// Initializes the DTO using an existing <see cref="Setting"/> instance,
    /// preparing it for conversion to a serializable format.
    /// </summary>
    /// <param name="setting">The source <see cref="Setting"/> object.</param>
    public SettingWindowDTO(SettingWindow? setting)
    {
        if(setting is null)
            throw new ArgumentNullException($"Object ({nameof(setting)}) in ({typeof(SettingWindowDTO)}) is null");

        this.setting = setting;
    }


    /// <summary>
    /// Converts the internal <see cref="PlayerCircleOutput"/> object to a DTO format
    /// by extracting primitive values and frame paths for serialization.
    /// </summary>
    public void ToDTO()
    {
        ScaleX = setting.ScaleX;
        ScaleY = setting.ScaleY;

        Positions = setting.Positions;
        CoordinatesInWindow = setting.CoordinatesInWindow;
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
    public SettingWindow ToObject()
    {
        throw new Exception($"This method has no implementation({typeof(SettingWindowDTO)}).");
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(SettingWindow? setting)
    {
        if (setting is null)
            return;

        setting.ScaleX = ScaleX;
        setting.ScaleY = ScaleY;

        Positions = setting.Positions;
        CoordinatesInWindow = setting.CoordinatesInWindow;
    }
}