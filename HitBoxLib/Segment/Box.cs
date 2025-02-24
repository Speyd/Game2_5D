using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DataPipes.Dictionary;
using HitBoxLib.Operations;
using HitBoxLib.PositionObject;
using HitBoxLib.Segment.SignsTypeSide;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using static System.Formats.Asn1.AsnWriter;
using static SFML.Window.Mouse;


namespace HitBoxLib.HitBoxSegment;
public class Box
{
    public Color RenderColor { get; set; } = Color.Red;
    public RenderHeightMode HeightRenderMode { get; set; } = RenderHeightMode.EdgeBased;

    public UniqueDictionary<SideType, HitBoxSide> Body { get; set; } = new();
    public string Title { get; set; } = string.Empty;

    public Box(Box box)
    {
        Body = box.Body;
        Title = box.Title;
    }
    public Box(UniqueDictionary<SideType, HitBoxSide> body, string title)
    {
        Body = body;
        Title = title;
    }
    public Box(string title)
    {
        Title = title;
    }


    public HitBoxSide? this[SideType side]
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
