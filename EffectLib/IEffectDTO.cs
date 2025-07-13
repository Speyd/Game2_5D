using System.Text.Json;
using System.Text.Json.Serialization;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using EffectLib.Effect;
using EffectLib.EffectCore;

namespace EffectLib;
/// <summary>
/// Defines an abstract base class for Data Transfer Objects (DTOs) that represent objects implementing <see cref="IEffect"/>.
/// Provides a standardized interface for serialization, deserialization, and mapping between domain models and their DTOs.
/// </summary>
public abstract class IEffectDTO : IDTO<IEffect>, IRegisterableDTO<IEffectDTO, IEffect>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    /// <summary>
    /// Gets the base file name used during JSON serialization (e.g., "object_", "dto_").
    /// </summary>
    public abstract string BaseFileName { get; }
    /// <summary>
    /// Gets the file extension used for JSON serialization (e.g., ".json").
    /// </summary>
    public abstract string BaseExtension { get; }



    /// <summary>
    /// Converts a domain object into its DTO representation by populating fields from the source model.
    /// </summary>
    public abstract void ToDTO();
    /// <summary>
    /// Creates a new instance of a DTO from the specified <see cref="IEffect"/> instance.
    /// Must be overridden by concrete DTO classes.
    /// </summary>
    /// <param name="obj">The source object to convert into a DTO.</param>
    /// <returns>A new instance of a class derived from <see cref="IEffectDTO"/>.</returns>
    public static IEffectDTO CreateDTO(IEffect obj)
    {
        throw new NotImplementedException($"The static method {nameof(CreateDTO)} must be overridden in the derived class {typeof(IEffectDTO).Name}.");
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
    public static void RegisterDTOJsonConverters(JsonSerializerOptions options) { }

    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public abstract IEffect ToObject();
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public abstract void ToObject(IEffect target);
}