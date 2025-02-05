using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;
using ScreenLib.SettingScreen;
using HitBoxLib.PositionObject;
using DataPipes.Dictionary;

namespace HitBoxLib
{
    public class HitBox
    {
        public static readonly HitBoxSideType[] AllSides = (HitBoxSideType[])Enum.GetValues(typeof(HitBoxSideType));
        public UniqueDictionary<HitBoxSideType, HitBoxSide> MainHitBox { get; set; } = new();


        public HitBox()
        {
            AddAllSides();
        }
        public HitBox(double offset)
        {
            AddAllSides(offset);
        }



        public void AddAllSides()
        {
            foreach (var side in AllSides)
                MainHitBox.Insert(side, new HitBoxSide(GetCoordinatePlane(side), GetSizeSide(side)));
        }
        public void AddAllSides(double offset)
        {
            foreach (var side in AllSides)
                MainHitBox.Insert(side, new HitBoxSide(GetCoordinatePlane(side), GetSizeSide(side), offset));
        }
        public void SetOffset(HitBoxSideType side, double offset)
        {
            MainHitBox[side]?.SetOffset(offset);
        }

        private CoordinatePlane GetCoordinatePlane(HitBoxSideType side)
        {
            return side switch
            {
                HitBoxSideType.Left or HitBoxSideType.Right => CoordinatePlane.X,
                HitBoxSideType.Top or HitBoxSideType.Bottom => CoordinatePlane.Y,
                HitBoxSideType.UpSide or HitBoxSideType.DownSide => CoordinatePlane.Z,
                _ => throw new ArgumentOutOfRangeException(nameof(side), $"Unknown side: {side}")
            };
        }
        private SideSize GetSizeSide(HitBoxSideType side)
        {
            return side switch
            {
                HitBoxSideType.Left or HitBoxSideType.Top or HitBoxSideType.DownSide => SideSize.Smaller,
                HitBoxSideType.Right or HitBoxSideType.Bottom or HitBoxSideType.UpSide => SideSize.Larger,
                _ => throw new ArgumentOutOfRangeException(nameof(side), $"Unknown side: {side}")
            };
        }

        public HitBoxSide? this[HitBoxSideType side]
        {
            get => MainHitBox.GetValue(side);
        }
        public List<HitBoxSide> this[CoordinatePlane side]
        {
            get
            {
                return MainHitBox.GetUniqueDictionary()
                    .Where(pair => pair.Value.CoordinatePlane == side)
                    .Select(pair => pair.Value)
                    .ToList();
            }
        }
    }
}
