using ProtoRender.Object;
using ScreenLib;
using ScreenLib.Output;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PartsWorldLib.Down;
/// <summary>
/// Renders a simple flat floor by filling the bottom portion of the screen with a solid color.
/// </summary>
public class Floor : IDownPart
{
    /// <summary>
    /// Gets or sets the output rendering layer priority for this surface.
    /// </summary>
    public OutputPriorityType OutputLayer { get; set; } = OutputPriorityType.Background;

    /// <summary>
    /// A reusable vertex array defining a quad (four vertices) used to draw the floor area.
    /// </summary>
    public VertexArray Vertices = new VertexArray(PrimitiveType.Quads, 4);

    /// <summary>
    /// The color used to fill the floor quad.
    /// </summary>
    public SFML.Graphics.Color ColorFilling { get; set; } = new Color(20, 20, 20);

    /// <summary>
    /// Initializes a new instance of the <see cref="Floor"/> class with the specified fill color.
    /// </summary>
    /// <param name="color">The color to use for the floor.</param>
    public Floor(SFML.Graphics.Color color)
    {
        this.ColorFilling = color;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Floor"/> class with the default fill color.
    /// </summary>
    public Floor()
        : this(new SFML.Graphics.Color(20, 20, 20))
    { }

    /// <summary>
    /// Updates the vertex positions based on the unit's camera height and enqueues the quad for rendering.
    /// </summary>
    /// <param name="unit">The unit containing camera position and height data.</param>
    public void Render(IUnit unit)
    {
        float normalizeHeight = RenderPartsWorld.NormalizeHeigthDownPart(unit);

        Vertices[0] = new Vertex(new Vector2f(0, Screen.ScreenHeight), ColorFilling);
        Vertices[1] = new Vertex(new Vector2f(Screen.ScreenWidth, Screen.ScreenHeight), ColorFilling);
        Vertices[2] = new Vertex(new Vector2f(Screen.ScreenWidth, normalizeHeight), ColorFilling);
        Vertices[3] = new Vertex(new Vector2f(0, normalizeHeight), ColorFilling);

        Screen.OutputPriority?.AddToPriority(OutputLayer, Vertices);
    }
}
