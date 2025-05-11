using ScreenLib;
using SFML.System;
using SFML.Window;
using ProtoRender.Object;

namespace MoveLib.Angle;
/// <summary>
/// Handles mouse movement input to control the unit's horizontal and vertical viewing angles.
/// Requires a unit to be set via <see cref="SetControlledUnit"/> before processing input.
/// </summary>
public static class MoveMouse
{
    private static IUnit? _unit;

    /// <summary>
    /// Sets the unit that will be controlled by mouse movement.
    /// </summary>
    /// <param name="unit">The unit to control.</param>
    public static void SetControlledUnit(IUnit unit)
    {
        _unit = unit;
    }

    /// <summary>
    /// Calculates and updates the unit's horizontal angle based on mouse movement.
    /// </summary>
    /// <param name="currentMousePosition">The current position of the mouse.</param>
    private static void SetAngleMouse(Vector2i currentMousePosition)
    {
        if (_unit is null) return;

        int actualMousePositionX = currentMousePosition.X - Screen.Setting.HalfWidth;
        float normalizedSensitivity = _unit.MouseSensitivity * Screen.ScreenRatio;

        _unit.TempAngle += actualMousePositionX * normalizedSensitivity;
    }

    /// <summary>
    /// Calculates and updates the unit's vertical angle based on mouse movement,
    /// clamped within the allowed vertical angle range.
    /// </summary>
    /// <param name="currentMousePosition">The current position of the mouse.</param>
    private static void SetVerticalAngleMouse(Vector2i currentMousePosition)
    {
        if (_unit is null) return;

        int actualMousePositionY = currentMousePosition.Y - Screen.Setting.HalfHeight;
        float normalizedSensitivity = _unit.MouseSensitivity * Screen.ScreenRatio;

        _unit.TempVerticalAngle += actualMousePositionY * normalizedSensitivity;
        _unit.TempVerticalAngle = (float)Math.Clamp(_unit.TempVerticalAngle, _unit.MinVerticalAngle, _unit.MaxVerticalAngle);
    }

    // <summary>
    /// Handles the actual SFML mouse movement event and updates the unit's view angles.
    /// Re-centers the mouse cursor to the screen center after processing.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Mouse move event arguments.</param>
    public static void OnMouseMoved(object sender, MouseMoveEventArgs e)
    {
        if (_unit is null || !_unit.IsMouseCaptured)
            return;

        Vector2i currentMousePosition = new Vector2i(e.X, e.Y);
        Mouse.SetPosition(new Vector2i(Screen.Setting.HalfWidth, Screen.Setting.HalfHeight), Screen.Window);

        SetAngleMouse(currentMousePosition);
        SetVerticalAngleMouse(currentMousePosition);
    }
}

