using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMapLib;
/// <summary>
/// Specifies the method used for rendering output in the system.
/// </summary>
public enum OutputRenderMethod
{
    /// <summary>
    /// Renders the output as a texture.
    /// </summary>
    Texture,

    /// <summary>
    /// Renders the output using a solid color.
    /// </summary>
    Color,
}
