
namespace DataPipes.DTO.Register;
/// <summary>
/// Specifies the model type associated with a Data Transfer Object (DTO) class.
/// </summary>
/// <remarks>
/// This attribute is used to mark DTO classes and indicate the source model type
/// that the DTO corresponds to. It facilitates automatic registration and mapping
/// between models and their DTOs.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class RegisterDTOAttribute : Attribute
{
    /// <summary>
    /// Gets the type of the model that this DTO represents.
    /// </summary>
    public Type ModelType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterDTOAttribute"/> class with the specified model type.
    /// </summary>
    /// <param name="modelType">The <see cref="Type"/> of the model associated with the DTO.</param>
    public RegisterDTOAttribute(Type modelType) => ModelType = modelType;
}