using System.Text.Json.Serialization;
using System.Text.Json;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using TextureLib.Loader.ImageProcessing;
using AnimationLib.Core;
using AnimationLib.Core.Elements;
using TextureLib.Loader;
using AnimationLib.Enum;


namespace AnimationLib.DTO;
/// <summary>
/// Data Transfer Object (DTO) for the <see cref="Frame"/> class.
/// Used to serialize and deserialize animation state data, including frame paths,
/// animation playback speed, current frame index, and animation toggle.
/// </summary>
public class FrameDTO : IDTO<Frame>, IRegisterableDTO<FrameDTO, Frame>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    [JsonIgnore]
    private Frame frame = new();

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public string BaseFileName { get; } = "frame_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public string BaseExtension { get; } = ".json";
    /// <summary>
    /// The index of the current frame in the animation sequence.
    /// Used to track which frame is currently being displayed.
    /// </summary>
    public int Index;

    /// <summary>
    /// The playback speed of the animation. 
    /// A higher value indicates slower animation playback (inverse relationship).
    /// </summary>
    public int SpeedAnimation;

    /// <summary>
    /// A list of file paths to the texture frames used in the animation.
    /// Represents the visual sequence of the animation.
    /// </summary>
    public List<string> Frames = new List<string>();

    /// <summary>
    /// Indicates whether the animation is active (true) or disabled (false).
    /// </summary>
    public PlayMode PlayMode;

    /// <summary>
    /// List DTO class ImageLoadOptions
    /// </summary>
    public ImageLoadOptionsDTO? loadOptionsDTO;

    /// <summary>
    /// Determines whether to use the maximum frame rectangle or the current frame's rectangle when rendering.
    /// </summary>
    public FrameRectMode RectMode;



    /// <summary>
    /// Parameterless constructor required for deserialization and manual population of properties.
    /// </summary>
    public FrameDTO() { }
    /// <summary>
    /// Initializes the DTO using an existing <see cref="Frame"/> instance,
    /// preparing it for conversion to a serializable format.
    /// </summary>
    /// <param name="frame">The source <see cref="Frame"/> object.</param>
    public FrameDTO(Frame frame)
    {
        this.frame = frame;
        loadOptionsDTO = frame.LoadOptions is not null ? new ImageLoadOptionsDTO(frame.LoadOptions) : null;
    }

    /// <summary>
    /// Converts the internal <see cref="FrameDTO"/> object to a DTO format
    /// by extracting primitive values and frame paths for serialization.
    /// </summary>
    public void ToDTO()
    {
        loadOptionsDTO?.ToDTO();

        Index = frame.Index;
        SpeedAnimation = frame.SpeedAnimation;
        RectMode = frame.RectMode;
        PlayMode = frame.PlayMode;

        foreach (var frame in frame.GetElements())
            Frames.Add(frame.PathTexture);

        Frames.Reverse();
    }

    /// <summary>
    /// Creates a new instance of <see cref="FrameDTO"/> from the given <see cref="FrameDTO"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="FrameDTO"/>.</returns>
    public static FrameDTO CreateDTO(Frame obj) => new FrameDTO(obj);

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
        ImageLoadOptionsDTO.RegisterDTOJsonConverters(options);
    }

    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public Frame ToObject()
    {
        Frames = ImageProcessor.RemoveDuplicateFrames(Frames);

        Frame frame = new Frame(loadOptionsDTO?.ToObject(), Frames.ToArray());
        frame.Index = Index;
        frame.SpeedAnimation = SpeedAnimation;
        frame.RectMode = RectMode;
        frame.PlayMode = PlayMode;

        return frame;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(Frame animationState)
    {
        animationState = new Frame(loadOptionsDTO?.ToObject(), Frames.ToArray());
        animationState.Index = Index;
        animationState.SpeedAnimation = SpeedAnimation;
        animationState.RectMode = RectMode;
        animationState.PlayMode = PlayMode;
    }
}
