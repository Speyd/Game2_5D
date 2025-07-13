using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProtoRender.Object;
/// <summary>
/// Defines an entity that can move in the game world.
/// </summary>
public interface IMovable
{
    /// <summary>
    /// Current movement speed of the entity (units per second).
    /// </summary>
    public float MoveSpeed { get; set; }

    /// <summary>
    /// Minimum allowed distance between the entity and nearby obstacles (collision padding).
    /// </summary>
    public float MinDistanceFromWall { get; set; }

    /// <summary>
    /// Thread-safe collection of object to ignore during processing.
    /// </summary>
    ConcurrentDictionary<IObject, byte> IgnoreCollisionObjects { get; set; }
}
