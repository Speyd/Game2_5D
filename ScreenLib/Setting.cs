using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ScreenLib.SettingScreen;
public class Setting
{
    private const int _tile = 100;
    /// <summary>World unit of horizontal measurement(X and Y axis)</summary>
    public int Tile { get; } = _tile;
    /// <summary>Half of Tile</summary>
    public int HalfTile { get; } = _tile / 2;


    private const float _verticalTile = 850;
    /// <summary>World unit of vertical measurement(Z axis)</summary>
    public float VerticalTile { get; } = _verticalTile;
    /// <summary>Half of VerticalTile</summary>
    public float HalfVerticalTile { get; } = _verticalTile / 2;

    /// <summary>Settings for Parallel</summary>
    public ParallelOptions ParallelOptions { get; init; } = new ParallelOptions
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount
    };
    /// <summary>Half of Width</summary>
    public int HalfWidth { get; private set; }
    /// <summary>Half of Width</summary>
    public int HalfHeight { get; private set; }
    /// <summary>Amount Rays</summary>
    public int AmountRays { get; private set; }
    /// <summary>Scale objects</summary>
    public int Scale { get; private set; }
    /// <summary>Center Ray</summary>
    public int CenterRay { get; private set; }

    /// <summary>
    /// Window customization constructor
    /// </summary>
    /// <param name="ScreenWidth">Screen width</param>
    /// <param name="ScreenHeight">Screen height</param>
    /// <param name="amountRays">Amount Rays</param>
    /// <exception cref="Exception">When one of the screen sizes is less than or equal to 0</exception>
    /// <exception cref="Exception">When the number of rays is less than or equal to 0</exception> 
    public Setting(int ScreenWidth, int ScreenHeight, int amountRays = -1)
    {
        if (ScreenWidth <= 0 || ScreenHeight <= 0)
            throw new Exception("Error builder 'Setting'");

        HalfWidth = ScreenWidth / 2;
        HalfHeight = ScreenHeight / 2;


        AmountRays = amountRays <= 0 ? ScreenWidth :
            amountRays > ScreenWidth ? throw new Exception("Amount ray more ScreenWidth"):
            amountRays;

        Scale = ScreenWidth / AmountRays;
        CenterRay = AmountRays / 2 - 1;
    }
    /// <summary>Updates fields depending on width</summary>
    public void ResetScreenWidthSetting()
    {
        HalfWidth = Screen.ScreenWidth / 2;

        AmountRays = Screen.ScreenWidth;
        Scale = Screen.ScreenWidth / AmountRays;
        CenterRay = AmountRays / 2 - 1;
    }
    /// <summary>Updates fields depending on height</summary>
    public void ResetScreenHeightSetting()
    {
        HalfHeight = Screen.ScreenHeight / 2;
    }
}
