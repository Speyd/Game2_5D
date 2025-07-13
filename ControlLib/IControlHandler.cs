using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlLib;
/// <summary>
/// Exposes the <see cref="ControlLib.Control"/> instance used for managing input bindings.
/// </summary>
public interface IControlHandler
{
    /// <summary>
    /// Associated input control manager.
    /// </summary>
    ControlLib.Control Control { get; }
}
