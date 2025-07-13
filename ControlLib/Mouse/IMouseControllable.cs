using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlLib.Mouse;
/// <summary>
/// Defines properties required for mouse-controlled camera or character movement.
/// </summary>
/// <remarks>
/// Implement this interface on objects that need mouse-based view rotation control.
/// Typical implementations include:
/// <list type="bullet">
/// <item>First-person character controllers</item>
/// <item>Free-look cameras</item>
/// <item>Editor viewport controls</item>
/// </list>
/// </remarks>
public interface IMouseControllable
{
    /// <summary>
    /// Gets or sets the mouse sensitivity multiplier for rotation.
    /// Higher values make mouse movements result in larger rotations.
    /// </summary>
    /// <value>A positive floating-point sensitivity value.</value>
    public float MouseSensitivity { get; set; }

    /// <summary>
    /// Gets or sets whether mouse input is currently captured for controlling rotation.
    /// When false, mouse movements will be ignored.
    /// </summary>
    public bool IsMouseCaptured { get; set; }

    /// <summary>
    /// Gets or sets the temporary horizontal rotation angle (yaw) in radians.
    /// Represents unprocessed rotation that hasn't been applied to the object yet.
    /// </summary>
    public double TempAngle { get; set; }

    /// <summary>
    /// Gets or sets the temporary vertical rotation angle (pitch) in radians.
    /// Represents unprocessed rotation that hasn't been applied to the object yet.
    /// </summary>
    public double TempVerticalAngle { get; set; }

    /// <summary>
    /// Gets or sets the minimum allowed vertical rotation angle (looking down) in radians.
    /// </summary>
    public double MinVerticalAngle { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed vertical rotation angle (looking up) in radians.
    /// </summary>
    public double MaxVerticalAngle { get; set; }
}
