using RayTracingLib;
using ProtoRender.Map;
using SFML.System;
using RayTracingLib.Detection;
using SFML.Graphics;
using ProtoRender.Object;


namespace DrawLib;
/// <summary>
/// The <see cref="Drawing"/> class provides methods for drawing graphical objects (points and sprites) on a map or object based on raycasting and hit detection.
/// It includes both synchronous and asynchronous methods for drawing and rendering points and sprites based on the interactions between the unit and obstacles.
/// </summary>
public static class Drawing
{
    static HitPoint hitPoint = new HitPoint();

    /// <summary>
    /// Draws a point on the object based on hit detection, texture, and height.
    /// </summary>
    /// <param name="obstacle">The object that the ray intersects with.</param>
    /// <param name="unit">The unit interacting with the ray.</param>
    /// <param name="heightObj">The height of the object for calculating the point's position.</param>
    /// <param name="colorFill">The color fill for the drawn point.</param>
    public static void DrawingObjectPoint(IObject? obstacle, IUnit unit, int heightObj, SFML.Graphics.Color colorFill)
    {
        if (obstacle is not null && obstacle is IDrawable drawable)
        {
            hitPoint = RayDetectionX.DetermineHitObjectSides(obstacle, unit);

            float textureX = drawable.CalculateTextureX(hitPoint.UV, hitPoint.TextureWallDetermine);

            float height = drawable.BringingToStandard(heightObj);
            float textureY = RayDetectionY.GetTextureCoordinate(hitPoint, drawable, unit, height);

            if (drawable.IsInsideTexture(textureX, textureY))
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

    /// <summary>
    /// Asynchronous version of <see cref="DrawingObjectPoint"/>. Draws a point on the object based on hit detection, texture, and height.
    /// </summary>
    /// <param name="obstacle">The object that the ray intersects with.</param>
    /// <param name="unit">The unit interacting with the ray.</param>
    /// <param name="heightObj">The height of the object for calculating the point's position.</param>
    /// <param name="colorFill">The color fill for the drawn point.</param>
    public static async Task DrawingObjectPointAsync(IObject? obstacle, IUnit unit, int heightObj, SFML.Graphics.Color colorFill)
    {
        await Task.Run(() =>
        {
            if (obstacle is not null && obstacle is IDrawable drawable)
            {
                hitPoint = RayDetectionX.DetermineHitObjectSides(obstacle, unit);

                float textureX = drawable.CalculateTextureX(hitPoint.UV, hitPoint.TextureWallDetermine);

                float height = drawable.BringingToStandard(heightObj);
                float textureY = RayDetectionY.GetTextureCoordinate(hitPoint, drawable, unit, height);

                if (drawable.IsInsideTexture(textureX, textureY))
                    return;

                float addHeight = height >= heightObj ? 0f : heightObj;
                Vector2f pointPosition = new Vector2f(textureX + addHeight, textureY);

                CircleShape point = new CircleShape(height)
                {
                    FillColor = colorFill,
                    Position = pointPosition
                };

                drawable.DrawObjectAsync(point);
            }
        });
    }

    /// <summary>
    /// Draws a point on the map based on raycasting and the hit object detection.
    /// </summary>
    /// <param name="map">The map to cast the ray on.</param>
    /// <param name="unit">The unit interacting with the ray.</param>
    /// <param name="heightObj">The height of the object for calculating the point's position.</param>
    /// <param name="colorFill">The color fill for the drawn point.</param>
    public static void DrawingPoint(IMap map, IUnit unit, int heightObj, SFML.Graphics.Color colorFill)
    {
        IObject? obstacle = Raycast.RaycastFun(map, unit).Item1;
        DrawingObjectPoint(obstacle, unit, heightObj, colorFill);
    }

    /// <summary>
    /// Asynchronous version of <see cref="DrawingPoint"/>. Draws a point on the map based on raycasting and the hit object detection.
    /// </summary>
    /// <param name="map">The map to cast the ray on.</param>
    /// <param name="unit">The unit interacting with the ray.</param>
    /// <param name="heightObj">The height of the object for calculating the point's position.</param>
    /// <param name="colorFill">The color fill for the drawn point.</param>
    public static async Task DrawingPointAsync(IMap map, IUnit unit, int heightObj, SFML.Graphics.Color colorFill)
    {
        IObject? obstacle = Raycast.RaycastFun(map, unit).Item1;
        await DrawingObjectPointAsync(obstacle, unit, heightObj, colorFill);
    }

    /// <summary>
    /// Draws a sprite on the object based on hit detection, texture, and height.
    /// </summary>
    /// <param name="obstacle">The object that the ray intersects with.</param>
    /// <param name="unit">The unit interacting with the ray.</param>
    /// <param name="sprite">The sprite to draw on the object.</param>
    public static void DrawingObjectSprite(IObject? obstacle, IUnit unit, Sprite sprite)
    {
        if (obstacle is not null && obstacle is IDrawable drawable)
        {
            hitPoint = RayDetectionX.DetermineHitObjectSides(obstacle, unit);

            float textureX = drawable.CalculateTextureX(hitPoint.UV, hitPoint.TextureWallDetermine);

            float height = drawable.BringingToStandard(sprite.Texture.Size.Y);
            float textureY = RayDetectionY.GetTextureCoordinate(hitPoint, drawable, unit, height);

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

    /// <summary>
    /// Asynchronous version of <see cref="DrawingObjectSprite"/>. Draws a sprite on the object based on hit detection, texture, and height.
    /// </summary>
    /// <param name="obstacle">The object that the ray intersects with.</param>
    /// <param name="unit">The unit interacting with the ray.</param>
    /// <param name="sprite">The sprite to draw on the object.</param>
    public static async Task DrawingObjectSpriteAsync(IObject? obstacle, IUnit unit, Sprite sprite)
    {
        await Task.Run(() =>
        {
            if (obstacle is not null && obstacle is IDrawable drawable)
            {
                hitPoint = RayDetectionX.DetermineHitObjectSides(obstacle, unit);

                float textureX = drawable.CalculateTextureX(hitPoint.UV, hitPoint.TextureWallDetermine);

                float height = drawable.BringingToStandard(sprite.Texture.Size.Y);
                float textureY = RayDetectionY.GetTextureCoordinate(hitPoint, drawable, unit, height);

                if (drawable.IsInsideTexture(textureX, textureY))
                    return;

                float addHeight = height >= sprite.Texture.Size.Y ? 0f : sprite.Texture.Size.Y;

                float x = textureX - sprite.Texture.Size.X / 2 + addHeight;
                float y = textureY + sprite.Texture.Size.X / 2;
                Vector2f dotPosition = new Vector2f(x, y);
                sprite.Position = dotPosition;

                drawable.DrawObjectAsync(sprite);
            }
        });
    }

    /// <summary>
    /// Draws a sprite on the map based on raycasting and the hit object detection.
    /// </summary>
    /// <param name="map">The map to cast the ray on.</param>
    /// <param name="unit">The unit interacting with the ray.</param>
    /// <param name="sprite">The sprite to draw on the map.</param>
    public static void DrawingSprite(IMap map, IUnit unit, Sprite sprite)
    {
        IObject? obstacle = Raycast.RaycastFun(map, unit).Item1;
        DrawingObjectSprite(obstacle, unit, sprite);
    }

    /// <summary>
    /// Asynchronous version of <see cref="DrawingSprite"/>. Draws a sprite on the map based on raycasting and the hit object detection.
    /// </summary>
    /// <param name="map">The map to cast the ray on.</param>
    /// <param name="unit">The unit interacting with the ray.</param>
    /// <param name="sprite">The sprite to draw on the map.</param>
    public static async Task DrawingSpriteAsync(IMap map, IUnit unit, Sprite sprite)
    {
        IObject? obstacle = Raycast.RaycastFun(map, unit).Item1;
        await DrawingObjectSpriteAsync(obstacle, unit, sprite);
    }
}
