using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using EntityLib;
using HitBoxLib;
using MapLib;
using ObstacleLib.SpriteLib;
using Render;
using Render.RenderInterface;
using Render.ZBufferRender;
using SFML.Graphics;
using SFML.System;

namespace BresenhamAlgorithm
{
    public class VisualizerHitBox(Map map)
    {
        public VisualizerType VisualizerType { get; set; } = VisualizerType.None;
        public bool IsDistanceLimited { get; set; } = true;

        public void Render(Entity entity)
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
                            ZBuffer.AddToZBuffer(HitBoxLib.Render.BuildHitBoxMesh(hitboxObjectInfo, observerInfo), index);
                            break;
                        case VisualizerType.VisualizeSelfRenderable when obstacle is ISelfRenderable:
                            ZBuffer.AddToZBuffer(HitBoxLib.Render.BuildHitBoxMesh(hitboxObjectInfo, observerInfo), index);
                            break;
                        case VisualizerType.VisualizeAll:
                            ZBuffer.AddToZBuffer(HitBoxLib.Render.BuildHitBoxMesh(hitboxObjectInfo, observerInfo), index);
                            break;
                    }
                    index += step;
                }
            }
        }

        public float CalculateDistance(Vector2f point1, Vector2f point2)
        {
            float deltaX = point2.X - point1.X;
            float deltaY = point2.Y - point1.Y;

            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
    }
}
