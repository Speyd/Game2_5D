using EntityLib;
using ScreenLib;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Render.RenderInterface;
using DataPipes.Pool;
using SFML.System;


namespace Render.RenderAlgorithm;
public class Result : IResettable
{
    private const int textureStretchingCloseUp = 8;

    public double Depth { get; set; } = 0;
    public double Offset { get; set; } = 0;
    public double ProjHeight { get; set; } = 0;
    public double CarAngle { get; set; } = 0;
    public double SinCarAngle { get; set; } = 0;
    public double CosCarAngle { get; set; } = 0;
    public int Ray { get; set; } = 0;
    public Vector2f? PositionPreviousObject { get; set; } = null;

    public void CalculationSettingRender(Entity entity, int ray, double depth, double coordinate, double carAngle)
    {
        Offset = coordinate;

        Depth = depth;
        Depth *= Math.Cos(entity.Angle - carAngle);
        Depth = Math.Max(Depth, 0.1);

        Ray = ray;
        CarAngle = carAngle;
        ;
        Offset = (int)Offset % Screen.Setting.Tile;
        ProjHeight = Math.Min((int)(entity.ProjCoeff / Depth), textureStretchingCloseUp * Screen.ScreenHeight);
    }

    public void Reset()
    {
        Depth = 0;
        Offset = 0;
        ProjHeight = 0;
        CarAngle = 0;
        Ray = 0;
        PositionPreviousObject = null;
    }

}
