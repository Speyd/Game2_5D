using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DataPipes.Dictionary;
using HitBoxLib.PositionObject;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using static System.Formats.Asn1.AsnWriter;
using static SFML.Window.Mouse;
namespace HitBoxLib
{
    public class Box
    {
        public SFML.Graphics.Color RenderColor { get; set; } = Color.Red;
        public HitboxHeightMode HeightRenderMode { get; set; } = HitboxHeightMode.EdgeBased;

        public UniqueDictionary<HitBoxSideType, HitBoxSide> Body { get; set; } = new();
        public string Title { get; set; } = string.Empty;

        public Box(Box box)
        {
            Body = box.Body;
            Title = box.Title;
        }
        public Box(UniqueDictionary<HitBoxSideType, HitBoxSide> body, string title)
        {
            Body = body;
            Title = title;
        }
        public Box(string title)
        {
            Title = title;
        }


        public HitBoxSide? this[HitBoxSideType side]
        {
            get => Body.GetValue(side);
        }

        public List<HitBoxSide> this[CoordinatePlane side]
        {
            get
            {
                return Body.GetUniqueDictionary()
                    .Where(pair => pair.Value.CoordinatePlane == side)
                    .Select(pair => pair.Value)
                    .ToList();
            }
        }

        public HitBoxSide? this[CoordinatePlane side, SideSize sideSize]
        {
            get
            {
                return Body.GetUniqueDictionary()
                    .Where(pair => pair.Value.CoordinatePlane == side && pair.Value.SideSize == sideSize)
                    .Select(pair => pair.Value)
                    .First();
            }
        }

    }
}
