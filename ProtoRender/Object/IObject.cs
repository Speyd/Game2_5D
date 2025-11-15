using HitBoxLib.Data.HitBoxObject;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.PositionObject;
using ProtoRender.Map;
using ProtoRender.RenderInterface;
namespace ProtoRender.Object;
/// <summary>
/// Defines an object that can be rendered, placed on a minimap, processed for collisions, and added to a map.
/// </summary>
/// <remarks>
/// The <see cref="IObject"/> interface combines rendering, minimap, and hitbox processing capabilities,
/// and allows for objects to be copied and moved on the map while supporting passability checks.
/// </remarks>
public interface IObject : IRenderable, IMiniMapRenderable, IHitBoxProcessor, IMapAdder, ITextureProvider
{
    /// <summary>
    /// Globally unique identifier for the object.
    /// </summary>
    public Guid UUID { get; set; }

    /// <summary>
    /// An optional event that is triggered when the object's position changes.
    /// </summary>
    public Action<IObject>? OnPositionChanged { get; set; }
    /// <summary>
    /// The map object to which this object belongs
    /// </summary>
    IMap? Map { get; set; }
    /// <summary>
    /// Gets the X coordinate of the object.
    /// </summary>
    public Coordinate X { get; init; }

    /// <summary>
    /// Gets the Y coordinate of the object.
    /// </summary>
    public Coordinate Y { get; init; }

    /// <summary>
    /// Gets the Z coordinate of the object (elevation or depth).
    /// </summary>
    public Coordinate Z { get; init; }

    /// <summary>
    /// Indicates whether the object is currently moving along any of its coordinates: X, Y, or Z.
    /// This flag will be true if movement occurs in any direction, including horizontal and vertical (elevation/depth).
    /// </summary>
    public bool IsMoving { get; }


    /// <summary>
    /// Cell in map(X axis), use Tile
    /// </summary>
    public int CellX { get; set; }
    /// <summary>
    /// Cell in map(Y axis), use Tile
    /// </summary>
    public int CellY { get; set; }

    /// <summary>
    /// Gets the hitbox of the object, used for collision detection.
    /// </summary>
    public HitBox HitBox { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the object is passable or impassable.
    /// </summary>
    public bool IsPassability { get; set; }

    /// <summary>
    /// If true, ignores collisions with objects marked as MainBox (primary bounding boxes),
    /// allowing them to be bypassed during collision checks.
    /// </summary>
    public bool IgnoreCollisonMainBox {  get; set; }

    /// <summary>
    /// Creates a deep copy of the object, duplicating all internal data and references,
    /// so that the returned object is completely independent of the original.
    /// </summary>
    /// <returns>
    /// A new instance of the object with fully copied values, including deep copies of all referenced objects.
    /// </returns>
    public IObject GetDeepCopy();

    /// <summary>
    /// Creates a copy of the object.
    /// </summary>
    /// <returns>A new instance of the object that is a copy of the current one.</returns>
    public IObject GetCopy();

}
