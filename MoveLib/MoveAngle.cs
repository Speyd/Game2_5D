using FpsLib;
using ScreenLib;


namespace MoveLib.Angle;
/// <summary>
/// Handles the logic for adjusting and resetting the unit's rotation angles,
/// including horizontal and vertical orientation based on input or mouse movement.
/// </summary>
public static class MoveAngle
{
    private static double SensitivityRelativeFPS = 1;
    /// <summary>
    /// Rotates the unit's horizontal angle based on a directional input.
    /// </summary>
    /// <param name="unit">The unit whose angle is being changed.</param>
    /// <param name="direction">
    /// The direction to rotate: typically -1 for left, +1 for right.
    /// </param>
    public static void TurnAngle(ProtoRender.Object.IUnit unit, int direction)
    {
        double normalizedMoveSpeedAngel = SensitivityRelativeFPS * Screen.ScreenRatio;
        unit.Angle -= Screen.ScreenRatio * direction;

        if (unit.Angle > Math.PI)
            unit.Angle -= 2 * Math.PI;
        if (unit.Angle < -Math.PI)
            unit.Angle += 2 * Math.PI;
    }

    /// <summary>
    /// Updates the unit's movement angle and vertical viewing angle based on mouse movement.
    /// Also normalizes rotation speed using frame delta time and sensitivity.
    /// </summary>
    /// <param name="unit">The unit to update angle values for.</param>
    public static void ResetAngle(ProtoRender.Object.IUnit unit)
    {
        double normalizedMoveSpeedAngel = unit.MouseSensitivity * Screen.ScreenRatio;
        SensitivityRelativeFPS = FPS.GetDeltaTime() * normalizedMoveSpeedAngel;

        unit.Angle = unit.TempAngle % (2 * Math.PI);
        unit.VerticalAngle = unit.TempVerticalAngle;
    }
}
