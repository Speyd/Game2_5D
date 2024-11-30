using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;

namespace MoveLib
{
    public class Setting
    {
        //Speed
        public float moveSpeed = 150f;
        public double moveSpeedAngel;

        //TempsValue
        public double verticalAngle = 0.0;
        public double angle = 0.0;

        //Settig Control
        public float minDistanceFromWall;
        public float mouseSensitivity;
        public bool isMouseCaptured = true;

        //Settig Mouse
        public double minVerticalAngle = -Math.PI / 2;
        public double maxVerticalAngle = Math.PI / 2;

        public Setting(float minDistanceFromWall, float mouseSensitivity) 
        {
            //moveSpeed = 20;
            this.minDistanceFromWall = minDistanceFromWall;
            this.mouseSensitivity = mouseSensitivity;
        }
    }
}
