using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;


namespace MoveLib;
public class Setting
{
    //Speed
    public float MoveSpeed { get; set; } = 150f;
    public double MoveSpeedAngel { get; set; } = 1;

    //TempsValue
    public double TempVerticalAngle = 0.0;
    public double TempAngle = 0.0;

    //Settig Control
    public float MinDistanceFromWall { get; set; } = 50;
    public float MouseSensitivity { get; set; } = 0.001f;
    public bool IsMouseCaptured { get; set; } = true;

    //Settig Mouse
    public static double MinVerticalAngle { get; set; } = -Math.PI / 2;
    public static double MaxVerticalAngle { get; set; } = Math.PI / 2;

    public Setting() 
    {}
}
