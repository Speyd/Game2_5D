using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HitBoxLib.PositionObject;
using HitBoxLib.Segment.SignsTypeSide;

namespace HitBoxLib.HitBoxSegment;
/// <summary>Hitbox side</summary>
public class HitBoxSide
{

    /// <summary>
    /// Hitbox side axis
    /// </summary>
    public CoordinatePlane CoordinatePlane { get; init; }

    /// <summary>
    /// Hitbox side
    /// </summary>
    public double Side { get; private set; } = 0;

    /// <summary>
    /// Hitbox center
    /// </summary>
    public double OrginalSide { get; private set; } = 0;


    private double _offset = 0;
    /// <summary>
    /// A number that represents the offset from the center of the hitbox.
    /// </summary>
    public double Offset
    {
        get => _offset;
        private set => _offset = SideSize == SideSize.Smaller ? -value : value;
    }

    /// <summary>
    /// If it is the smaller side, then any Offset will be converted to a negative number and vice versa.
    /// </summary>
    public SideSize SideSize { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HitBoxSide"/> class by copying values from another instance.
    /// </summary>
    /// <param name="hitBoxSide">The instance to copy from.</param>
    public HitBoxSide(HitBoxSide hitBoxSide)
    {
        SideSize = hitBoxSide.SideSize;

        CoordinatePlane = hitBoxSide.CoordinatePlane;
        Side = hitBoxSide.Side;
        OrginalSide = hitBoxSide.OrginalSide;
        _offset = hitBoxSide.Offset;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="HitBoxSide"/> class using a coordinate, side size, and offset.
    /// </summary>
    /// <param name="coordinate">The coordinate from which to derive the side value.</param>
    /// <param name="sideSize">The size of the side (Smaller or Larger).</param>
    /// <param name="offset">The offset to apply to the side.</param>
    public HitBoxSide(Coordinate coordinate, SideSize sideSize, double offset)
    {
        SideSize = sideSize;

        CoordinatePlane = coordinate.CoordinatePlane;
        Side = coordinate.Axis;
        OrginalSide = Side;
        SetOffset(offset);

    }
    /// <summary>
    /// Initializes a new instance of the <see cref="HitBoxSide"/> class with a given coordinate plane, side size, and offset.
    /// </summary>
    /// <param name="coordinatePlane">The coordinate plane (X, Y, or Z).</param>
    /// <param name="sideSize">The size of the side (Smaller or Larger).</param>
    /// <param name="offset">The offset to apply to the side.</param>
    public HitBoxSide(CoordinatePlane coordinatePlane, SideSize sideSize, double offset)
    {
        SideSize = sideSize;
        CoordinatePlane = coordinatePlane;

        SetOffset(offset);
    }

    [JsonConstructor]
    public HitBoxSide(CoordinatePlane coordinatePlane, int side, int orginalSide, float offset, SideSize sideSize)
    {
        CoordinatePlane = coordinatePlane;
        Side = side;
        OrginalSide = orginalSide;
        _offset = offset;
        SideSize = sideSize;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="HitBoxSide"/> class with default values.
    /// </summary>
    /// <param name="coordinatePlane">The coordinate plane (X, Y, or Z).</param>
    /// <param name="sideSize">The size of the side (Smaller or Larger).</param>
    public HitBoxSide(CoordinatePlane coordinatePlane, SideSize sideSize)
    {
        SideSize = sideSize;

        CoordinatePlane = coordinatePlane;
        Side = 0;
        OrginalSide = Side;
        Offset = 0;
    }

    /// <summary>
    /// Sets a new offset and updates the side value using the provided coordinate.
    /// </summary>
    /// <param name="coordinate">The coordinate used to calculate the original side value.</param>
    /// <param name="offset">The offset to apply.</param>
    /// <exception cref="Exception">
    /// Thrown if the coordinate's plane does not match the side's coordinate plane.
    /// </exception>
    public void SetOffset(Coordinate coordinate, double offset)
    {
        if (coordinate.CoordinatePlane != CoordinatePlane)
            throw new Exception("Unsuitable type CoordinatePlane!");

        OrginalSide = coordinate.Axis;
        Offset = offset;
        Side = OrginalSide + offset;
    }
    /// <summary>
    /// Sets a new offset using the existing original side value.
    /// </summary>
    /// <param name="offset">The offset to apply to the original side.</param>
    public void SetOffset(double offset)
    {
        Offset = offset;
        Side = OrginalSide + Offset;
    }
    /// <summary>
    /// Sets the side value based on a new coordinate, using the existing offset.
    /// </summary>
    /// <param name="coordinate">The coordinate used to update the original side value.</param>
    /// <exception cref="Exception">
    /// Thrown if the coordinate's plane does not match the side's coordinate plane.
    /// </exception>
    public void SetSide(Coordinate coordinate)
    {
        if (coordinate.CoordinatePlane != CoordinatePlane)
            throw new Exception("Unsuitable type CoordinatePlane!");

        OrginalSide = coordinate.Axis;
        Side = coordinate.Axis + Offset;
    } /// <summary>
      /// Sets the side value based on a new coordinate, using the existing offset.
      /// </summary>
      /// <param name="coordinate">The coordinate used to update the original side value.</param>
    public void SetSide(double coordinate)
    {
        OrginalSide = coordinate;
        Side = coordinate + Offset;
    }

}
