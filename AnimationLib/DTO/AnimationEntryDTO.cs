using AnimationLib.Core.Elements;
using AnimationLib.Core.Utils;
using AnimationLib.Core;
using AnimationLib.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using TextureLib.Loader;
using DataPipes.DTO;
using DataPipes.DTO.Register;

namespace AnimationLib.DTO;
/// <summary>
/// Data Transfer Object (DTO) for the <see cref="AnimationEntry"/> class.
/// Used to serialize and deserialize animation state data, including frame paths,
/// animation playback speed, current frame index, and animation toggle.
/// </summary>
public class AnimationEntryDTO : IDTO<AnimationEntry>, IRegisterableDTO<AnimationEntryDTO, AnimationEntry>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    [JsonIgnore]
    private AnimationEntry animationEntry = new AnimationEntry();

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
    /// DTO class AnimationClip
    /// </summary>
    public AnimationClipDTO animationClipDTO = new();
    /// <summary>
    /// Gets the priority of this animation entry.
    /// Higher priority animations can override lower priority ones.
    /// </summary>
    public int Priority;

    /// <summary>
    /// Indicates whether this animation is currently playing.
    /// </summary>
    public bool IsPlaying;

    /// <summary>
    /// Gets the name of this animation entry.
    /// </summary>
    public string Name = string.Empty;




    /// <summary>
    /// Parameterless constructor required for deserialization and manual population of properties.
    /// </summary>
    public AnimationEntryDTO() { }
    /// <summary>
    /// Initializes the DTO using an existing <see cref="AnimationEntry"/> instance,
    /// preparing it for conversion to a serializable format.
    /// </summary>
    /// <param name="animationEntry">The source <see cref="AnimationEntry"/> object.</param>
    public AnimationEntryDTO(AnimationEntry animationEntry)
    {
        this.animationEntry = animationEntry;
        animationClipDTO = new AnimationClipDTO(animationEntry.Animation);
    }

    /// <summary>
    /// Converts the internal <see cref="FrameDTO"/> object to a DTO format
    /// by extracting primitive values and frame paths for serialization.
    /// </summary>
    public void ToDTO()
    {
        animationClipDTO?.ToDTO();

        Priority = animationEntry.Priority;
        IsPlaying = animationEntry.IsPlaying;
        Name = animationEntry.Name;
    }

    /// <summary>
    /// Creates a new instance of <see cref="AnimationEntryDTO"/> from the given <see cref="AnimationEntryDTO"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="AnimationEntryDTO"/>.</returns>
    public static AnimationEntryDTO CreateDTO(AnimationEntry obj) => new AnimationEntryDTO(obj);

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
    public AnimationEntry ToObject()
    {
        AnimationEntry animationEntry = new AnimationEntry(Name, animationClipDTO.ToObject(), Priority);
        animationEntry.IsPlaying = IsPlaying;

        return animationEntry;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(AnimationEntry animationEntry)
    {
        animationEntry.Animation = animationClipDTO.ToObject();

        animationEntry.Priority = Priority;
        animationEntry.IsPlaying = IsPlaying;
        animationEntry.Name = Name;
    }
}

