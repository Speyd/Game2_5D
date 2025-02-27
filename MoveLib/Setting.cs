using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;


namespace MoveLib;
public static class Setting
{
    //Speed
    public static float MoveSpeed { get; set; } = 150f;
    public static double MoveSpeedAngel { get; set; } = 1;

    //TempsValue
    public static double TempVerticalAngle = 0.0;
    public static double TempAngle = 0.0;

    //Settig Control
    public static float MinDistanceFromWall { get; set; } = 50;
    public static float MouseSensitivity { get; set; } = 0.001f;
    public static bool IsMouseCaptured { get; set; } = true;

    //Settig Mouse
    public static double MinVerticalAngle { get; set; } = -Math.PI / 2;
    public static double MaxVerticalAngle { get; set; } = Math.PI / 2;

}
