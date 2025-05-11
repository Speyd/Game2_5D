using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ScreenLib.Output;
/// <summary>Rendering order of objects in the main window</summary>
public enum OutputPriorityType
{
    /// <summary>Background</summary>
    Background,
    /// <summary>Objects that are rendered by rays</summary>
    ZBufferRender,
    /// <summary>Program interfaces</summary>
    Interface,
    /// <summary>Enter</summary>
    Enter,
}
