using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataPipes.DTO.Register;
/// <summary>
/// Maintains a registry of DTO types and their corresponding domain object types.
/// This registry facilitates dynamic type resolution during JSON serialization and deserialization,
/// enabling automatic addition of the appropriate JSON converters to <see cref="JsonSerializerOptions"/>.
/// </summary>
public class DTOJsonOptionsRegistry
{
    /// <summary>
    /// A dictionary mapping DTO types to their associated domain object types.
    /// The key is the DTO type, and the value is the corresponding domain object type.
    /// </summary>
    public readonly Dictionary<Type, Type> RegisteredDtoPairs = new();

    /// <summary>
    /// Registers a DTO type and its associated domain object type in the registry.
    /// After registration, JSON converters for this DTO-object pair can be automatically
    /// added to serialization options to ensure correct (de)serialization.
    /// </summary>
    /// <typeparam name="TDto">The DTO type implementing <see cref="IDTO{TObject}"/> and <see cref="IRegisterableDTO{TDto, TObject}"/>.</typeparam>
    /// <typeparam name="TObject">The domain object type linked to the DTO.</typeparam>
    public void Register<TDto, TObject>()
        where TDto : class, IDTO<TObject>, IRegisterableDTO<TDto, TObject>
    {
        RegisteredDtoPairs[typeof(TDto)] = typeof(TObject);
    }

    /// <summary>
    /// Unregisters a previously registered DTO and domain object type pair from the registry.
    /// </summary>
    /// <typeparam name="TDto">The DTO type to remove.</typeparam>
    /// <typeparam name="TObject">The domain object type linked to the DTO.</typeparam>
    public void Unregister<TDto, TObject>()
        where TDto : class, IDTO<TObject>, IRegisterableDTO<TDto, TObject>
    {
        RegisteredDtoPairs.Remove(typeof(TDto));
    }

    /// <summary>
    /// Checks if a DTO and domain object type pair is registered in the registry.
    /// </summary>
    /// <typeparam name="TDto">The DTO type to check.</typeparam>
    /// <typeparam name="TObject">The domain object type to check against.</typeparam>
    /// <returns><c>true</c> if the pair is registered; otherwise, <c>false</c>.</returns>
    public bool IsRegistered<TDto, TObject>()
        where TDto : class, IDTO<TObject>, IRegisterableDTO<TDto, TObject>
    {
        return RegisteredDtoPairs.TryGetValue(typeof(TDto), out var objectType)
               && objectType == typeof(TObject);
    }
    /// <summary>
    /// Registers all missing JSON converters for the DTO-object type pairs stored in the registry.
    /// This ensures that each registered DTO type has its corresponding <see cref="DTOJsonConverter{TDto, TObject}"/> 
    /// added to the provided <see cref="JsonSerializerOptions.Converters"/> collection, if not already present.
    /// </summary>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to which the converters will be added.</param>
    public void RegisterAllDTOJsonConverters(JsonSerializerOptions options)
    {
        foreach (var reg in RegisteredDtoPairs)
        {
            var TDto = reg.Key;
            var TObject = reg.Value;
            var converterType = typeof(DTOJsonConverter<,>).MakeGenericType(TDto, TObject);

            if (!options.Converters.Any(c => c.GetType() == converterType))
            {
                var converterInstance = (JsonConverter)Activator.CreateInstance(converterType);
                options.Converters.Add(converterInstance);
            }
        }
    }
}
