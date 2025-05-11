using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;
using TextureLib;


namespace RayTracingLib.Detection;
/// <summary>
/// Represents a hit point in a 3D space where a ray intersects an object or wall.
/// It stores information about the texture coordinates (UV), distances to various points, 
/// and determines the side of the wall and texture associated with the hit point.
/// </summary>
public class HitPoint
{
    /// <summary>
    /// Gets or sets the UV texture coordinate system at the hit point.
    /// </summary>
    public Vector2f UV { get; set; }

    /// <summary>
    /// Gets or sets the distance from the hit point to the original point (e.g., the viewpoint or ray origin).
    /// This distance is scaled by the screen width for rendering purposes.
    /// </summary>
    public float DistanceToPoint { get; set; } = 0;

    /// <summary>
    /// Gets or sets the distance from the hit point to the wall or object.
    /// This distance is also scaled by the screen width for rendering.
    /// </summary>
    public float DistanceToWall { get; set; } = 0;

    /// <summary>
    /// Gets or sets the distance from the hit point to the wall or object without considering tile size.
    /// This value can be used for non-tile-based calculations.
    /// </summary>
    public float DistanceToWallWithoutTile { get; set; } = 0;

    /// <summary>
    /// Gets or sets the side of the wall that the hit point is associated with.
    /// This is used to determine the orientation of the object hit.
    /// </summary>
    public ObjectSide WallDetermine { get; set; } = ObjectSide.Error;

    /// <summary>
    /// Gets or sets the side of the wall's texture that the hit point corresponds to.
    /// This determines which part of the texture should be used for rendering the object at the hit point.
    /// </summary>
    public ObjectSide TextureWallDetermine { get; set; } = ObjectSide.Error;

    /// <summary>
    /// Initializes a new instance of the <see cref="HitPoint"/> class with specified values.
    /// </summary>
    /// <param name="uV">The UV texture coordinates at the hit point.</param>
    /// <param name="distanceToPoint">The distance from the hit point to the original point.</param>
    /// <param name="distanceToWall">The distance from the hit point to the wall or object.</param>
    /// <param name="wallDetermine">The side of the wall at the hit point.</param>
    /// <param name="textureWallDetermine">The side of the texture corresponding to the hit point.</param>
    public HitPoint(Vector2f uV,
        float distanceToPoint, float distanceToWall,
        ObjectSide wallDetermine, ObjectSide textureWallDetermine)
    {
        UV = uV;
        DistanceToPoint = distanceToPoint / Screen.Setting.Tile * Screen.MultWidth;
        DistanceToWall = distanceToWall / Screen.Setting.Tile * Screen.MultWidth;
        DistanceToWallWithoutTile = distanceToWall * Screen.MultWidth;
        WallDetermine = wallDetermine;
        TextureWallDetermine = textureWallDetermine;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HitPoint"/> class with default values.
    /// </summary>
    public HitPoint()
    {
        UV = new Vector2f(0, 0);
        DistanceToPoint = 0;
        DistanceToWall = 0;
        WallDetermine = ObjectSide.Error;
        TextureWallDetermine = ObjectSide.Error;
    }
}
