using SFML.Graphics;
using SFML.System;

namespace DataPipes;
/// <summary>
/// Provides utility methods for working with textures,
/// such as calculating texture coordinates (UV mapping),
/// extracting sub-rectangles, and handling common texture-related operations.
/// </summary>
public static class TextureUtils
{
    /// <summary>
    /// Returns a set of texture coordinates (UV) for the specified rectangular area of a texture.
    /// The order of the points is:
    /// - tl = top-left corner,
    /// - tr = top-right corner,
    /// - br = bottom-right corner,
    /// - bl = bottom-left corner.
    /// </summary>
    /// <param name="rect">The texture rectangle <see cref="IntRect"/>.</param>
    /// <returns>
    /// A tuple of four <see cref="Vector2f"/> representing the UV coordinates
    /// (tl, tr, br, bl) of the given rectangle.
    /// </returns>
    public static (Vector2f tl, Vector2f tr, Vector2f br, Vector2f bl) GetTexCoords(IntRect rect) =>
    (
        new(rect.Left, rect.Top),
        new(rect.Left + rect.Width, rect.Top),
        new(rect.Left + rect.Width, rect.Top + rect.Height),
        new(rect.Left, rect.Top + rect.Height)
    );

    /// <summary>
    /// Calculates the four vertex positions of a textured quad in world space,
    /// based on the given starting position, scale, and texture rectangle size.
    /// The order of the points is:
    /// - topLeft = top-left corner,
    /// - topRight = top-right corner,
    /// - bottomRight = bottom-right corner,
    /// - bottomLeft = bottom-left corner.
    /// </summary>
    /// <param name="position">The top-left starting position of the quad.</param>
    /// <param name="scale">The scaling factor applied to the quad.</param>
    /// <param name="textureRect">The rectangle defining the size of the texture region.</param>
    /// <returns>
    /// A tuple of four <see cref="Vector2f"/> representing the quad's vertex positions
    /// (topLeft, topRight, bottomRight, bottomLeft).
    /// </returns>
    public static (Vector2f topLeft, Vector2f topRight, Vector2f bottomRight, Vector2f bottomLeft)
        GetQuadPositions(Vector2f position, Vector2f scale, IntRect textureRect) =>
    (
        new(position.X, position.Y),
        new(position.X + scale.X * textureRect.Width, position.Y),
        new(position.X + scale.X * textureRect.Width, position.Y + scale.Y * textureRect.Height),
        new(position.X, position.Y + scale.Y * textureRect.Height)
    );

}
