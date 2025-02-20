using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLib;
using HitBoxLib;
using MapLib;
using ObstacleLib.SpriteLib;
using Render.ZBufferRender;
using SFML.Graphics;

namespace BresenhamAlgorithm
{
    public class VisualizerHitBox(Map map)
    {
        public void Render(Entity entity)
        {
            float index = 0.0000001f;
            float step = 0.0000001f;
            ObserverInfo observerInfo = entity.GetObserverInfo();

            foreach (var obstList in map.Obstacles)
            {
                foreach (var obstacle in obstList.Value)
                {
                    if (obstacle is SpriteObstacle sprite)
                    {
                        HitboxObjectInfo hitboxObjectInfo = obstacle.GetHitboxObjectInfo();
                        VertexArray vertex = HitBoxLib.Render.BuildHitBoxMesh(hitboxObjectInfo, observerInfo);

                        ZBuffer.AddToZBuffer(vertex, index);
                    }
                    index += step;
                }
            }
        }
    }
}
