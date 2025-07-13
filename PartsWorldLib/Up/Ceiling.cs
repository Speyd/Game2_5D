using ProtoRender.Object;
using ScreenLib;
using ScreenLib.Output;
using SFML.Graphics;
using SFML.System;

namespace PartsWorldLib.Up;
/// <summary>
/// Represents the ceiling part of the scene, which renders a colored rectangle
/// at the top of the screen up to a normalized height, depending on the unit.
/// </summary>
public class Ceiling : IUpPart
{
    /// <summary>
    /// Gets or sets the output rendering layer priority for this surface.
    /// Determines on which layer the ceiling is drawn.
    /// </summary>
    public OutputPriorityType OutputLayer { get; set; } = OutputPriorityType.Background;

    /// <summary>
    /// Gets or sets the vertex array that defines the shape and color of the ceiling.
    /// </summary>
    public VertexArray Vertices = new VertexArray(PrimitiveType.Quads, 4);

    /// <summary>
    /// Gets or sets the fill color of the ceiling.
    /// </summary>
    public Color ColorFilling { get; set; } = new Color(100, 149, 237);

    /// <summary>
    /// Initializes a new instance of the <see cref="Ceiling"/> class
    /// with the specified fill color.
    /// </summary>
    /// <param name="color">The color to fill the ceiling with.</param>
    public Ceiling(SFML.Graphics.Color color)
    {
        this.ColorFilling = color;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Ceiling"/> class
    /// with the default color (Cornflower Blue).
    /// </summary>
    public Ceiling()
        : this(new SFML.Graphics.Color(100, 149, 237))
    { }

    /// <summary>
    /// Renders the ceiling based on the given unit's position.
    /// Computes the height where the ceiling should end and
    /// sends the vertices to the screen's output priority queue.
    /// </summary>
    /// <param name="unit">The unit whose position is used to normalize the ceiling height.</param>
    public void Render(IUnit unit)
    {
        float normalizeHeight = RenderPartsWorld.NormalizeHeigthUpPart(unit);

        Vertices[0] = new Vertex(new Vector2f(0, 0), ColorFilling);
        Vertices[1] = new Vertex(new Vector2f(Screen.ScreenWidth, 0), ColorFilling);
        Vertices[2] = new Vertex(new Vector2f(Screen.ScreenWidth, normalizeHeight), ColorFilling);
        Vertices[3] = new Vertex(new Vector2f(0, normalizeHeight), ColorFilling);

        Screen.OutputPriority?.AddToPriority(OutputLayer, Vertices);
    }
}