using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EntityLib;
public interface IObserver
{
    /// <summary>Fov observer</summary>
    double Fov {  get; set; }
    /// <summary>Half Fov observer</summary>
    double HalfFov { get; }
    /// <summary>Delta Angle observer</summary>
    double DeltaAngle { get; }

    /// <summary>X - Cos(Angle); Y - Sin(Angle)</summary>
    Vector2f Direction { get;}
    /// <summary>X - -Sin(Angle); Y - Cos(Angle)</summary>
    Vector2f Plane { get; }


    /// <summary>Angle Entity(Horizontal axis)</summary>
    double Angle { get; set; }
    /// <summary>Vertical Angle Entity(Vertical axis)</summary>
    double VerticalAngle { get; set; }
}
