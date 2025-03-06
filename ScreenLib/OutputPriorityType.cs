using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ScreenLib.Output;
/// <summary>Rendering order of objects in the main window</summary>
public enum OutputPriorityType
{
    Background,
    ZBufferRender,
    Interface,
    Enter,
}
