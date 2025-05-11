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
/// <summary>
/// Represents a volumetric hitbox composed of multiple sides aligned to coordinate planes.
/// Each side defines a portion of the hitbox's boundary in 3D space.
/// The <see cref="Box"/> class provides access to these sides for collision detection,
/// rendering, and spatial analysis in physics or game engines.
/// </summary>
public class Box
{
    /// <summary>Edge color for hitbox rendering</summary>
    public Color RenderColor { get; set; } = Color.Red;
    /// <summary>Method for determining the depth of a figure</summary>
    public RenderHeightMode HeightRenderMode { get; set; } = RenderHeightMode.EdgeBased;
    /// <summary>List of all hitbox edges</summary>
    public Dictionary<(CoordinatePlane, SideSize), HitBoxSide> Body { get; set; } = new();
    /// <summary>
    /// Gets or sets the title of the box.
    /// </summary>
    public string Title { get; set; } = string.Empty;


    /// <summary>
    /// Initializes a new instance of the <see cref="Box"/> class by copying another box.
    /// </summary>
    /// <param name="box">The box to copy from.</param>
    public Box(Box box)
    {
        foreach (var pair in box.Body)
        {
            Body.Add(pair.Key, new HitBoxSide(pair.Value));
        }

        RenderColor = box.RenderColor;
        HeightRenderMode = box.HeightRenderMode;
        Title = box.Title;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Box"/> class with a predefined body and title.
    /// </summary>
    /// <param name="body">A dictionary that maps coordinate planes and side sizes to hitbox sides.</param>
    /// <param name="title">The title of the box.</param>
    public Box(Dictionary<(CoordinatePlane, SideSize), HitBoxSide> body, string title)
    {
        Body = body;
        Title = title;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Box"/> class with a specified title.
    /// </summary>
    /// <param name="title">The title of the box.</param>
    public Box(string title)
    {
        Title = title;
    }


    /// <summary>
    /// Gets a list of <see cref="HitBoxSide"/>s from the box that match the specified coordinate plane.
    /// </summary>
    /// <param name="side">The coordinate plane to filter by.</param>
    /// <returns>A list of hitbox sides on the specified plane.</returns>
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

    /// <summary>
    /// Gets the <see cref="HitBoxSide"/> associated with the specified coordinate plane and side size.
    /// </summary>
    /// <param name="side">The coordinate plane.</param>
    /// <param name="sideSize">The size of the side (e.g., smaller or larger).</param>
    /// <returns>
    /// The corresponding hitbox side, or <c>null</c> if no match is found.
    /// </returns>
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
