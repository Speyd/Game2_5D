using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlLib
{
    internal class Setting
    {
        //Speed
        public double moveSpeed;
        public double moveSpeedAngel;

        //TempsValue
        public double verticalAngle = 0.0;
        public double angle = 0.0;

        //Settig Control
        public float minDistanceFromWall;
        public float mouseSensitivity;
        public bool isMouseCaptured = true;


        public Setting(float minDistanceFromWall, float mouseSensitivity) 
        {
            this.minDistanceFromWall = minDistanceFromWall;
            this.mouseSensitivity = mouseSensitivity;
        }
    }
}
