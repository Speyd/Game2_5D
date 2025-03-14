using RayTracingLib;
using EntityLib;
using MapLib;
using SFML.System;
using RayTracingLib.Detection;
using SFML.Graphics;
using ScreenLib;
using TextureLib;
using ProtoRender.Object;


namespace DrawLib;
public static class Drawing
{
    static HitPoint hitPoint = new HitPoint();

    public static void DrawingPoint(Map map, Entity entity, int heightObj, SFML.Graphics.Color colorFill)
    {
        IObject? obstacle = Raycast.RaycastFun(map, entity);
        if (obstacle is not null && obstacle is IDrawable drawable)
        {
            hitPoint = RayDetectionX.DetermineHitObjectSides(obstacle, entity);


            float textureX = drawable.CalculateTextureX(hitPoint.UV, hitPoint.TextureWallDetermine);

            float height = drawable.BringingToStandard(heightObj);
            float textureY = RayDetectionY.GetTextureCoordinate(hitPoint, drawable, entity, height);

            if(drawable.IsInsideTexture(textureX, textureY))
                return;


            float addHeight = height >= heightObj ? 0f : heightObj;
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
        IObject? obstacle = Raycast.RaycastFun(map, entity);

        if (obstacle is not null && obstacle is IDrawable drawable)
        {
            hitPoint = RayDetectionX.DetermineHitObjectSides(obstacle, entity);


            float textureX = drawable.CalculateTextureX(hitPoint.UV, hitPoint.TextureWallDetermine);

            float height = drawable.BringingToStandard(sprite.Texture.Size.Y);
            float textureY = RayDetectionY.GetTextureCoordinate(hitPoint, drawable, entity, height);

            if (drawable.IsInsideTexture(textureX, textureY))
                return;

            float addHeight = height >= sprite.Texture.Size.Y ? 0f : sprite.Texture.Size.Y;

            float x = textureX - sprite.Texture.Size.X / 2 + addHeight;
            float y = textureY + sprite.Texture.Size.X / 2;
            Vector2f dotPosition = new Vector2f(x, y);
            sprite.Position = dotPosition;

            drawable.DrawObject(sprite);
        }
    }
}
