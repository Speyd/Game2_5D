namespace RenderLib.HitBox;
/// <summary>
/// The type of hitboxes that will be rendered by the visualizer.
/// </summary>
public enum VisualizerHitBoxType
{
    /// <summary>
    /// No hitboxes will be visualized.
    /// </summary>
    None,

    /// <summary>
    /// Visualize all hitboxes regardless of renderability.
    /// </summary>
    VisualizeAll,

    /// <summary>
    /// Visualize only hitboxes that are raycast-renderable.
    /// </summary>
    VisualizeRayRenderable,

    /// <summary>
    /// Visualize only the hitboxes of the unit itself.
    /// </summary>
    VisualizeSelfRenderable,
}