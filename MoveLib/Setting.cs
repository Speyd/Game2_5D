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
    /// <summary>Entity movement speed</summary>
    public static float MoveSpeed { get; set; } = 150f;
    /// <summary>Normalizes mouse sensitivity depending on fps</summary>
    public static double MoveSpeedAngel { get; set; } = 1;

    //TempsValue
    public static double TempVerticalAngle = 0.0;
    public static double TempAngle = 0.0;

    //Settig Control
    /// <summary>Minimum collision distance to an object</summary>
    public static float MinDistanceFromWall { get; set; } = 100;
    /// <summary>Mouse sensitivity</summary>
    public static float MouseSensitivity { get; set; } = 0.001f;
    public static bool IsMouseCaptured { get; set; } = true;

    //Settig Mouse
    /// <summary>Limits camera rotation vertically above the middle of the screen</summary>
    public static double MinVerticalAngle { get; set; } = -Math.PI / 2;
    /// <summary>Limits camera rotation vertically below the middle of the screen</summary>
    public static double MaxVerticalAngle { get; set; } = Math.PI / 2;

}
