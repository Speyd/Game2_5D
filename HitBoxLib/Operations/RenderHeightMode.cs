using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HitBoxLib.Operations;

/// <summary>Method for determining the depth of a figure</summary>
public enum RenderHeightMode
{
    /// <summary>The distance is calculated from each edge</summary>
    EdgeBased,
    /// <summary>The distance is calculated from the center of the object</summary>
    CenterBased
}
