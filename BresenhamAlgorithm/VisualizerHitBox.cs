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
using ObstacleLib.SpriteLib;
using Render;
using Render.RenderAlgorithm;
using Render.RenderInterface;
using SFML.Graphics;
using SFML.System;


namespace BresenhamAlgorithm;
/// <summary> Visualizes it boxes of objects </summary>
public static class VisualizerHitBox
{
    /// <summary> The type of hitboxes that will be rendered </summary>
    public static VisualizerType VisualizerType { get; set; } = VisualizerType.None;
    /// <summary> Limits rendering to a maximum distance </summary>
    public static bool IsDistanceLimited { get; set; } = true;


    public static void Render(Map map, Entity entity)
    {
        float index = 0.0000001f;
        float step = 0.0000001f;

        ObserverInfo observerInfo = entity.GetObserverInfo();

        foreach (var obstList in map.Obstacles)
        {
            foreach (var obstacle in obstList.Value)
            {
                HitboxObjectInfo hitboxObjectInfo = obstacle.GetHitboxObjectInfo();
                if (IsDistanceLimited && CalculateDistance(hitboxObjectInfo.position, observerInfo.position) > entity.MaxRenderTile)
                    continue;

                switch (VisualizerType)
                {
                    case VisualizerType.VisualizeRayRenderable when obstacle is IRayRenderable:
                        ZBuffer.AddToZBuffer(HitBoxLib.Operations.Render.BuildHitBoxMesh(hitboxObjectInfo, observerInfo), index);
                        break;
                    case VisualizerType.VisualizeSelfRenderable when obstacle is ISelfRenderable:
                        ZBuffer.AddToZBuffer(HitBoxLib.Operations.Render.BuildHitBoxMesh(hitboxObjectInfo, observerInfo), index);
                        break;
                    case VisualizerType.VisualizeAll:
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
