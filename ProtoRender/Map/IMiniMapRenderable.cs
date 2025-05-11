
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLib;


namespace ProtoRender.Map;
/// <summary>
/// Defines rendering behavior for objects displayed on a minimap, including scaling, color, and texture appearance.
/// </summary>
public interface IMiniMapRenderable
{
    /// <summary>
    /// Gets or sets the scale factor for the object's size on the minimap.
    /// </summary>
    public float SizeScale { get; set; }

    /// <summary>
    /// Gets or sets the scale factor for the object's position on the minimap.
    /// </summary>
    public float PositionScale { get; set; }

    /// <summary>
    /// Gets or sets the fill color used to represent the object on the minimap.
    /// </summary>
    public SFML.Graphics.Color ColorInMap { get; set; }

    /// <summary>
    /// Gets or sets the texture used to represent the object on the minimap, if any.
    /// </summary>
    public TextureObstacle? TextureInMiniMap { get; set; }

    /// <summary>
    /// Fills the given rectangle shape with a solid color and optional outline for minimap rendering.
    /// </summary>
    /// <param name="rectangleShape">The rectangle shape to be filled.</param>
    /// <param name="OutlineThickness">The thickness of the shape's outline (default is 1).</param>
    void FillingColorShape(RectangleShape rectangleShape, float OutlineThickness = 1);

    /// <summary>
    /// Applies a texture to the given rectangle shape for minimap rendering.
    /// </summary>
    /// <param name="rectangleShape">The rectangle shape to be textured.</param>
    void FillingTextureShape(RectangleShape rectangleShape);

    /// <summary>
    /// Calculates the size offset based on a base offset and the current size scale.
    /// </summary>
    /// <param name="baseOffset">The base size offset.</param>
    /// <returns>The scaled size offset for the minimap.</returns>
    float SizeOffsetMap(float baseOffset);

    /// <summary>
    /// Calculates the position offset based on a base offset and the current position scale.
    /// </summary>
    /// <param name="baseOffset">The base position offset.</param>
    /// <returns>The scaled position offset for the minimap.</returns>
    float CoordinatesOffsetMap(float baseOffset);

    /// <summary>
    /// Converts world or map coordinates to minimap coordinates based on the tile size.
    /// </summary>
    /// <param name="mapTile">The size of a single map tile.</param>
    /// <returns>The position on the minimap corresponding to the object's world position.</returns>
    Vector2f ConversionToMapCoordinates(float mapTile);
}
