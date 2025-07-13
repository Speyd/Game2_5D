using DataPipes.DTO;
using DataPipes.DTO.Register;
using System.Text.Json.Serialization;
using System.Text.Json;
using TextureLib.Loader.LoaderMode;


namespace TextureLib.Loader.ImageProcessing;
/// <summary>
/// Data Transfer Object (DTO) for <see cref="ImageLoadOptions"/>, 
/// used to transfer image loading configuration data between layers.
/// Implements the <see cref="IDTO{T}"/> interface for conversion to the domain model.
/// </summary>
public class ImageLoadOptionsDTO : IDTO<ImageLoadOptions>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    [JsonIgnore]
    private ImageLoadOptions options;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public string BaseFileName { get; } = "imageLoadOptions_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public string BaseExtension { get; } = ".json";



    /// <summary>
    /// Flags specifying which color channels to filter during processing.
    /// </summary>
    public ColorChannelFilter ColorChannelFilter;
    /// <summary>
    /// Mode specifying how color replacement should be applied (per channel or all channels).
    /// </summary>
    public ColorReplaceMode ColorReplaceMode;
    /// <summary>
    /// Mode specifying how frames are loaded (full frame, accumulate, or none).
    /// </summary>
    public FrameLoadMode FrameLoadMode;

    /// <summary>
    /// Start color range for filtering pixels during color replacement.
    /// </summary>
    public SFML.Graphics.Color StartFilterRGBA;
    /// <summary>
    /// End color range for filtering pixels during color replacement.
    /// </summary>
    public SFML.Graphics.Color EndFilterRGBA;
    /// <summary>
    /// Base color used for replacing filtered pixel colors.
    /// </summary>
    public SFML.Graphics.Color BaseReplaceColor;


    /// <summary>
    /// String representing which color channels to load (e.g., "RGBA").
    /// </summary>
    public string LoadSettingMapping { get; set; } = "RGBA";



    /// <summary>
    /// Parameterless constructor.
    /// </summary>
    public ImageLoadOptionsDTO()
    { }
    public ImageLoadOptionsDTO(ImageLoadOptions options)
    {
        this.options = options;
    }

    /// <summary>
    /// Converts a HitBoxSide instance into this DTO representation.
    /// </summary>
    public void ToDTO()
    {
        ColorChannelFilter = options.ColorChannelFilter;
        ColorReplaceMode = options.ColorReplaceMode;
        FrameLoadMode = options.FrameLoadMode;
        StartFilterRGBA = options.StartFilterRGBA;
        EndFilterRGBA = options.EndFilterRGBA;
        BaseReplaceColor = options.BaseReplaceColor;
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
    public ImageLoadOptions ToObject()
    {
        var options = new ImageLoadOptions();
        options.ColorChannelFilter = ColorChannelFilter;
        options.ColorReplaceMode = ColorReplaceMode;
        options.FrameLoadMode = FrameLoadMode;
        options.StartFilterRGBA = StartFilterRGBA;
        options.EndFilterRGBA = EndFilterRGBA;
        options.BaseReplaceColor = BaseReplaceColor;

        return options;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(ImageLoadOptions options)
    {
        options.ColorChannelFilter = ColorChannelFilter;
        options.ColorReplaceMode = ColorReplaceMode;
        options.FrameLoadMode = FrameLoadMode;
        options.StartFilterRGBA = StartFilterRGBA;
        options.EndFilterRGBA = EndFilterRGBA;
        options.BaseReplaceColor = BaseReplaceColor;
    }
}