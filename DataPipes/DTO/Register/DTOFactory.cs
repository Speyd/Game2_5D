using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPipes.DTO.Register;
/// <summary>
/// A factory class responsible for creating instances of <typeparamref name="TDto"/> from instances of <typeparamref name="TObject"/>.
/// </summary>
/// <remarks>
/// Maintains a registry of creator functions mapped by the runtime type of <typeparamref name="TObject"/>.
/// When <see cref="CreateFrom"/> is called, the factory looks up the creator function for the object's exact runtime type and invokes it to produce the corresponding DTO.
/// If no creator is registered for the object's type, a <see cref="NotSupportedException"/> is thrown.
/// </remarks>
/// <typeparam name="TDto">The type of DTO objects to create.</typeparam>
/// <typeparam name="TObject">The source object type from which DTOs are created.</typeparam>
public static class DTOFactory<TDto, TObject>
{
    /// <summary>
    /// A dictionary mapping concrete runtime types of <typeparamref name="TObject"/> to functions that create corresponding <typeparamref name="TDto"/> instances.
    /// The key is the exact runtime type of the source object; the value is a function that takes the object and returns the corresponding DTO.
    /// </summary>
    public static Dictionary<Type, Func<TObject, TDto>> Creators = new();

    /// <summary>
    /// Creates a DTO instance for the given <typeparamref name="TObject"/> instance.
    /// Looks up the creator function in the <see cref="Creators"/> dictionary by the object's runtime type and invokes it.
    /// Throws a <see cref="NotSupportedException"/> if no creator is registered for the object's type.
    /// </summary>
    /// <param name="obj">The source object for which to create a DTO.</param>
    /// <returns>The created DTO instance.</returns>
    /// <exception cref="NotSupportedException">Thrown if the object's type is not supported or registered in <see cref="Creators"/>.</exception>
    public static TDto CreateFrom(TObject obj)
    {
        var type = obj?.GetType();
        if (type is not null && Creators.TryGetValue(type, out var creator))
            return creator(obj);

        throw new NotSupportedException($"Unsupported object type: {type?.Name ?? "Type is null"}");
    }
}

