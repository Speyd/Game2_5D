using HitBoxLib.Data.HitBoxObject;
using HitBoxLib.Data.Observer;
using ProtoRender.Map;
using ProtoRender.RenderAlgorithm;
using ProtoRender.RenderInterface;
using ScreenLib;
using DataPipes;

namespace RenderLib.HitBox;
/// <summary>
/// Provides functionality for visualizing hitboxes of map objects, useful for debugging rendering logic or spatial behavior.
/// </summary>
/// <remarks>
/// Allows filtering hitbox visualization by object type and distance to the observing unit.
/// </remarks>
public static class VisualizerHitBox
{
    /// <summary>
    /// The type of hitboxes that will be rendered.
    /// </summary>
    /// <remarks>
    /// Determines the category of objects whose hitboxes should be visualized. Options include all objects, only ray-renderable, or self-renderable types.
    /// </remarks>
    public static VisualizerHitBoxType VisualizerType { get; set; } = VisualizerHitBoxType.None;

    /// <summary>
    /// Limits rendering to a maximum distance from the observer to reduce clutter and improve performance.
    /// </summary>
    /// <remarks>
    /// If set to true, hitboxes beyond the rendering unit's <c>MaxRenderTile</c> distance will not be rendered.
    /// </remarks>
    public static bool IsDistanceLimited { get; set; } = true;

    private const float indexStepDepth = 0.0000001f;
    /// <summary>
    /// Renders the hitboxes of obstacles on the map based on the configured <see cref="VisualizerType"/> and distance limit.
    /// </summary>
    /// <param name="unit">The unit performing the rendering, typically the player or observer.</param>
    public static void Render(ProtoRender.Object.IUnit unit)
    {
        if (unit.Map is null)
            return;

        float index = indexStepDepth;
        float step = indexStepDepth;
        ObserverInfo observerInfo = unit.GetObserverInfo();

        Parallel.ForEach(unit.Map.Obstacles.Values, Screen.Setting.ParallelOptions, obstList =>
        {
            foreach(var obstacle in obstList.Keys)
            {
                RenderInfo hitboxObjectInfo = obstacle.GetRenderHitBoxInfo();
                if (IsDistanceLimited && MathUtils.CalculateDistance(hitboxObjectInfo.position, observerInfo.position) > unit.MaxRenderTile)
                    return;

                foreach(var box in HitBoxLib.Operations.Render.BuildHitBoxMesh(hitboxObjectInfo, observerInfo))
                {
                    switch (VisualizerType)
                    {
                        case VisualizerHitBoxType.VisualizeRayRenderable when obstacle is IRayRenderable:
                            ZBuffer.AddToZBuffer(box, index);
                            break;
                        case VisualizerHitBoxType.VisualizeSelfRenderable when obstacle is ISelfRenderable:
                            ZBuffer.AddToZBuffer(box, index);
                            break;
                        case VisualizerHitBoxType.VisualizeAll:
                            ZBuffer.AddToZBuffer(box, index);
                            break;
                    }
                    index += step;
                }
            }
        });
    }
}
