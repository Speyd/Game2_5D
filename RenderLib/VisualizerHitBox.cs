using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using EntityLib;
using HitBoxLib.Data.HitBoxObject;
using HitBoxLib.Data.Observer;
using MapLib;
using ProtoRender;
using ProtoRender.RenderAlgorithm;
using ProtoRender.RenderInterface;
using SFML.Graphics;
using SFML.System;


namespace RenderLib.HitBox;
/// <summary> Visualizes it boxes of objects </summary>
public static class VisualizerHitBox
{
    /// <summary> The type of hitboxes that will be rendered </summary>
    public static VisualizerHitBoxType VisualizerType { get; set; } = VisualizerHitBoxType.None;
    /// <summary> Limits rendering to a maximum distance </summary>
    public static bool IsDistanceLimited { get; set; } = true;
    private const float indexStepDepth = 0.0000001f;

    public static void Render(Map map, Entity entity)
    {
        float index = indexStepDepth;
        float step = indexStepDepth;

        ObserverInfo observerInfo = entity.GetObserverInfo();

        foreach (var obstList in map.Obstacles)
        {
            foreach (var obstacle in obstList.Value)
            {
                RenderInfo hitboxObjectInfo = obstacle.GetRenderHitBoxInfo();
                if (IsDistanceLimited && CalculateDistance(hitboxObjectInfo.position, observerInfo.position) > entity.MaxRenderTile)
                    continue;

                switch (VisualizerType)
                {
                    case VisualizerHitBoxType.VisualizeRayRenderable when obstacle is IRayRenderable:
                        ZBuffer.AddToZBuffer(HitBoxLib.Operations.Render.BuildHitBoxMesh(hitboxObjectInfo, observerInfo), index);
                        break;
                    case VisualizerHitBoxType.VisualizeSelfRenderable when obstacle is ISelfRenderable:
                        ZBuffer.AddToZBuffer(HitBoxLib.Operations.Render.BuildHitBoxMesh(hitboxObjectInfo, observerInfo), index);
                        break;
                    case VisualizerHitBoxType.VisualizeAll:
                        ZBuffer.AddToZBuffer(HitBoxLib.Operations.Render.BuildHitBoxMesh(hitboxObjectInfo, observerInfo), index);
                        break;
                }
                index += step;
            }
        }
    }

    public static float CalculateDistance(Vector2f point1, Vector2f point2)
    {
        float deltaX = point2.X - point1.X;
        float deltaY = point2.Y - point1.Y;

        return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }
}
