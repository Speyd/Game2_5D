using ProtoRender.Object;
using ScreenLib.Output;


namespace PartsWorldLib.Down;
/// <summary>
/// Defines an object that renders a lower portion of the scene (e.g., floor or ground).
/// </summary>
public interface IDownPart
{
    /// <summary>
    /// Gets or sets the output rendering layer priority for this surface.
    /// </summary>
    public OutputPriorityType OutputLayer { get; set; }

    /// <summary>
    /// Performs rendering logic for the given unit, drawing the lower segment based on unit state.
    /// </summary>
    /// <param name="unit">The unit providing camera position, angle, and height information.</param>
    public void Render(IUnit unit);
}