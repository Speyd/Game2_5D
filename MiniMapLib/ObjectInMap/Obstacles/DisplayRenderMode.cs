using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MiniMapLib.ObjectInMap.Obstacles;

/// <summary>
/// Defines the rendering modes for displaying objects on the minimap,
/// controlling which portions of the map and entities are shown.
/// </summary>
public enum DisplayRenderMode
{
    /// <summary>
    /// Renders only a predefined, limited section of the map.
    /// </summary>
    SpecificArea,

    /// <summary>
    /// Renders the entire available map area, regardless of unit position.
    /// </summary>
    EntireArea,

    /// <summary>
    /// Renders only the area visible to the unit, based on its vision or detection radius.
    /// </summary>
    UnitVisibilityArea
}

