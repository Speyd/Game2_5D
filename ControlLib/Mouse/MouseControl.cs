using SFML.System;
using SFML.Window;
using ScreenLib;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlLib.Mouse;
/// <summary>
/// Provides static methods for handling mouse input and translating it into camera/view angle adjustments.
/// Manages mouse movement processing for a single controlled target implementing <see cref="IMouseControllable"/>.
/// </summary>
/// <remarks>
/// <para>This class handles:
/// <list type="bullet">
/// <item>Mouse position normalization (cursor recentering)</item>
/// <item>Horizontal rotation (yaw) calculation</item>
/// <item>Vertical rotation (pitch) calculation with angle clamping</item>
/// </list>
/// </para>
/// <para>Note: The controlled target must be set via <see cref="SetControlledTarget"/> before processing input.</para>
/// </remarks>
public static class MouseControl
{
    private static IMouseControllable? _target;

    /// <summary>
    /// Sets the current target for mouse movement control.
    /// </summary>
    /// <param name="target">The <see cref="IMouseControllable"/> object to receive mouse input.</param>
    public static void SetControlledTarget(IMouseControllable target)
    {
        _target = target;
    }

    /// <summary>
    /// Processes SFML mouse movement events and updates target's viewing angles.
    /// </summary>
    /// <param name="sender">Event source (typically SFML window).</param>
    /// <param name="e">Mouse movement event data.</param>
    /// <remarks>
    /// Automatically recenters the mouse cursor and applies sensitivity settings from the controlled target.
    /// Only processes input when <see cref="IMouseControllable.IsMouseCaptured"/> is true.
    /// </remarks>
    public static void OnMouseMoved(object? sender, MouseMoveEventArgs e)
    {
        if (_target is null || !_target.IsMouseCaptured)
            return;

        var currentMousePos = new Vector2i(e.X, e.Y);
        SFML.Window.Mouse.SetPosition(new Vector2i(Screen.Setting.HalfWidth, Screen.Setting.HalfHeight), Screen.Window);

        UpdateHorizontalAngle(currentMousePos);
        UpdateVerticalAngle(currentMousePos);
    }

    /// <summary>
    /// Calculates horizontal rotation angle based on mouse X-axis movement.
    /// </summary>
    /// <param name="mousePos">Current mouse position in screen coordinates.</param>
    private static void UpdateHorizontalAngle(Vector2i mousePos)
    {
        int deltaX = mousePos.X - Screen.Setting.HalfWidth;
        float sensitivity = _target!.MouseSensitivity * Screen.ScreenRatio;
        _target.TempAngle += deltaX * sensitivity;
    }

    /// <summary>
    /// Calculates vertical rotation angle based on mouse Y-axis movement and clamps it within configured limits.
    /// </summary>
    /// <param name="mousePos">Current mouse position in screen coordinates.</param>
    private static void UpdateVerticalAngle(Vector2i mousePos)
    {
        int deltaY = mousePos.Y - Screen.Setting.HalfHeight;
        float sensitivity = _target!.MouseSensitivity * Screen.ScreenRatio;
        _target.TempVerticalAngle += deltaY * sensitivity;
        _target.TempVerticalAngle = Math.Clamp(_target.TempVerticalAngle, _target.MinVerticalAngle, _target.MaxVerticalAngle);
    }
}