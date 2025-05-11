using DataPipes.Pool;
using ProtoRender.Object;
using ProtoRender.RenderInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProtoRender.RenderAlgorithm;

/// <summary>Stores basic information about an object during raycasting</summary>
public class InfoObject : IResettable
{
    /// <summary>Distance to the object</summary>
    public double depth;
    /// <summary>Coordinate value</summary>
    public double coordinate;
    /// <summary>Reference to the rendered object</summary>
    public IObject? Object = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="InfoObject"/> class with the specified depth, coordinate, and associated object.
    /// </summary>
    /// <param name="depth">The depth value used for rendering or sorting purposes.</param>
    /// <param name="coordinate">The coordinate on the screen or axis where the object is positioned.</param>
    /// <param name="Object">The object associated with this info container, typically a game or drawable entity.</param>

    public InfoObject(double depth, double coordinate, IObject Object)
    {
        this.depth = depth;
        this.coordinate = coordinate;
        this.Object = Object;
    }

    /// <summary>
    /// Resets all fields of the result to their default values.
    /// </summary>
    public void Reset()
    {
        depth = 0;
        coordinate = 0;
        Object = null;
    }
}
