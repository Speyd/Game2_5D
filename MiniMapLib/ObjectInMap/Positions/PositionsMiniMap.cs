using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MiniMapLib.ObjectInMap.Positions;
/// <summary>
/// Specifies the position of the minimap on the game screen.
/// </summary>
public enum PositionsMiniMap
{
    /// <summary>None position.</summary>
    None,

    /// <summary>Minimap is displayed in the upper-left corner of the screen.</summary>
    UpperLeftCorner,

    /// <summary>Minimap is displayed in the lower-left corner of the screen.</summary>
    LowerLeftCorner,

    /// <summary>Minimap is displayed in the upper-right corner of the screen.</summary>
    UpperRightCorner,

    /// <summary>Minimap is displayed in the lower-right corner of the screen.</summary>
    LowerRightCorner,
}