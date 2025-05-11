using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HitBoxLib.PositionObject;
/// <summary>
/// Represents a delegate that is called during a coordinate movement event,
/// along with its fixed parameters to be passed upon invocation.
/// </summary>
public class MoveAxisDelegate
{
    /// <summary>
    /// The delegate method to invoke when the axis moves.
    /// </summary>
    public Delegate? MoveDelegate { get; set; }

    /// <summary>
    /// An array of fixed parameters to pass to the delegate when it is invoked.
    /// </summary>
    public object[] FixedParameters { get; set; }
}
