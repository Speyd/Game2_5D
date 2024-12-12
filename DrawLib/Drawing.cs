using RayTracingLib;
using MapLib.Obstacles;
using EntityLib;
using MapLib;
using MapLib.Obstacles.Texture;
using SFML.System;
using RayTracingLib.Detection;
using SFML.Graphics;
using ScreenLib;
using MapLib.Obstacles.DiversityObstacle.TexturedWallLib;
using Render;

namespace DrawLib
{
    public class Drawing()
    {
        HitPoint hitPoint;

        public void DrawingPoint(Map map, Entity entity, int heightObj)
        {
            Obstacle? obstacle = Raycast.RaycastFun(map, entity);

            if (obstacle is not null && obstacle is IDrawable drawable)
            {
                hitPoint = RayDetectionX.DetermineWallAllSides(obstacle, entity);


                float textureX = drawable.CalculateTextureX(hitPoint.UV, hitPoint.TextureWallDetermine);

                float height = drawable.BringingToStandard(heightObj);
                float textureY = RayDetectionY.GetTextureCoordinate(hitPoint, drawable, entity, height);
                Vector2f dotPosition = new Vector2f(textureX, textureY);

                CircleShape dot = new CircleShape(heightObj)
                {
                    FillColor = SFML.Graphics.Color.Black,
                    Position = dotPosition
                };


                //Sprite s = new Sprite(new Texture(@"Resources\Image\Sprite\Devil\1.png"));
                //dotPosition = new Vector2f(dotPosition.X - s.Texture.Size.X / 2, dotPosition.Y - s.Texture.Size.Y / 2);
                //s.Position = dotPosition;
                drawable.DrawObject(dot);
            }
        }
    }
}
