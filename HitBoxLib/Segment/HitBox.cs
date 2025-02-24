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


namespace HitBoxLib.HitBoxSegment;
public class HitBox
{
    public static readonly SideType[] AllSides = (SideType[])Enum.GetValues(typeof(SideType));
    public Box MainHitBox { get; set; }
    public List<Box> SegmentedHitbox { get; set; } = new();

    public HitBox(string titleMainHitBox = "Body")
    {
        MainHitBox = new Box(AddAllSides(), titleMainHitBox);
    }
    public HitBox(double offset, string titleMainHitBox = "Body")
    {
        MainHitBox = new Box(AddAllSides(offset), titleMainHitBox);
    }



    public UniqueDictionary<SideType, HitBoxSide> AddAllSides()
    {
        UniqueDictionary<SideType, HitBoxSide> addedHitBox = new();

        foreach (var side in AllSides)
            addedHitBox.Insert(side, new HitBoxSide(GetCoordinatePlane(side), GetSizeSide(side)));

        return addedHitBox;
    }
    public UniqueDictionary<SideType, HitBoxSide> AddAllSides(double offset)
    {
        UniqueDictionary<SideType, HitBoxSide> addedHitBox = new();

        foreach (var side in AllSides)
            addedHitBox.Insert(side, new HitBoxSide(GetCoordinatePlane(side), GetSizeSide(side), offset));

        return addedHitBox;
    }


    public bool IsAllSidesIncludedMainHitbox(Box segmentHitBox)
    {
        var sideTypeSegmentHitBox = segmentHitBox.Body.GetAllKey();
        foreach (var side in MainHitBox.Body.GetAllKey())
        {
            if (sideTypeSegmentHitBox.Contains(side) == false)
                return false;
        }

        return true;
    }
    public bool IsAllSidesIncludedMainHitbox(UniqueDictionary<SideType, HitBoxSide> segmentHitBox)
    {
        var sideTypeSegmentHitBox = segmentHitBox.GetAllKey();
        foreach (var side in MainHitBox.Body.GetAllKey())
        {
            if (sideTypeSegmentHitBox.Contains(side) == false)
                return false;
        }

        return true;
    }


    public bool IsAllCoordinateIncludedMainHitbox(Box segmentHitBox)
    {
        if (IsAllSidesIncludedMainHitbox(segmentHitBox) == false)
            return false;

        foreach (var side in MainHitBox.Body.GetAllKey())
        {
            if (segmentHitBox[side]?.SideSize == SideSize.Smaller &&
                segmentHitBox[side]?.Side < MainHitBox[side]?.Side)
            {
                return false;
            }
            else if (segmentHitBox[side]?.SideSize == SideSize.Larger &&
                segmentHitBox[side]?.Side > MainHitBox[side]?.Side)
            {
                return false;
            }
        }

        return true;
    }
    public bool IsAllCoordinateIncludedMainHitbox(UniqueDictionary<SideType, HitBoxSide> segmentHitBox)
    {
        if (IsAllSidesIncludedMainHitbox(segmentHitBox) == false)
            return false;

        foreach (var side in MainHitBox.Body.GetAllKey())
        {
            if (segmentHitBox[side]?.Offset < 0)
            {
                if (MainHitBox[side]?.Offset < 0 && MainHitBox[side]?.Offset > segmentHitBox[side]?.Offset)
                    return false;
                else if (MainHitBox[side]?.Offset > 0)
                    return false;
            }
            else if (segmentHitBox[side]?.Offset > 0)
            {
                if (MainHitBox[side]?.Offset > 0 && MainHitBox[side]?.Offset < segmentHitBox[side]?.Offset)
                    return false;
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
    public void AddSegmentHitBox(UniqueDictionary<SideType, HitBoxSide> segmentHitBox, string title)
    {

        if (IsAllCoordinateIncludedMainHitbox(segmentHitBox) == false)
            return;
        Box box = new Box(segmentHitBox, title);
        SegmentedHitbox.Add(box);
    }



    public void SetOffset(SideType side, double offset)
    {
        MainHitBox[side]?.SetOffset(offset);
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

    private CoordinatePlane GetCoordinatePlane(SideType side)
    {
        return side switch
        {
            SideType.Left or SideType.Right => CoordinatePlane.X,
            SideType.Top or SideType.Bottom => CoordinatePlane.Y,
            SideType.Up or SideType.Down => CoordinatePlane.Z,
            _ => throw new ArgumentOutOfRangeException(nameof(side), $"Unknown side: {side}")
        };
    }
    private SideSize GetSizeSide(SideType side)
    {
        return side switch
        {
            SideType.Left or SideType.Top or SideType.Down => SideSize.Smaller,
            SideType.Right or SideType.Bottom or SideType.Up => SideSize.Larger,
            _ => throw new ArgumentOutOfRangeException(nameof(side), $"Unknown side: {side}")
        };
    }


    public HitBoxSide? this[SideType side]
    {
        get => MainHitBox[side];
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

}
