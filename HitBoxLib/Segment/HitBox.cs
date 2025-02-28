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
public class HitBox
{
    /// <summary>All the SideSize</summary>
    public static readonly SideSize[] sideSizes = (SideSize[])Enum.GetValues(typeof(SideSize));
    /// <summary>All the CoordinatePlane</summary>
    public static readonly CoordinatePlane[] coordinatePlanes = (CoordinatePlane[])Enum.GetValues(typeof(CoordinatePlane));
    
    public Box MainHitBox { get; set; }
    /// <summary>Segmented hitboxes that are part of the main hitbox</summary>
    public List<Box> SegmentedHitbox { get; set; } = new();

    public HitBox(string titleMainHitBox = "Body")
    {
        MainHitBox = new Box(AddAllSides(), titleMainHitBox);
    }
    public HitBox(double offset, string titleMainHitBox = "Body")
    {
        MainHitBox = new Box(AddAllSides(offset), titleMainHitBox);
    }



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


    public bool IsAllSidesIncludedMainHitbox(Dictionary<(CoordinatePlane, SideSize), HitBoxSide> segmentHitBox)
    {
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
    public bool IsAllSidesIncludedMainHitbox(Box segmentHitBox)
    {
        return IsAllSidesIncludedMainHitbox(segmentHitBox.Body);
    }

    public bool IsAllCoordinateIncludedMainHitbox(Dictionary<(CoordinatePlane, SideSize), HitBoxSide> segmentHitBox)
    {
        if (IsAllSidesIncludedMainHitbox(segmentHitBox) == false)
            return false;

        foreach (var coordinatePlane in coordinatePlanes)
        {
            foreach (var segment in segmentHitBox)
            {
                if (segment.Value.CoordinatePlane != coordinatePlane)
                    continue;

                if (segment.Value.SideSize == SideSize.Smaller &&
                       segment.Value.Side < MainHitBox[coordinatePlane, SideSize.Smaller]?.Side)
                {
                    return false;
                }
                else if (segment.Value.SideSize == SideSize.Larger &&
                    segment.Value.Side > MainHitBox[coordinatePlane, SideSize.Larger]?.Side)
                {
                    return false;
                }
            }
        }

        return true;
    }
    public bool IsAllCoordinateIncludedMainHitbox(Box segmentHitBox)
    {
        if (IsAllSidesIncludedMainHitbox(segmentHitBox) == false)
            return false;

        foreach (var coordinatePlane in coordinatePlanes)
        {
            foreach (var sideSize in sideSizes)
            {
                if (sideSize == SideSize.Smaller &&
                       segmentHitBox[coordinatePlane, sideSize]?.Side < MainHitBox[coordinatePlane, sideSize]?.Side)
                {
                    return false;
                }
                else if (sideSize == SideSize.Larger &&
                    segmentHitBox[coordinatePlane, sideSize]?.Side > MainHitBox[coordinatePlane, sideSize]?.Side)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void AddSegmentHitBox(Box segmentHitBox)
    {
        if (IsAllCoordinateIncludedMainHitbox(segmentHitBox) == false)
            return;

        SegmentedHitbox.Add(segmentHitBox);
    }
    public void AddSegmentHitBox(Dictionary<(CoordinatePlane, SideSize), HitBoxSide> segmentHitBox, string title)
    {

        if (IsAllCoordinateIncludedMainHitbox(segmentHitBox) == false)
            return;
        Box box = new Box(segmentHitBox, title);
        SegmentedHitbox.Add(box);
    }



    public void SetOffset(CoordinatePlane plane, SideSize sideSize, double offset)
    {
        MainHitBox[plane, sideSize]?.SetOffset(offset);
    }
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
    public Box? this[int index]
    {
        get
        {
            if (index < 0 || index >= SegmentedHitbox.Count)
                return null;

            return SegmentedHitbox[index];
        }
    }
    public HitBoxSide? this[CoordinatePlane plane, SideSize sideSize]
    {
        get
        {
            return MainHitBox[plane, sideSize];
        }
    }

}
