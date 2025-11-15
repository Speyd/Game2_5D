using System.Text.Json.Serialization;
using System.Text.Json;
using TextureLib.Loader.ImageProcessing;
using TextureLib.Loader.LoaderMode;
using DataPipes.DTO;
using DataPipes.DTO.Register;


namespace TextureLib.Loader;
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
    /// Optional settings for processing the image after it is loaded.
    /// Allows specifying filters, color adjustments, or other image processing options.
    /// If set to <c>null</c>, no additional processing will be applied.
    /// </summary>
    ImageProcessorOptions? ProcessorOptions { get; set; } = null;

    /// <summary>
    /// Specifies whether to use cached data when loading an image.
    /// If set to <c>true</c>, the loader will attempt to reuse previously loaded resources.
    /// </summary>
    public bool UseCashe { get; set; } = true;

    /// <summary>
    /// Determines whether a new image instance should be created even if a cached one exists.
    /// If set to <c>true</c>, a new image object is always created.
    /// </summary>
    public bool CreateNew { get; set; } = false;

    /// <summary>
    /// Specifies whether the image should be loaded asynchronously.
    /// If set to <c>true</c>, the loading will be performed on a separate thread.
    /// </summary>
    public bool LoadAsync { get; set; } = true;





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
        ProcessorOptions = options.ProcessorOptions;
        UseCashe = options.UseCashe;
        CreateNew = options.CreateNew;
        LoadAsync = options.LoadAsync;
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
        options.ProcessorOptions = options.ProcessorOptions;
        options.UseCashe = options.UseCashe;
        options.CreateNew = options.CreateNew;
        options.LoadAsync = options.LoadAsync;

        return options;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(ImageLoadOptions options)
    {
        options.ProcessorOptions = ProcessorOptions;
        options.UseCashe = UseCashe;
        options.CreateNew = CreateNew;
        options.LoadAsync = LoadAsync;
    }
}