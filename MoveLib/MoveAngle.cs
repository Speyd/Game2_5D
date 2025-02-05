using EntityLib;
using ScreenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveLib
{
    public class MoveAngle(MoveLib.Setting setting)
    {
        public void TurnAngle(ref double entityAngle, int direction)
        {
            double normalizedMoveSpeedAngel = setting.MoveSpeedAngel * Screen.ScreenRatio;
            entityAngle -= normalizedMoveSpeedAngel * direction;

            if (entityAngle > Math.PI)
                entityAngle -= 2 * Math.PI;
            if (entityAngle < -Math.PI)
                entityAngle += 2 * Math.PI;
        }

        public void ResetAngle(Entity entity, double deltaTime)
        {
            double normalizedMoveSpeedAngel = setting.MoveSpeedAngel * Screen.ScreenRatio;
            setting.MoveSpeedAngel = 1 * deltaTime * normalizedMoveSpeedAngel;

            entity.Angle = setting.TempAngle % (2 * Math.PI);
            entity.VerticalAngle = setting.TempVerticalAngle;
        }
    }
}
