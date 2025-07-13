using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;
using ScreenLib.SettingScreen;
using HitBoxLib.PositionObject;
using DataPipes.Dictionary;
using HitBoxLib.Segment.SignsTypeSide;
using System.Drawing;


namespace HitBoxLib.HitBoxSegment;
/// <summary>HitBox</summary>
public class HitBox
{
    /// <summary>All the SideSize</summary>
    public static readonly SideSize[] sideSizes = (SideSize[])Enum.GetValues(typeof(SideSize));
    /// <summary>All the CoordinatePlane</summary>
    public static readonly CoordinatePlane[] coordinatePlanes = (CoordinatePlane[])Enum.GetValues(typeof(CoordinatePlane));

    /// <summary>
    /// Gets or sets the main hitbox that defines the full bounding area.
    /// </summary>
    public Box MainHitBox { get; set; }

    /// <summary>
    /// Gets or sets the segmented hitboxes that are part of the main hitbox.
    /// </summary>
    public List<Box> SegmentedHitbox { get; set; } = new();


    /// <summary>
    /// Initializes a new instance of the <see cref="HitBox"/> class with a default main hitbox and title.
    /// </summary>
    /// <param name="titleMainHitBox">The title for the main hitbox. Default is "Body".</param>
    public HitBox(string titleMainHitBox = "Body")
    {
        MainHitBox = new Box(AddAllSides(), titleMainHitBox);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HitBox"/> class by copying another hitbox.
    /// </summary>
    /// <param name="hitBox">The hitbox to copy from.</param>
    public HitBox(HitBox hitBox)
    {
        MainHitBox = new Box(hitBox.MainHitBox);
        foreach (var segment in hitBox.SegmentedHitbox)
            SegmentedHitbox.Add(new Box(segment));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HitBox"/> class with offset applied to all sides.
    /// </summary>
    /// <param name="offset">Offset applied to each hitbox side.</param>
    /// <param name="titleMainHitBox">The title for the main hitbox. Default is "Body".</param>
    public HitBox(double offset, string titleMainHitBox = "Body")
    {
        MainHitBox = new Box(AddAllSides(offset), titleMainHitBox);
    }



    /// <summary>
    /// Adds all hitbox sides (Smaller and Larger) for all coordinate planes.
    /// </summary>
    /// <returns>A dictionary containing all generated hitbox sides.</returns>
    public Dictionary<(CoordinatePlane, SideSize), HitBoxSide> AddAllSides()
    {
        Dictionary<(CoordinatePlane, SideSize), HitBoxSide> addedHitBox = new();

        foreach (var coordinatePlane in coordinatePlanes)
        {
            foreach (var sideSize in sideSizes)
            {
                addedHitBox.Add((coordinatePlane, sideSize), new HitBoxSide(coordinatePlane, sideSize));
            }
        }

        return addedHitBox;
    }
    /// <summary>
    /// Adds all hitbox sides with the specified offset for each side.
    /// </summary>
    /// <param name="offset">The offset to apply to each side.</param>
    /// <returns>A dictionary containing all generated hitbox sides with offset.</returns>
    public Dictionary<(CoordinatePlane, SideSize), HitBoxSide> AddAllSides(double offset)
    {
        Dictionary<(CoordinatePlane, SideSize), HitBoxSide> addedHitBox = new();

        foreach (var coordinatePlane in coordinatePlanes)
        {
            foreach (var sideSize in sideSizes)
            {
                addedHitBox.Add((coordinatePlane, sideSize), new HitBoxSide(coordinatePlane, sideSize, offset));
            }
        }

        return addedHitBox;
    }


    /// <summary>
    /// Checks whether all sides are included in the provided segment hitbox.
    /// </summary>
    /// <param name="segmentBox">The segment box to check.</param>
    /// <returns><c>true</c> if all sides are present; otherwise, <c>false</c>.</returns>
    public bool IsAllSidesIncludedMainHitbox(Box segmentBox)
    {
        var segmentHitBox = segmentBox.Body;
        foreach (var coordinatePlane in coordinatePlanes)
        {
            foreach (var sideSize in sideSizes)
            {
                bool check = false;
                foreach (var segment in segmentHitBox)
                {
                    if (segment.Value.SideSize == sideSize && segment.Value.CoordinatePlane == coordinatePlane)
                        check = true;
                }

                if (check == false)
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Checks whether the given segment box stays entirely within the bounds of the MainHitBox.
    /// This ensures no segment side exceeds the limits of the main hitbox.
    /// </summary>
    /// <param name="segment">The segment box to check.</param>
    /// <returns><c>true</c> if the segment is within bounds; otherwise, <c>false</c>.</returns>
    public bool IsSegmentWithinMain(Box segment)
    {
        if (IsAllSidesIncludedMainHitbox(segment) == false)
            return false;

        foreach (var coordinatePlane in coordinatePlanes)
        {
            var mainMin = MainHitBox[coordinatePlane, SideSize.Smaller]?.Offset ?? double.MinValue;
            var mainMax = MainHitBox[coordinatePlane, SideSize.Larger]?.Offset ?? double.MaxValue;

            var segMin = segment[coordinatePlane, SideSize.Smaller]?.Offset ?? double.MaxValue;
            var segMax = segment[coordinatePlane, SideSize.Larger]?.Offset ?? double.MinValue;

            if (segMin < mainMin || segMin > mainMax ||
                segMax > mainMax || segMax < mainMin)
                return false;
        }

        return true;
    }

    private void ResetSideSegmentHitBox(Box segmentHitBox)
    {
        foreach (var body in MainHitBox.Body)
        {
            segmentHitBox[body.Value.CoordinatePlane, body.Value.SideSize]?.SetSide(body.Value.OrginalSide);
        }
    }
    /// <summary>
    /// Adds a segment hitbox if it fits within the main hitbox.
    /// </summary>
    /// <param name="segmentHitBox">The segment hitbox to add.</param>
    public void AddSegmentHitBox(Box segmentHitBox)
    {
        if (IsSegmentWithinMain(segmentHitBox) == false)
            return;

        ResetSideSegmentHitBox(segmentHitBox);
        SegmentedHitbox.Add(segmentHitBox);
    }

    /// <summary>
    /// Sets an offset value for a specific side of the main hitbox.
    /// </summary>
    /// <param name="plane">The coordinate plane (e.g., X, Y, Z).</param>
    /// <param name="sideSize">The side size (Smaller or Larger).</param>
    /// <param name="offset">The offset to apply.</param>
    public void SetOffset(CoordinatePlane plane, SideSize sideSize, double offset)
    {
        MainHitBox[plane, sideSize]?.SetOffset(offset);
    }
    /// <summary>
    /// Updates all matching hitbox sides (main and segmented) with a new coordinate value.
    /// </summary>
    /// <param name="coordinate">The coordinate to set as side value.</param>
    public void SetSide(Coordinate coordinate)
    {
        foreach (var hitBoxSide in MainHitBox[coordinate.CoordinatePlane])
            hitBoxSide.SetSide(coordinate);

        foreach (var hitbox in SegmentedHitbox)
        {
            foreach (var hitBoxSide in hitbox[coordinate.CoordinatePlane])
                hitBoxSide.SetSide(coordinate);
        }
    }

    /// <summary>
    /// Gets a segmented hitbox by its title (case-insensitive).
    /// </summary>
    /// <param name="title">The title of the segmented hitbox.</param>
    /// <returns>
    /// The <see cref="Box"/> with the matching title, or <c>null</c> if not found.
    /// </returns>
    public Box? this[string title]
    {
        get
        {
            foreach (var hitbox in SegmentedHitbox)
            {
                if (hitbox.Title.ToLower() == title.ToLower())
                    return hitbox;
            }

            return null;
        }
    }
    /// <summary>
    /// Gets a segmented hitbox by its index in the collection.
    /// </summary>
    /// <param name="index">The zero-based index of the segmented hitbox.</param>
    /// <returns>
    /// The <see cref="Box"/> at the specified index, or <c>null</c> if the index is out of range.
    /// </returns>
    public Box? this[int index]
    {
        get
        {
            if (index < 0 || index >= SegmentedHitbox.Count)
                return null;

            return SegmentedHitbox[index];
        }
    }
    /// <summary>
    /// Gets the hitbox side from the main hitbox for the specified coordinate plane and side size.
    /// </summary>
    /// <param name="plane">The coordinate plane (e.g., X, Y, Z).</param>
    /// <param name="sideSize">The side size (Smaller or Larger).</param>
    /// <returns>
    /// The corresponding <see cref="HitBoxSide"/>, or <c>null</c> if not found.
    /// </returns>
    public HitBoxSide? this[CoordinatePlane plane, SideSize sideSize]
    {
        get
        {
            return MainHitBox[plane, sideSize];
        }
    }
    /// <summary>
    /// Gets a list of <see cref="HitBoxSide"/>s from the box that match the specified coordinate plane.
    /// </summary>
    /// <param name="plane">The coordinate plane to filter by.</param>
    /// <returns>A list of hitbox sides on the specified plane.</returns>
    public List<HitBoxSide> this[CoordinatePlane plane]
    {
        get
        {
            return MainHitBox[plane];
        }
    }

}
