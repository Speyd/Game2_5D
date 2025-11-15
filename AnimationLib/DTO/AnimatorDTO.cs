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
using AnimationLib.Core;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using AnimationLib.Core.Utils;

namespace AnimationLib.DTO;
/// <summary>
/// Data Transfer Object (DTO) for the <see cref="Frame"/> class.
/// Used to serialize and deserialize animation state data, including frame paths,
/// animation playback speed, current frame index, and animation toggle.
/// </summary>
public class AnimatorDTO : IDTO<Animator>, IRegisterableDTO<AnimatorDTO, Animator>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    [JsonIgnore]
    private Animator animator = new();

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
    /// List DTO class AnimationEntry
    /// </summary>
    public List<AnimationEntryDTO> animationEntries = new();



    /// <summary>
    /// Parameterless constructor required for deserialization and manual population of properties.
    /// </summary>
    public AnimatorDTO() { }
    /// <summary>
    /// Initializes the DTO using an existing <see cref="Animator"/> instance,
    /// preparing it for conversion to a serializable format.
    /// </summary>
    /// <param name="animator">The source <see cref="Animator"/> object.</param>
    public AnimatorDTO(Animator animator)
    {
        this.animator = animator;
        animationEntries.AddRange(animator.GetAnimationEntries().Select(e => new AnimationEntryDTO(e)));
    }

    /// <summary>
    /// Converts the internal <see cref="FrameDTO"/> object to a DTO format
    /// by extracting primitive values and frame paths for serialization.
    /// </summary>
    public void ToDTO()
    {
        animationEntries.ForEach(e => e.ToDTO());
    }

    /// <summary>
    /// Creates a new instance of <see cref="AnimatorDTO"/> from the given <see cref="AnimatorDTO"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="AnimatorDTO"/>.</returns>
    public static AnimatorDTO CreateDTO(Animator obj) => new AnimatorDTO(obj);

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
    public Animator ToObject()
    {
        Animator animator = new Animator();
        animator.AddAnimations(animationEntries.Select(e => e.ToObject()).ToList());
       
        return animator;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(Animator animationState)
    {
        animator.AddAnimations(animationEntries.Select(e => e.ToObject()).ToList());
    }
}
