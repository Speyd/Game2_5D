using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPipes.DTO.Register;
/// <summary>
/// Defines a contract for Data Transfer Objects (DTOs) that can be registered and created from a source object.
/// </summary>
/// <typeparam name="TDto">The DTO type implementing this interface.</typeparam>
/// <typeparam name="TObject">The source object type from which the DTO is created.</typeparam>
/// <remarks>
/// Implementing types must provide a static method <see cref="CreateDTO"/> which creates a DTO instance from an instance of <typeparamref name="TObject"/>.
/// This interface is intended to support registration and factory patterns for DTO creation.
/// </remarks>
public interface IRegisterableDTO<TDto, TObject>
    where TDto : IDTO<TObject>
{
    /// <summary>
    /// The name of the static method used to create a DTO from a source object.
    /// </summary>
    public static readonly string CreateDTOPropertyName = nameof(CreateDTO);

    /// <summary>
    /// Creates a new instance of the DTO from the specified source object.
    /// </summary>
    /// <param name="obj">The source object from which to create the DTO.</param>
    /// <returns>A new instance of <typeparamref name="TDto"/>.</returns>
    static abstract TDto CreateDTO(TObject obj);
}