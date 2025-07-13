using DataPipes.DTO.Register;
using EffectLib.EffectCore;
using DataPipes.DTO;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace EffectLib.Effect;
[RegisterDTO(typeof(CustomEffect))]
public class CustomEffectDTO : IEffectDTO, IDTO<CustomEffect>, IRegisterableDTO<CustomEffectDTO, CustomEffect>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static new DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();
    /// <summary>
    /// A reference to the original <see cref="CustomEffect"/> object. Ignored during serialization.
    /// </summary>
    [JsonIgnore]
    private CustomEffect effect;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public override string BaseFileName { get; } = "customEffect_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public override string BaseExtension { get; } = ".json";

    /// <summary>
    /// Gets or sets the effect color as a vector with RGBA components (values from 0 to 1).
    /// </summary>
    public virtual SFML.Graphics.Glsl.Vec4 EffectColor { get; set; } = new(1f, 1f, 1f, 1f);

    /// <summary>
    /// Gets or sets a value indicating whether to invert the effect intensity.
    /// </summary>
    public bool InvertEffect;

    /// <summary>
    /// Gets or sets the strength multiplier of the effect.
    /// </summary>
    public float EffectStrength;

    /// <summary>
    /// Gets or sets the softness or range of the effect transition.
    /// </summary>
    public float EffectRange;

    /// <summary>
    /// Gets or sets the start threshold for the effect's intensity.
    /// </summary>
    public float EffectStart;

    /// <summary>
    /// Gets or sets the end threshold for the effect's intensity.
    /// </summary>
    public float EffectEnd;

    public CustomEffectDTO() { }
    /// <summary>
    /// Constructs the DTO from an existing <see cref="CustomEffect"/> instance.
    /// </summary>
    /// <param name="effect">The source object to be serialized.</param>
    public CustomEffectDTO(CustomEffect effect)
    {
        this.effect = effect;
    }


    /// <summary>
    /// Populates this DTO with data extracted from the original <see cref="CustomEffect"/> instance,
    /// including scale, angle, distance, and animation state.
    /// </summary>
    public override void ToDTO()
    {
        EffectColor = effect.EffectColor;
        InvertEffect = effect.InvertEffect;
        EffectStrength = effect.EffectStrength;
        EffectRange = effect.EffectRange;
        EffectStart = effect.EffectStart;
        EffectEnd = effect.EffectEnd;
    }

    /// <summary>
    /// Creates a new instance of <see cref="CustomEffectDTO"/> from the given <see cref="IEffect"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="CustomEffectDTO"/>.</returns>
    public static new IEffectDTO CreateDTO(IEffect obj) => new CustomEffectDTO((CustomEffect)obj);
    /// <summary>
    /// Creates a new instance of <see cref="CustomEffectDTO"/> from the given <see cref="CustomEffect"/>.
    /// </summary>
    /// <param name="obj">The object to convert to a DTO.</param>
    /// <returns>A new instance of <see cref="CustomEffectDTO"/>.</returns>
    public static CustomEffectDTO CreateDTO(CustomEffect obj) => new CustomEffectDTO(obj);

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
        IEffectDTO.DtoTypeRegistry.RegisterAllDTOJsonConverters(options);
    }


    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public override CustomEffect ToObject()
    {
        CustomEffect effect = new CustomEffect();
        effect.EffectColor = EffectColor;
        effect.InvertEffect = InvertEffect;
        effect.EffectStrength = EffectStrength;
        effect.EffectRange = EffectRange;
        effect.EffectStart = EffectStart;
        effect.EffectEnd = EffectEnd;

        return effect;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public override void ToObject(IEffect obj)
    {
        obj.EffectColor = EffectColor;
        obj.InvertEffect = InvertEffect;
        obj.EffectStrength = EffectStrength;
        obj.EffectRange = EffectRange;
        obj.EffectStart = EffectStart;
        obj.EffectEnd = EffectEnd;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(CustomEffect effect)
    {
        effect.EffectColor = EffectColor;
        effect.InvertEffect = InvertEffect;
        effect.EffectStrength = EffectStrength;
        effect.EffectRange = EffectRange;
        effect.EffectStart = EffectStart;
        effect.EffectEnd = EffectEnd;
    }
}