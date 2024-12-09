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
            entityAngle -= setting.MoveSpeedAngel * direction;

            if (entityAngle > Math.PI)
                entityAngle -= 2 * Math.PI;
            if (entityAngle < -Math.PI)
                entityAngle += 2 * Math.PI;
        }

        public void ResetAngle(Entity entity, double deltaTime)
        {
            setting.MoveSpeedAngel = 1 * deltaTime;

            entity.Angle = setting.TempAngle % (2 * Math.PI);
            entity.VerticalAngle = setting.TempVerticalAngle;
        }
    }
}
