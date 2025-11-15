using RayTracingLib;
using ProtoRender.Map;
using SFML.System;
using RayTracingLib.Detection;
using SFML.Graphics;
using ProtoRender.Object;
using System.Collections.Concurrent;


namespace DrawLib;
/// <summary>
/// The <see cref="Drawing"/> class provides methods for drawing graphical objects (points and sprites) on a map or object based on raycasting and hit detection.
/// It includes both synchronous and asynchronous methods for drawing and rendering points and sprites based on the interactions between the unit and obstacles.
/// </summary>
public static class Drawing
{
    private static readonly ConcurrentDictionary<Type, Action<IObject?, IUnit, Drawable>> _renderers = new();
    private static readonly ConcurrentDictionary<Type, Action<IUnit, Drawable>> _renderersOnMap = new();
    /// <summary>
    /// Registers a custom draw function for a specific type of <see cref="Drawable"/>.
    /// This allows dynamic dispatch of rendering logic for different drawable types.
    /// </summary>
    /// <typeparam name="T">The type of the drawable object.</typeparam>
    /// <param name="drawFunc">The draw function to invoke for the given type.</param>
    public static void Register<T>(Action<IObject?, IUnit, T> drawFunc) where T : Drawable
    {
        _renderers[typeof(T)] = (obstacle, unit, drawable) => drawFunc(obstacle, unit, (T)drawable);
    }
    /// <summary>
    /// Registers a custom draw function for a specific type of <see cref="Drawable"/>.
    /// This allows dynamic dispatch of rendering logic for different drawable types.
    /// </summary>
    /// <typeparam name="T">The type of the drawable object.</typeparam>
    /// <param name="drawFunc">The draw function to invoke for the given type.</param>
    public static void RegisterOnMap<T>(Action<IUnit, T> drawFunc) where T : Drawable
    {
        _renderersOnMap[typeof(T)] = (unit, drawable) => drawFunc(unit, (T)drawable);
    }

    static Drawing()
    {
        Register<Sprite>(DrawingSprite);
        Register<CircleShape>(DrawingCircle);

        RegisterOnMap<Sprite>(DrawingSpriteOnMap);
        RegisterOnMap<CircleShape>(DrawingCircleOnMap);
    }


    static HitPoint hitPoint = new HitPoint();
    /// <summary>
    /// Renders a <see cref="CircleShape"/> on a specified obstacle, adjusting for hit detection and texture mapping.
    /// </summary>
    /// <param name="obstacle">The obstacle that the ray intersects.</param>
    /// <param name="unit">The unit from which the ray originates.</param>
    /// <param name="circleShape">The base circle shape to draw.</param>
    public static void DrawingCircle(IObject? obstacle, IUnit unit, CircleShape circleShape)
    {
        if (obstacle is not null && obstacle is IDrawable drawable)
        {
            hitPoint = RayDetectionX.DetermineHitObjectSides(obstacle, unit);
            float radius = circleShape.Radius;

            float textureX = drawable.CalculateTextureX(hitPoint.UV, hitPoint.TextureWallDetermine);
            float newRadius = (drawable.BringingToStandardHeight(radius) + drawable.BringingToStandardWidth(radius)) / 2;
            float textureY = RayDetectionY.GetTextureCoordinate(hitPoint, drawable, unit, newRadius);

            if (drawable.IsInsideTexture(textureX, textureY))
                return;

            float addHeight = newRadius >= radius ? 0f : radius;
            Vector2f pointPosition = new Vector2f(textureX + addHeight, textureY);

            CircleShape point = new CircleShape(circleShape)
            {
                Radius = newRadius,
                Position = pointPosition
            };

            drawable.DrawObjectAsync(point);
        }
    }
    /// <summary>
    /// Performs raycasting from a unit's position on the given map and draws a <see cref="CircleShape"/> on the detected obstacle.
    /// </summary>
    /// <param name="map">The map containing obstacles.</param>
    /// <param name="unit">The unit from which the ray is cast.</param>
    /// <param name="point">The circle shape to draw.</param>
    public static void DrawingCircleOnMap(IUnit unit, CircleShape point)
    {
        IObject? obstacle = Raycast.RaycastFun(unit).Item1;
        DrawingCircle(obstacle, unit, point);
    }

    /// <summary>
    /// Base vertical scale factor for all sprites (relative to their original size).
    /// Used to standardize the height of rendered objects.
    /// </summary>
    public static float baseHeightScaleSprite = 0.4f;

    /// <summary>
    /// Base horizontal scale factor for all sprites (relative to their original size).
    /// Helps ensure consistent visual width for objects in the scene.
    /// </summary>
    public static float baseWidthScaleSprite = 0.4f;
    /// <summary>
    /// Renders a <see cref="Sprite"/> on a specified obstacle with appropriate scaling and positioning based on texture mapping.
    /// </summary>
    /// <param name="obstacle">The obstacle that the ray intersects.</param>
    /// <param name="unit">The unit from which the ray originates.</param>
    /// <param name="sprite">The sprite to render.</param>
    public static void DrawingSprite(IObject? obstacle, IUnit unit, Sprite sprite)
    {
        if (obstacle is not null && obstacle is IDrawable drawable)
        {
            hitPoint = RayDetectionX.DetermineHitObjectSides(obstacle, unit);

            float textureX = drawable.CalculateTextureX(hitPoint.UV, hitPoint.TextureWallDetermine);

            float height = drawable.BringingToStandardHeight(sprite.Texture.Size.Y);
            float width = drawable.BringingToStandardWidth(sprite.Texture.Size.X);

            float textureY = RayDetectionY.GetTextureCoordinate(hitPoint, drawable, unit, 0);
            if (drawable.IsInsideTexture(textureX, textureY))
                return;

            float scaleY = (height / (sprite.Texture.Size.Y / sprite.Scale.Y));
            float scaleX = (width / (sprite.Texture.Size.X / sprite.Scale.X));

            float x = textureX - (width * scaleX) / 2;
            float y = textureY - (sprite.Texture.Size.Y * scaleY) / 2;

            SFML.Graphics.Sprite newSprite = new SFML.Graphics.Sprite(sprite)
            {
                Scale = new Vector2f(scaleX, scaleY),
                Origin = new Vector2f(0, 0),
                Position = new Vector2f(x, y),
            };
            drawable.DrawObjectAsync(newSprite);
        }
    }

    /// <summary>
    /// Performs raycasting from a unit's position on the given map and draws a <see cref="Sprite"/> on the detected obstacle.
    /// </summary>
    /// <param name="unit">The unit from which the ray is cast.</param>
    /// <param name="sprite">The sprite to render.</param>
    public static void DrawingSpriteOnMap(IUnit unit, Sprite sprite)
    {
        IObject? obstacle = Raycast.RaycastFun(unit).Item1;
        DrawingSprite(obstacle, unit, sprite);
    }

    /// <summary>
    /// Draws a generic <see cref="Drawable"/> object by invoking the registered rendering logic based on its runtime type.
    /// </summary>
    /// <param name="obstacle">The obstacle that the ray intersects.</param>
    /// <param name="unit">The unit from which the ray originates.</param>
    /// <param name="drawable">The drawable object to render.</param>
    /// <exception cref="NotSupportedException">Thrown when no renderer is registered for the drawable type.</exception>
    public static void DrawingObject(IObject? obstacle, IUnit unit, Drawable drawable)
    {
        if (drawable is null)
            throw new ArgumentNullException(nameof(drawable));

        var type = drawable.GetType();

        if (_renderers.TryGetValue(type, out var renderer))
            renderer(obstacle, unit, drawable);
        else
            throw new NotSupportedException($"No renderer registered for drawable type {type.Name}");
    }
    /// <summary>
    /// Performs raycasting from a unit on the map and draws a generic <see cref="Drawable"/> object on the detected obstacle.
    /// </summary>
    /// <param name="map">The map containing obstacles.</param>
    /// <param name="unit">The unit performing the raycast.</param>
    /// <param name="drawable">The drawable object to render.</param>
    public static void DrawingObjectOnMap(IUnit unit, Drawable drawable)
    {
        IObject? obstacle = Raycast.RaycastFun(unit).Item1;
        DrawingObject(obstacle, unit, drawable);
    }

    /// <summary>
    /// Calculates the drawing coordinates on the texture of an object hit by a ray.
    /// </summary>
    /// <param name="obstacle">The object hit by the ray. Must implement <see cref="IDrawable"/>.</param>
    /// <param name="unit">The unit from which the ray is cast.</param>
    /// <returns>
    /// Returns the texture coordinates as a <see cref="Vector2f"/>.
    /// Returns <c>null</c> if the object does not implement <see cref="IDrawable"/> 
    /// or if the point is inside the texture (invalid for drawing).
    /// </returns>
    public static Vector2f? GetDrawingCoordinte(IObject? obstacle, IUnit unit)
    {
        if (obstacle is not null && obstacle is IDrawable drawable)
        {
            HitPoint hitPoint = RayDetectionX.DetermineHitObjectSides(obstacle, unit);

            float textureX = drawable.CalculateTextureX(hitPoint.UV, hitPoint.TextureWallDetermine);
            float textureY = RayDetectionY.GetTextureCoordinate(hitPoint, drawable, unit, 0);

            if (drawable.IsInsideTexture(textureX, textureY))
                return default;

            return new Vector2f(textureX, textureY);
        }

        return default;
    }
}
