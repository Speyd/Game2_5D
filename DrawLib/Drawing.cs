using RayTracingLib;
using EntityLib;
using MapLib;
using SFML.System;
using RayTracingLib.Detection;
using SFML.Graphics;
using ScreenLib;
using ObstacleLib;
using ObstacleLib.TexturedWallLib;
using TextureLib;
using Render.Object;


namespace DrawLib;
public static class Drawing
{
    static HitPoint hitPoint = new HitPoint();

    public static void DrawingPoint(Map map, Entity entity, int heightObj, SFML.Graphics.Color colorFill)
    {
        Obstacle? obstacle = Raycast.RaycastFun(map, entity);
        if (obstacle is not null && obstacle is IDrawable drawable)
        {
            hitPoint = RayDetectionX.DetermineWallAllSides(obstacle, entity);


            float textureX = drawable.CalculateTextureX(hitPoint.UV, hitPoint.TextureWallDetermine);

            float height = drawable.BringingToStandard(heightObj);
            float textureY = RayDetectionY.GetTextureCoordinate(hitPoint, drawable, entity, height);

            float addHeight = height > heightObj ? 0f : heightObj;
            Vector2f pointPosition = new Vector2f(textureX + addHeight, textureY);

            CircleShape point = new CircleShape(height)
            {
                FillColor = colorFill,
                Position = pointPosition
            };

            drawable.DrawObject(point);
        }
    }

    public static void DrawingSprite(Map map, Entity entity, Sprite sprite)
    {
        Obstacle? obstacle = Raycast.RaycastFun(map, entity);

        if (obstacle is not null && obstacle is IDrawable drawable)
        {
            hitPoint = RayDetectionX.DetermineWallAllSides(obstacle, entity);


            float textureX = drawable.CalculateTextureX(hitPoint.UV, hitPoint.TextureWallDetermine);

            float height = drawable.BringingToStandard(sprite.Texture.Size.Y);
            float textureY = RayDetectionY.GetTextureCoordinate(hitPoint, drawable, entity, height);

            Vector2f dotPosition = new Vector2f(textureX - sprite.Texture.Size.X / 2, textureY + sprite.Texture.Size.X / 2);
            sprite.Position = dotPosition;

            drawable.DrawObject(sprite);
        }
    }
}
