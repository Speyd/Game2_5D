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
        private float BringingToStandard(TexturedWall wall, float radius)
        {
            if (wall.CurrentRenderTexture is null)
                throw new Exception("CurrentRenderTexture is null(BringingToStandard)");

            return radius * (wall.CurrentRenderTexture.Base.Height / TextureObstacle.BaseHeight);
        }

        public float GetTextureXCoordinate(TexturedWall wall, Entity entity)
        {
            HitPoint hitPoint = RayDetectionX.DetermineWallAllSides(wall, entity);

            wall.CurrentRenderTexture = wall.MultiTextured[hitPoint.TextureWallDetermine];
            if (wall.CurrentRenderTexture is null)
                throw new Exception("CurrentRenderTexture is null (GetTextureCoordinate)");

            float textureX = hitPoint.UV.X > hitPoint.UV.Y ? hitPoint.UV.X : hitPoint.UV.Y;
            textureX *= wall.CurrentRenderTexture.Base.Width / Screen.Setting.Scale;

            return textureX - (float)Math.Pow(wall.CurrentRenderTexture.Base.Height / TextureObstacle.BaseHeight, 4.5f);
        }


        public void DrawingPoint(Map map, Entity entity, int radiusPoint)
        {
            Obstacle? obstacle = Raycast.RaycastFun(map, entity).Where(o => o is IDrawable).FirstOrDefault();

            if (obstacle is not null && obstacle is TexturedWall wall)// obstacle is TexturedWall wall
            {
                hitPoint = RayDetectionX.DetermineWallAllSides(wall, entity);
                float textureX = GetTextureXCoordinate(wall, entity);

                float radius = BringingToStandard(wall, radiusPoint);
                float textureY = RayDetectionY.GetTextureCoordinate(hitPoint, wall, entity, radius);
                Vector2f dotPosition = new Vector2f(textureX, textureY);

                CircleShape dot = new CircleShape(radius)
                {
                    FillColor = SFML.Graphics.Color.Black,
                    Position = dotPosition
                };

                //Sprite s = new Sprite(new Texture(@"Resources\Image\Sprite\Devil\1.png"));
                //dotPosition = new Vector2f(dotPosition.X - s.Texture.Size.X / 2, dotPosition.Y - s.Texture.Size.Y / 2);
                //s.Position = dotPosition;
                if (wall.CurrentRenderTexture is not null)
                {
                    wall.CurrentRenderTexture.Mod.Draw(dot);
                    wall.CurrentRenderTexture.Mod.Display();
                }
            }
        }
    }
}
