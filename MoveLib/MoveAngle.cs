using EntityLib;
using ScreenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveLib
{
    public class MoveAngle(Screen screen, MoveLib.Setting setting)
    {
        public void TurnAngle(ref double entityAngle, int direction)
        {
            entityAngle -= setting.moveSpeedAngel * direction;
        }

        public void resetAngle(Entity entity, double deltaTime)
        {
           // double tempMoveSpeed = (100 * deltaTime);
            //setting.moveSpeed = (float)(tempMoveSpeed - Math.Min(tempMoveSpeed - 0.6, (screen.Setting.AmountRays / screen.ScreenWidth)));
            setting.moveSpeedAngel = 1 * deltaTime;

            entity.GetEntityA() = setting.angle % (2 * Math.PI);
            entity.GetEntityVerticalA() = setting.verticalAngle;
        }
    }
}
