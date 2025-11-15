using AnimationLib.Core.Elements;
using AnimationLib.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using TextureLib.Loader;
using DataPipes.DTO.Register;
using DataPipes.DTO;
using NGenerics.Extensions;
using AnimationLib.Selector;

namespace AnimationLib.DTO;
/// <summary>
/// Data Transfer Object (DTO) for the <see cref="AnimationClip"/> class.
/// Used to serialize and deserialize animation state data, including frame paths,
/// animation playback speed, current frame index, and animation toggle.
/// </summary>
public class AnimationClipDTO : IDTO<AnimationClip>, IRegisterableDTO<AnimationClipDTO, AnimationClip>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    [JsonIgnore]
    private AnimationClip animationClip = new();

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public string BaseFileName { get; } = "animationClip_";
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
    /// Indicates whether the animation is active (true) or disabled (false).
    /// </summary>
    public PlayMode PlayMode;


    /// <summary>
    /// Optional base frame selector for determining which frame to display.
    /// </summary>
    public IElementSelector? BaseSelector { get; set; } = null;

    /// <summary>
    /// Optional frame selector used to refine frame selection within the current frame.
    /// </summary>
    public IElementSelector? FrameSelector { get; set; } = null;

    /// <summary>
    /// DTO class Frame
    /// </summary>

    public List<FrameDTO> frameDTO = new();

    /// <summary>
    /// Parameterless constructor required for deserialization and manual population of properties.
    /// </summary>
    public AnimationClipDTO() { }
    /// <summary>
    /// Initializes the DTO using an existing <see cref="AnimationClip"/> instance,
    /// preparing it for conversion to a serializable format.
    /// </summary>
    /// <param name="animationClip">The source <see cref="AnimationClip"/> object.</param>
    public AnimationClipDTO(AnimationClip animationClip)
    {
        this.animationClip = animationClip;
        frameDTO.AddRange(animationClip.GetElements().Select(e => new FrameDTO(e)));
    }

    /// <summary>
    /// Converts the internal <see cref="AnimationClipDTO"/> object to a DTO format
    /// by extracting primitive values and frame paths for serialization.
    /// </summary>
    public void ToDTO()
    {
        frameDTO.ForEach(f => f.ToDTO());

        Index = animationClip.Index;
        SpeedAnimation = animationClip.SpeedAnimation;
        PlayMode = animationClip.PlayMode;
        BaseSelector = animationClip.BaseSelector;
        FrameSelector = animationClip.FrameSelector;
    }

    /// <summary>
    /// Creates a new instance of <see cref="AnimationClipDTO"/> from the given <see cref="AnimationClipDTO"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="AnimationClipDTO"/>.</returns>
    public static AnimationClipDTO CreateDTO(AnimationClip obj) => new AnimationClipDTO(obj);

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
        FrameDTO.RegisterDTOJsonConverters(options);
        ImageLoadOptionsDTO.RegisterDTOJsonConverters(options);
    }

    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public AnimationClip ToObject()
    {
        AnimationClip animationClip = new AnimationClip();
        frameDTO.ForEach(f => animationClip.AddElement(f.ToObject()));

        animationClip.Index = Index;
        animationClip.SpeedAnimation = SpeedAnimation;
        animationClip.PlayMode = PlayMode;
        animationClip.BaseSelector = BaseSelector;
        animationClip.FrameSelector = FrameSelector;

        return animationClip;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(AnimationClip animationState)
    {
        frameDTO.ForEach(f => animationClip.AddElement(f.ToObject()));

        animationClip.Index = Index;
        animationClip.SpeedAnimation = SpeedAnimation;
        animationClip.PlayMode = PlayMode;
        animationClip.BaseSelector = BaseSelector;
        animationClip.FrameSelector = FrameSelector;
    }
}