using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProtoRender.Map;
/// <summary>
/// Defines functionality for adding objects to a map, including single-instance constraints and custom handling logic.
/// </summary>
public interface IMapAdder
{
    /// <summary>
    /// Returns a value indicating whether this object can be added to a map cell.
    /// </summary>
    public bool IsSingleAddable { get; }

    /// <summary>
    /// Handles the logic for adding an object to the map at the specified coordinates.
    /// </summary>
    /// <param name="x">The X coordinate on the map.</param>
    /// <param name="y">The Y coordinate on the map.</param>
    /// <param name="resetHitBoxSide">If true, resets the hitbox side during addition.</param>
    public void HandleObjectAddition(double x, double y, bool resetHitBoxSide);
}
