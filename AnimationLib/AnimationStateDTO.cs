using System.Text.Json.Serialization;
using System.Text.Json;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using TextureLib.Loader.ImageProcessing;


namespace AnimationLib;
/// <summary>
/// Data Transfer Object (DTO) for the <see cref="AnimationState"/> class.
/// Used to serialize and deserialize animation state data, including frame paths,
/// animation playback speed, current frame index, and animation toggle.
/// </summary>
public class AnimationStateDTO : IDTO<AnimationState>, IRegisterableDTO<AnimationStateDTO, AnimationState>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    [JsonIgnore]
    private AnimationState animationState = new();

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public string BaseFileName { get; } = "animationState_";
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
    public int Speed;

    /// <summary>
    /// A list of file paths to the texture frames used in the animation.
    /// Represents the visual sequence of the animation.
    /// </summary>
    public List<string> Frames = new List<string>();

    /// <summary>
    /// Indicates whether the animation is active (true) or disabled (false).
    /// </summary>
    public bool IsAnimation;

    public ImageLoadOptionsDTO loadOptionsDTO;

    /// <summary>
    /// Parameterless constructor required for deserialization and manual population of properties.
    /// </summary>
    public AnimationStateDTO() { }
    /// <summary>
    /// Initializes the DTO using an existing <see cref="AnimationState"/> instance,
    /// preparing it for conversion to a serializable format.
    /// </summary>
    /// <param name="animationState">The source <see cref="AnimationState"/> object.</param>
    public AnimationStateDTO(AnimationState animationState)
    {
        this.animationState = animationState;
        loadOptionsDTO = new ImageLoadOptionsDTO(animationState.LoadOptions);
    }

    /// <summary>
    /// Converts the internal <see cref="AnimationState"/> object to a DTO format
    /// by extracting primitive values and frame paths for serialization.
    /// </summary>
    public void ToDTO()
    {
        loadOptionsDTO.ToDTO();

        Index = animationState.Index;
        Speed = animationState.Speed;
        IsAnimation = animationState.IsAnimation;

        foreach (var frame in animationState.GetFrames())
            Frames.Add(frame.PathTexture);

        Frames.Reverse();
    }

    /// <summary>
    /// Creates a new instance of <see cref="AnimationStateDTO"/> from the given <see cref="AnimationState"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="AnimationStateDTO"/>.</returns>
    public static AnimationStateDTO CreateDTO(AnimationState obj) => new AnimationStateDTO(obj);

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
    public AnimationState ToObject()
    {
        Frames = ImageProcessor.RemoveDuplicateFrames(Frames);

        AnimationState animationState = new AnimationState(loadOptionsDTO.ToObject(), true, Frames.ToArray());
        animationState.Index = Index;
        animationState.Speed = Speed;
        animationState.IsAnimation = IsAnimation;

        return animationState;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(AnimationState animationState)
    {
        animationState = new AnimationState(loadOptionsDTO.ToObject(), true, Frames.ToArray());
        animationState.Index = Index;
        animationState.Speed = Speed;
        animationState.IsAnimation = IsAnimation;
    }
}
