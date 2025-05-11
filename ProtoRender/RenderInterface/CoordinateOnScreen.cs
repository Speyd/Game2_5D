using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProtoRender.RenderInterface;
/// <summary>
/// Represents an object's coordinates on the screen,
/// including its horizontal position and vertical boundaries (top and bottom).
/// </summary>
public struct CoordinateOnScreen
{
    /// <summary>
    /// The horizontal (X-axis) position of the object on the screen.
    /// </summary>
    public float X;

    /// <summary>
    /// The lower vertical boundary of the object on the screen(Y-axis).
    /// </summary>
    public float Bottom;

    /// <summary>
    /// The upper vertical boundary of the object on the screen (minimum Y-coordinate)(Y-axis).
    /// </summary>
    public float Top;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoordinateOnScreen"/> struct
    /// with a horizontal position and a single vertical coordinate, used as both the top and bottom.
    /// </summary>
    /// <param name="x">The horizontal (X-axis) position on the screen.</param>
    /// <param name="y">The vertical (Y-axis) position on the screen, used for both top and bottom.</param>
    public CoordinateOnScreen(float x, float y)
    {
        X = x;
        Bottom = y;
        Top = y;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CoordinateOnScreen"/> struct
    /// with a horizontal position and separate top and bottom vertical boundaries.
    /// </summary>
    /// <param name="x">The horizontal (X-axis) position on the screen.</param>
    /// <param name="bottom">The lower vertical boundary (bottom Y-coordinate).</param>
    /// <param name="top">The upper vertical boundary (top Y-coordinate).</param>
    public CoordinateOnScreen(float x, float bottom, float top)
    {
        X = x;
        Bottom = bottom;
        Top = top;
    }

}
