using ScreenLib;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoRender.RenderInterface;
using DataPipes.Pool;
using SFML.System;


namespace ProtoRender.RenderAlgorithm;
/// <summary>
/// Represents the result of a raycasting operation, containing information needed for rendering and object positioning.
/// </summary>
public class Result : IResettable
{
    private const int textureStretchingCloseUp = 8;

    /// <summary>
    /// Gets or sets the distance from the observer to the object hit by the ray.
    /// </summary>
    public double Depth { get; set; } = 0;

    /// <summary>
    /// Gets or sets the horizontal coordinate (offset) where the ray hit the object.
    /// </summary>
    public double Offset { get; set; } = 0;

    /// <summary>
    /// Gets or sets the projected height of the object based on its distance from the observer.
    /// </summary>
    public double ProjHeight { get; set; } = 0;

    /// <summary>
    /// Gets or sets the viewing angle of the car or observer relative to the ray.
    /// </summary>
    public double CarAngle { get; set; } = 0;
    /// <summary>
    /// Sin CarAngle.
    /// </summary>
    public double SinCarAngle { get; set; } = 0;
    /// <summary>
    /// Cos CarAngle.
    /// </summary>
    public double CosCarAngle { get; set; } = 0;

    /// <summary>
    /// Gets or sets the index of the ray used in the raycasting process.
    /// </summary>
    public int Ray { get; set; } = 0;

    /// <summary>
    /// Gets or sets the previous position of the object detected by the ray.
    /// </summary>
    public CoordinateOnScreen? PositionPreviousObject { get; set; } = null;

    /// <summary>
    /// Calculates and sets rendering-related parameters based on observer and raycasting data.
    /// </summary>
    /// <param name="observer">The observer (camera or unit) performing the raycast.</param>
    /// <param name="ray">The index of the current ray.</param>
    /// <param name="depth">The distance from the observer to the detected object.</param>
    /// <param name="coordinate">The coordinate where the ray intersects the object.</param>
    /// <param name="carAngle">The angle of the observer relative to the ray.</param>
    public void CalculationSettingRender(ProtoRender.Object.IObserver observer, int ray, double depth, double coordinate, double carAngle)
    {
        Offset = coordinate;

        Depth = depth;
        Depth *= Math.Cos(observer.Angle - carAngle);
        Depth = Math.Max(Depth, 0.1);

        Ray = ray;
        CarAngle = carAngle;

        Offset = (int)Offset % Screen.Setting.Tile;
        ProjHeight = Math.Min((int)(observer.ProjCoeff / Depth), textureStretchingCloseUp * Screen.ScreenHeight);
    }

    /// <summary>
    /// Resets all fields of the result to their default values.
    /// </summary>
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

