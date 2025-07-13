using System.Text.Json.Serialization;
using System.Text.Json;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using PartsWorldLib.Up;
using ScreenLib.Output;
using TextureLib.Loader.ImageProcessing;
using System.Xml.Linq;

namespace PartsWorldLib.DTO;
/// <summary>
/// Data Transfer Object (DTO) for the <see cref="Sky"/> class.
/// Handles serialization and deserialization of sky-related properties,
/// including texture path, scrolling behavior, vertical pitch offsets, and rendering layer.
/// Implements <see cref="IUpPartDTO"/> for upward part integration,
/// <see cref="IDTO{Sky}"/> for generic DTO functionality,
/// and <see cref="IRegisterableDTO{SkyDTO, Sky}"/> for registration and type mapping.
/// </summary>
[RegisterDTO(typeof(Sky))]
public class SkyDTO : IUpPartDTO, IDTO<Sky>, IRegisterableDTO<SkyDTO, Sky>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static new DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();
    /// <summary>
    /// A reference to the original <see cref="SkyDTO"/> object. Ignored during serialization.
    /// </summary>
    [JsonIgnore]
    private Sky sky;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public override string BaseFileName { get; } = "sky_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public override string BaseExtension { get; } = ".json";

    /// <summary>
    /// Gets or sets the output rendering layer priority for this surface.
    /// </summary>
    public OutputPriorityType outputLayer;

    /// <summary>
    /// Gets or sets the path to the texture resource.
    /// </summary>
    public string texturePath = string.Empty;

    /// <summary>
    /// Multiplier that controls the horizontal scrolling speed of the sky texture.
    /// Higher values result in faster scrolling; lower values make it slower.
    /// </summary>
    public float scrollAngleMultiplier;

    /// <summary>
    /// The vertical pitch offset applied to the sky texture.
    /// Positive values shift the texture upward by default; negative values shift it downward.
    /// </summary>
    public float pitchVerticalOffset;

    /// <summary>
    /// The sensitivity of vertical texture movement in response to camera pitch.
    /// Higher values result in faster vertical scrolling of the texture.
    /// </summary>
    public float pitchVerticalShift;

    /// <summary>
    /// If set to <c>true</c>, vertical texture movement is disabled and the sky remains static vertically.
    /// </summary>
    public bool isStaticTexture;

    /// <summary>
    /// Gets or sets the image loading options used to configure how images are processed and loaded.
    /// </summary>
    public ImageLoadOptionsDTO LoadOptions;

    public SkyDTO() { }
    /// <summary>
    /// Constructs the DTO from an existing <see cref="Sky"/> instance.
    /// </summary>
    /// <param name="sky">The source object to be serialized.</param>
    public SkyDTO(Sky sky)
    {
        this.sky = sky;
        LoadOptions = new ImageLoadOptionsDTO(sky.LoadOptions);
    }


    /// <summary>
    /// Populates this DTO with data extracted from the original <see cref="Sky"/> instance,
    /// including scale, angle, distance, and animation state.
    /// </summary>
    public override void ToDTO()
    {
        LoadOptions.ToDTO();

        outputLayer = sky.OutputLayer;
        texturePath = sky.TexturePath;
        scrollAngleMultiplier = sky.ScrollAngleMultiplier;
        pitchVerticalOffset = sky.PitchVerticalOffset;
        pitchVerticalShift = sky.PitchVerticalShift;
        isStaticTexture = sky.IsStaticTexture;
    }

    /// <summary>
    /// Creates a new instance of <see cref="SkyDTO"/> from the given <see cref="IUpPart"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="SkyDTO"/>.</returns>
    public static new SkyDTO CreateDTO(IUpPart obj) => new SkyDTO((Sky)obj);
    /// <summary>
    /// Creates a new instance of <see cref="SkyDTO"/> from the given <see cref="Sky"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="SkyDTO"/>.</returns>
    public static SkyDTO CreateDTO(Sky obj) => new SkyDTO(obj);

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
        IUpPartDTO.DtoTypeRegistry.RegisterAllDTOJsonConverters(options);
        ImageLoadOptionsDTO.RegisterDTOJsonConverters(options);
    }


    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public override Sky ToObject()
    {
        Sky sky = new Sky(texturePath, LoadOptions.ToObject());
        sky.OutputLayer = outputLayer;
        sky.ScrollAngleMultiplier = scrollAngleMultiplier;
        sky.PitchVerticalOffset = pitchVerticalOffset;
        sky.PitchVerticalShift = pitchVerticalShift;
        sky.IsStaticTexture = isStaticTexture;

        return sky;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public override void ToObject(IUpPart obj)
    {
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(Sky sky)
    {
        sky.LoadOptions = LoadOptions.ToObject();

        sky.TexturePath = texturePath;
        sky.OutputLayer = outputLayer;
        sky.ScrollAngleMultiplier = scrollAngleMultiplier;
        sky.PitchVerticalOffset = pitchVerticalOffset;
        sky.PitchVerticalShift = pitchVerticalShift;
        sky.IsStaticTexture = isStaticTexture;
    }
}

