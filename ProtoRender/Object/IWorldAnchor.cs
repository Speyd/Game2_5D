using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoRender.Object;
/// <summary>
/// Marker interface that designates an object as a <c>world anchor</c>.
/// A world anchor is an entity whose presence prevents its associated world
/// from being unloaded or suspended. 
/// </summary>
/// <remarks>
/// This interface intentionally defines no members. 
/// Its purpose is to provide a semantic contract that can be checked 
/// at runtime or used in generic constraints to identify "anchoring" entities.
///
/// Typical use cases:
/// <list type="bullet">
///   <item>Keep specific worlds always loaded while an anchor object exists.</item>
///   <item>Differentiate anchor objects from regular world entities.</item>
///   <item>Enable infrastructure components to query for anchoring behavior without 
///   introducing hard dependencies between systems.</item>
/// </list>
/// </remarks>
public interface IWorldAnchor
{
}
