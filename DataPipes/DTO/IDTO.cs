using DataPipes.DTO.Register;
using System.Text.Json;


namespace DataPipes.DTO;
/// <summary>
/// Interface for Data Transfer Objects (DTO) that supports JSON serialization and deserialization,
/// along with utility functionality for generating a unique file path for saving the data.
/// </summary>
/// <typeparam name="T">The type of DTO that implements this interface.</typeparam>
public interface IDTO<T>
{
    /// <summary>
    /// Gets the global or shared <see cref="DtoTypeRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    public static abstract DTOJsonOptionsRegistry DtoTypeRegistry { get; }
    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    string BaseFileName { get; }

    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    string BaseExtension { get; }

    /// <summary>
    /// Converts the object into its DTO representation. Should be called before serialization if needed.
    /// </summary>
    void ToDTO();
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    T ToObject();
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    void ToObject(T target);


    /// <summary>
    /// Ensures that the DTO registration logic is initialized once per application domain.
    /// Typically used to prepare internal mappings or caches for DTOs.
    /// </summary>
    public static abstract void RegisterDTO();

    /// <summary>
    /// The name of the static method expected on DTO types to register custom JSON converters.
    /// </summary>
    /// <remarks>
    /// This constant is used during reflection to locate and invoke the <c>RegisterDTOJsonConverters</c> method
    /// on each DTO type. The method should be defined as:
    /// <code>
    /// public static void RegisterDTOJsonConverters(JsonSerializerOptions options)
    /// </code>
    /// and is responsible for adding any necessary <see cref="System.Text.Json.Serialization.JsonConverter"/> instances
    /// to the provided <see cref="System.Text.Json.JsonSerializerOptions"/>.
    /// </remarks>

    public static readonly string RegisterDTOJsonConvertersPropertyName = nameof(RegisterDTOJsonConverters);
    /// <summary>
    /// Registers custom JSON converters required for correct serialization and deserialization of DTO types.
    /// </summary>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to which converters will be added.</param>
    public static abstract void RegisterDTOJsonConverters(JsonSerializerOptions options);

    /// <summary>
    /// Generates a unique file path in the given directory based on a base file name and extension.
    /// This avoids overwriting existing files.
    /// </summary>
    /// <param name="directory">The target directory to generate the file in.</param>
    /// <param name="baseName">The base name to use for the file (defaults to "dto_").</param>
    /// <param name="extension">The file extension (defaults to ".json").</param>
    /// <returns>A unique file path for saving a new file.</returns>
    public static string GetUniqueFilePath(string directory, string baseName = "dto_", string extension = ".json")
    {
        int counter = 1;
        string fileName;
        string filePath;

        do
        {
            fileName = $"{baseName}{counter}{extension}";
            filePath = Path.Combine(directory, fileName);
            counter++;
        } while (File.Exists(filePath));

        return filePath;
    }
}