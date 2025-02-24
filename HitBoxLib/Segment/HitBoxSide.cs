using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HitBoxLib.PositionObject;
using HitBoxLib.Segment.SignsTypeSide;

namespace HitBoxLib.HitBoxSegment;
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

    public HitBoxSide(Coordinate coordinate, SideSize sideSize, double offset)
    {
        SideSize = sideSize;

        CoordinatePlane = coordinate.CoordinatePlane;
        Side = coordinate.Axis;
        OrginalSide = Side;
        Offset = offset;
    }
    public HitBoxSide(CoordinatePlane coordinatePlane, SideSize sideSize, double offset)
    {
        SideSize = sideSize;

        CoordinatePlane = coordinatePlane;
        Offset = offset;
    }
    public HitBoxSide(CoordinatePlane coordinatePlane, SideSize sideSize)
    {
        SideSize = sideSize;

        CoordinatePlane = coordinatePlane;
        Side = 0;
        OrginalSide = Side;
        Offset = 0;
    }


    public void SetOffset(Coordinate coordinate, double offset)
    {
        if (coordinate.CoordinatePlane != CoordinatePlane)
            throw new Exception("Unsuitable type CoordinatePlane!");

        OrginalSide = coordinate.Axis;
        Offset = offset;
        Side = OrginalSide + offset;
    }
    public void SetOffset(double offset)
    {
        Offset = offset;
        Side = OrginalSide + Offset;
    }
    public void SetSide(Coordinate coordinate)
    {
        if (coordinate.CoordinatePlane != CoordinatePlane)
            throw new Exception("Unsuitable type CoordinatePlane!");

        OrginalSide = coordinate.Axis;
        Side = coordinate.Axis + Offset;
    }

}
