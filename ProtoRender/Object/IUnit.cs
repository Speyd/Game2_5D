using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using ProtoRender.Map;

namespace ProtoRender.Object;
/// <summary>
/// Defines the behavior and properties of a unit, which is an object that can be observed and interacted with in the environment.
/// </summary>
/// <remarks>
/// This interface extends both <see cref="IObject"/> and <see cref="IObserver"/> interfaces,
/// representing an entity that has both the characteristics of an object in the world (e.g., position, passability)
/// and the ability to observe the environment (e.g., field of view, direction, rendering).
/// </remarks>
public interface IUnit : IObject, IObserver, IMovable
{
    /// <summary>
    /// Gets the origin position of the unit, typically representing its initial spawn or reference point.
    /// </summary>
    Vector2f OriginPosition { get; }
    /// <summary>
    /// The map object to which this object belongs
    /// </summary>
    IMap? Map { get; set; }
    /// <summary>
    /// Creates a copy of the unit with the same properties as the current instance.
    /// </summary>
    /// <returns>A new instance of the unit, which is a copy of the original unit.</returns>
    new IUnit GetCopy();
}
