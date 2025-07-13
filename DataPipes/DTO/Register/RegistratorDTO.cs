using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace DataPipes.DTO.Register;
/// <summary>
/// Responsible for scanning assemblies to find and register DTO types implementing <typeparamref name="TDto"/>
/// with their associated model types <typeparamref name="TObject"/> based on the <see cref="RegisterDTOAttribute"/>.
/// </summary>
/// <typeparam name="TDto">The DTO type that implements <see cref="IDTO{TObject}"/> and <see cref="IRegisterableDTO{TDto, TObject}"/>.</typeparam>
/// <typeparam name="TObject">The model type from which DTOs are created.</typeparam>
/// <remarks>
/// This class ensures initialization happens only once per app domain, and registers:
/// - Deserialization delegates keyed by the DTO's static type identifier,
/// - Factory methods to create DTO instances from model objects.
///
/// It leverages reflection to find DTO classes marked with <see cref="RegisterDTOAttribute"/>,
/// then uses their static properties and methods defined by <see cref="IRegisterableDTO{TDto, TObject}"/>.
/// </remarks>
public static class RegistratorDTO<TDto, TObject>
    where TDto : class, IDTO<TObject>, IRegisterableDTO<TDto, TObject>
{
    private static bool _initialized = false;

    private static readonly List<(Type dto, Type obj)> _registeredTypes = new();
    public static IReadOnlyList<(Type dto, Type obj)> RegisteredTypes => _registeredTypes.AsReadOnly();



    /// <summary>
    /// Scans all loaded assemblies to find and register DTO types implementing <typeparamref name="TDto"/>.
    /// </summary>
    /// <remarks>
    /// This method performs registration only once per application domain.
    /// It discovers DTO classes marked with <see cref="RegisterDTOAttribute"/>,
    /// then registers deserializers and factory methods accordingly.
    /// </remarks>
    public static void EnsureInitialized()
    {
        if (_initialized)
            return;

        _initialized = true;

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).ToArray()!;
            }

            var dtoTypes = types
                .Where(t =>
                    t != null &&
                    typeof(TDto).IsAssignableFrom(t) &&
                    typeof(IDTO<TObject>).IsAssignableFrom(t) &&
                    t.GetCustomAttribute<RegisterDTOAttribute>() != null);

            foreach (var dtoType in dtoTypes)
            {
                bool converter = false;
                bool factory = false;

                var attr = dtoType.GetCustomAttribute<RegisterDTOAttribute>();
                var modelType = attr?.ModelType;

                var dtoTypeName = (dtoType)?.Name;
                string methodName = IRegisterableDTO<TDto, TObject>.CreateDTOPropertyName;
                var createMethod = dtoType?.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(m => m.Name == methodName && m.GetParameters().Length == 1);


                if (dtoType is not null && !string.IsNullOrEmpty(dtoTypeName) &&
                    !DTOJsonConverter<TDto, TObject>.Deserializers.ContainsKey(dtoTypeName))
                {
                    DTOJsonConverter<TDto, TObject>.Deserializers.Add(dtoTypeName, (raw, opts) =>
                        JsonSerializer.Deserialize(raw, dtoType, opts) as TDto);

                    converter = true;
                }

                if (createMethod is not null && modelType is not null &&
                    !DTOFactory<TDto, TObject>.Creators.ContainsKey(modelType))
                {
                    DTOFactory<TDto, TObject>.Creators.Add(modelType, obj =>
                        createMethod.Invoke(null, new object[] { obj }) as TDto);

                    factory = true;
                }

                if (converter && factory)
                    _registeredTypes.Add((dtoType, modelType));
            }
        }
    }

    /// <summary>
    /// Invokes the static JSON converter registration method on all discovered and registered DTO types.
    /// </summary>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> instance to which custom converters will be added.</param>
    /// <remarks>
    /// This method iterates over all DTO types registered via <see cref="EnsureInitialized"/> and uses reflection
    /// to find and invoke their static <c>RegisterDTOJsonConverters</c> method, if it exists. This allows each DTO
    /// to contribute its own JSON converters (e.g., for polymorphic serialization) to the global serialization configuration.
    ///
    /// Each DTO must declare a public static method named <c>RegisterDTOJsonConverters</c> with a single parameter
    /// of type <see cref="JsonSerializerOptions"/> for this to work correctly.
    /// </remarks>
    public static void RegisterAllObjectDTOJsonConverters(JsonSerializerOptions options)
    {
        foreach (var reg in RegisteredTypes)
        {
            var dtoType = reg.dto;
            var method = dtoType.GetMethod(
                IDTO<TObject>.RegisterDTOJsonConvertersPropertyName,
                BindingFlags.Static | BindingFlags.Public);

            if (method != null)
                method.Invoke(null, new object[] { options });
        }
    }
}