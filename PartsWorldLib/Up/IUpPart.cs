using ProtoRender.Object;
using ScreenLib.Output;

namespace PartsWorldLib.Up;
/// <summary>
/// Defines an object that renders an upper portion of the scene (e.g., ceiling or sky).
/// </summary>
public interface IUpPart
{
    /// <summary>
    /// Gets or sets the output rendering layer priority for this surface.
    /// </summary>
    public OutputPriorityType OutputLayer { get; set; }

    /// <summary>
    /// Performs rendering logic for the given unit, drawing the upper segment based on unit state.
    /// </summary>
    /// <param name="unit">The unit providing camera position, angle, and height information.</param>
    public void Render(IUnit unit);
}
