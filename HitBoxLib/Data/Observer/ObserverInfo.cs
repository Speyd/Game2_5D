using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HitBoxLib.Data.Observer;
/// <summary>Information about observer</summary>
public struct ObserverInfo
{
    /// <summary>Angle observer</summary>
    public double angle;
    /// <summary>Vertical angle observer</summary>
    public double verticalAngle;
    /// <summary>Fov observer</summary>
    public double fov;
    /// <summary>Delta angle observer</summary>
    public double deltaAngle;
    /// <summary>Position observer</summary>
    public Vector3f position;
}
