using PartsWorldLib.Down;
using PartsWorldLib.Up;
using ScreenLib;
using ProtoRender.Object;

namespace PartsWorldLib;
/// <summary>
/// Responsible for rendering the upper and lower parts of the world, delegating to respective parts.
/// Provides utilities for normalizing unit heights based on vertical angle.
/// </summary>
public class RenderPartsWorld
{
    /// <summary>
    /// Gets or sets the upper part of the world renderer.
    /// </summary>
    public IUpPart? UpPart { get; set; } = null;

    /// <summary>
    /// Gets or sets the lower part of the world renderer.
    /// </summary>
    public IDownPart? DownPart { get; set; } = null;


    /// <summary>
    /// Initializes a new instance of <see cref="RenderPartsWorld"/> with specified upper and lower parts.
    /// </summary>
    /// <param name="upPart">The upper part renderer, or null.</param>
    /// <param name="downPart">The lower part renderer, or null.</param>
    public RenderPartsWorld(IUpPart? upPart, IDownPart? downPart)
    {
        this.UpPart = upPart;
        this.DownPart = downPart;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="RenderPartsWorld"/> with only an upper part.
    /// </summary>
    /// <param name="upPart">The upper part renderer.</param>
    public RenderPartsWorld(IUpPart upPart)
        : this(upPart, null)
    { }

    /// <summary>
    /// Initializes a new instance of <see cref="RenderPartsWorld"/> with only a lower part.
    /// </summary>
    /// <param name="downPart">The lower part renderer.</param>
    public RenderPartsWorld(IDownPart downPart)
        : this(null, downPart)
    { }

    /// <summary>
    /// Initializes a new instance of <see cref="RenderPartsWorld"/> with no parts assigned.
    /// </summary>
    public RenderPartsWorld()
        : this(null, null)
    { }


    /// <summary>
    /// Normalizes the height for rendering the lower part of a unit based on its vertical angle.
    /// </summary>
    /// <param name="unit">The unit whose vertical angle is considered.</param>
    /// <returns>The normalized height for the lower part.</returns>
    public static float NormalizeHeigthDownPart(IUnit unit)
    {
        if (unit.VerticalAngle >= 0)
            return Screen.Setting.HalfHeight / (float)(unit.VerticalAngle + 1);
        else
            return Screen.Setting.HalfHeight * (float)(Math.Abs(unit.VerticalAngle) + 1);
    }

    /// <summary>
    /// Normalizes the height for rendering the upper part of a unit based on its vertical angle.
    /// </summary>
    /// <param name="unit">The unit whose vertical angle is considered.</param>
    /// <returns>The normalized height for the upper part.</returns>
    public static float NormalizeHeigthUpPart(IUnit unit)
    {
        if (unit.VerticalAngle >= 0)
            return Screen.Setting.HalfHeight / (float)(unit.VerticalAngle + 1);
        else
            return Screen.Setting.HalfHeight * (float)(Math.Abs(unit.VerticalAngle) + 1);
    }

    /// <summary>
    /// Renders both the upper and lower parts of the world for the given unit, if they are assigned.
    /// </summary>
    /// <param name="unit">The unit to render.</param>
    public void Render(IUnit unit)
    {
        UpPart?.Render(unit);
        DownPart?.Render(unit);
    }
}
