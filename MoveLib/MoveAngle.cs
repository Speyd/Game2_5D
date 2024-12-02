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
            entityAngle -= setting.moveSpeedAngel * direction;
        }

        public void ResetAngle(Entity entity, double deltaTime)
        {
            setting.moveSpeedAngel = 1 * deltaTime;

            entity.Angle = setting.angle % (2 * Math.PI);
           // Console.WriteLine(entity.GetEntityA());
            entity.VerticalAngle = setting.verticalAngle;
        }
    }
}
