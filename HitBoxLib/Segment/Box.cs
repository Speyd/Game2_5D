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
    /// <summary>Edge color for hitbox rendering</summary>
    public Color RenderColor { get; set; } = Color.Red;
    /// <summary>Method for determining the depth of a figure</summary>
    public RenderHeightMode HeightRenderMode { get; set; } = RenderHeightMode.EdgeBased;
    /// <summary>List of all hitbox edges</summary>
    public Dictionary<(CoordinatePlane, SideSize), HitBoxSide> Body { get; set; } = new();
    public string Title { get; set; } = string.Empty;

    public Box(Box box)
    {
        Body = box.Body;
        Title = box.Title;
    }
    public Box(Dictionary<(CoordinatePlane, SideSize), HitBoxSide> body, string title)
    {
        Body = body;
        Title = title;
    }
    public Box(string title)
    {
        Title = title;
    }

    public List<HitBoxSide> this[CoordinatePlane side]
    {
        get
        {
            return Body.Where(pair => pair.Value.CoordinatePlane == side)
                 .Where(pair => pair.Key.Item1 == side) 
                .Select(pair => pair.Value)           
                .ToList();
        }
    }

    public HitBoxSide? this[CoordinatePlane side, SideSize sideSize]
    {
        get
        {
            if (Body.TryGetValue((side, sideSize), out var hitBoxSide))
                return hitBoxSide;

            return null;
        }
    }

}
