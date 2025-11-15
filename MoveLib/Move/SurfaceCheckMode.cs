using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveLib.Move;
/// <summary>
/// Specifies the type of surface check to perform for an object.
/// </summary>
public enum SurfaceCheckMode
{
    /// <summary>
    /// Checks for the highest surface below the object (e.g., ground or floor).
    /// </summary>
    Ground,

    /// <summary>
    /// Checks for the lowest surface above the object (e.g., ceiling or roof).
    /// </summary>
    Ceiling
}
