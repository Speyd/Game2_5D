using EntityLib;
using FpsLib;
using ScreenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MoveLib;
public static class MoveAngle
{
    public static void TurnAngle(ref double entityAngle, int direction)
    {
        double normalizedMoveSpeedAngel = MoveLib.Setting.MoveSpeedAngel * Screen.ScreenRatio;
        entityAngle -= normalizedMoveSpeedAngel * direction;

        if (entityAngle > Math.PI)
            entityAngle -= 2 * Math.PI;
        if (entityAngle < -Math.PI)
            entityAngle += 2 * Math.PI;
    }

    public static void ResetAngle(Entity entity)
    {
        double normalizedMoveSpeedAngel = MoveLib.Setting.MoveSpeedAngel * Screen.ScreenRatio;
        MoveLib.Setting.MoveSpeedAngel = 1 * FPS.GetDeltaTime() * normalizedMoveSpeedAngel;

        entity.Angle = MoveLib.Setting.TempAngle % (2 * Math.PI);
        entity.VerticalAngle = MoveLib.Setting.TempVerticalAngle;
    }
}
